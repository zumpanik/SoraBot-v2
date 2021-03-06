﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SoraBot.Bot.Extensions;
using SoraBot.Bot.Modules.AudioModule;
using SoraBot.Data.Configurations;
using SoraBot.Services.Misc;
using SoraBot.Services.Reminder;
using SoraBot.Services.Utils;
using Victoria;

namespace SoraBot.Bot
{
    // HEAVILY copied/inspired by Modix. Never worked with BackgroundServices so I had to look that up :)
    public sealed class SoraBot : BackgroundService
    {
        private readonly ILogger<SoraBot> _logger;
        private readonly DiscordSocketClient _socketClient;
        private readonly DiscordRestClient _restClient;
        private readonly CommandService _commandService;
        private readonly IServiceProvider _serviceProvider;
        private readonly DiscordSerilogAdapter _serilogAdapter;
        private readonly WeebService _weebService;
        private readonly LavaNode _lavaNode;

        private IServiceScope _scope;
        private readonly SoraBotConfig _config;

        public SoraBot(
            ILogger<SoraBot> logger,
            DiscordSocketClient socketClient,
            DiscordRestClient restClient,
            CommandService commandService,
            IServiceProvider serviceProvider,
            DiscordSerilogAdapter serilogAdapter,
            IOptions<SoraBotConfig> soraConfig,
            WeebService weebService,
            LavaNode lavaNode)
        {
            _logger = logger;
            _socketClient = socketClient;
            _restClient = restClient;
            _commandService = commandService;
            _serviceProvider = serviceProvider;
            _serilogAdapter = serilogAdapter;
            _weebService = weebService;
            _lavaNode = lavaNode;
            _config = soraConfig?.Value ?? throw new ArgumentNullException(nameof(soraConfig));
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("SoraBot Background task is starting...");

            IServiceScope scope = null;
            try
            {
                scope = _serviceProvider.CreateScope();
                
                _logger.LogTrace("Registering listeners for Discord client events.");

                _socketClient.Disconnected += OnDisconnect;
                _socketClient.UserVoiceStateUpdated += OnUserVoiceStateUpdated;
                
                _socketClient.Log += _serilogAdapter.HandleLog;
                _socketClient.Ready += SocketClientOnReady;
                _restClient.Log += _serilogAdapter.HandleLog;
                _commandService.Log += _serilogAdapter.HandleLog;

                // Register with the cancellation token so we can stop listening to 
                // client events if the service is shutting down or being disposed
                stoppingToken.Register(OnStopping);

                // The only thing that could go wrong at this point is the client failing to login and start. Promote
                // our local service scope to a field so that it's available to the HandleCommand method once events
                // start firing after we've connected.
                _scope = scope;
                
                _logger.LogInformation("Loading command modules...");
                await _commandService.AddModulesAsync(typeof(SoraBot).Assembly, _scope.ServiceProvider);
                // Adding WeebServices and Commands
                try
                {
                    var token = _config.WeebToken;
                    if (_weebService.TryAuthenticate(token).Result)
                    {
                        // Add interactions
                        _logger.LogInformation("Could authenticate WeebService. Adding module and commands.");
                        await _weebService.AddInteractions(_commandService);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed to add Interactions.");
                }
                
                _logger.LogInformation("{Modules} modules loaded, containing {Commands} commands",
                    _commandService.Modules.Count().ToString(), _commandService.Modules.SelectMany(d => d.Commands).Count().ToString());

                await StartClient(stoppingToken);
                _logger.LogInformation("Discord client started successfully.");
                
                _logger.LogInformation("Warming up all services that rely on timers etc.");
                _scope.ServiceProvider.GetRequiredService<IReminderService>();
                _scope.ServiceProvider.GetRequiredService<HealthChecker>();
                _scope.ServiceProvider.GetRequiredService<AudioEventHandler>();

                // This way the background task stays alive 
                // await Task.Delay(-1);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occured while attempting to start the background service.");

                try
                {
                    OnStopping();

                    _logger.LogInformation("Logging out of Discord");
                    await _socketClient.LogoutAsync();
                }
                finally
                {
                    scope?.Dispose();
                    _scope = null;
                }                
                throw;
            }
            
        }
        
        private async Task OnUserVoiceStateUpdated(SocketUser user, SocketVoiceState oldState, SocketVoiceState newState)
        {
            var guild = newState.VoiceChannel?.Guild ?? oldState.VoiceChannel?.Guild;
            if (guild == null) return;
            if (!_lavaNode.TryGetPlayer(guild, out var player))
                return;
            // So this is a guild that has a currently active sora music player. Let's investigate
            if (player.VoiceChannel == null) return; // shouldn't ever happen but we never know 
            
            SocketVoiceChannel vc = guild.CurrentUser.VoiceChannel;
            if (vc == null) return; // Sora is in none so we'll just ignore this.
            
            var userCount = vc.Users.Count(x => !x.IsBot);
            if (userCount > 0)
            {
                // Check if channel is AFK channel
                if (guild.AFKChannel?.Id == player.VoiceChannel.Id)
                {
                    // leave this shit
                    await _lavaNode.LeaveAsync(player.VoiceChannel);
                    return;
                }
                // No action required
                return; 
            }
            
            // Otherwise we leave the VC.
            await _lavaNode.LeaveAsync(player.VoiceChannel);
        }

        private Task SocketClientOnReady()
        {
            // TODO figure out if this is fine or if we should use IsConnected to check
            _lavaNode.ConnectAsync();
            _socketClient.Ready -= SocketClientOnReady;
            return Task.CompletedTask;
        }

        private void OnStopping()
        {
            _logger.LogInformation("Stopping background service.");
            
            _socketClient.Disconnected -= OnDisconnect;
            
            _socketClient.Log -= _serilogAdapter.HandleLog;
            _restClient.Log -= _serilogAdapter.HandleLog;
            _commandService.Log -= _serilogAdapter.HandleLog;
        }

        private async Task StartClient(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await _socketClient.LoginAsync(TokenType.Bot, _config.DiscordToken);
            await _socketClient.StartAsync();

            await _restClient.LoginAsync(TokenType.Bot, _config.DiscordToken);
        }

        private Task OnDisconnect(Exception ex)
        {
            _logger.LogInformation(ex, "The bot disconnected unexpectedly.");
            // _applicationLifetime.StopApplication(); // TODO investigate this
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            try
            {
                // If the service is currently running, this will cancel the cancellation token that was passed into
                // our ExecuteAsync method, unregistering our event handlers for us.
                base.Dispose();

            }
            finally
            {
                _scope?.Dispose();
                _socketClient.Dispose();
                _restClient.Dispose();
            }
        }
    }
}