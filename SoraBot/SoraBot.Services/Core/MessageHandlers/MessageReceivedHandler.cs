﻿using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using SoraBot.Common.Messages;
using SoraBot.Common.Messages.MessageAdapters;

namespace SoraBot.Services.Core.MessageHandlers
{
    public class MessageReceivedHandler : IMessageHandler<MessageReceived>
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DiscordSocketCoreListeningBehavior> _logger;

        public MessageReceivedHandler(
            DiscordSocketClient client,
            CommandService commandService,
            IServiceProvider serviceProvider,
            ILogger<DiscordSocketCoreListeningBehavior> logger)
        {
            _client = client;
            _commandService = commandService;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
        
        public async Task HandleMessageAsync(MessageReceived msg, CancellationToken cancellationToken = default)
        {
            var m = msg.Message;
            // Make sure its a SocketUserMessage and the author is not null
            if (!(m is SocketUserMessage message) || m.Author == null)
                return;
            
            // Return if the message originates from a bot or webhook
            if (message.Author.IsBot || message.Author.IsWebhook)
                return;
            
            // Make sure we only reply to messages in guilds and not DMs
            if (!(m.Channel is SocketGuildChannel channel))
                return;

            string prefix = "beta!"; // TODO CHANGE THIS
            
            // Can't possibly be a command. Safe some cpu cycles
            if (message.Content.Length <= prefix.Length)
                return;

            // Check if the message starts with the prefix or mention before we
            // try and do anything else.
            int argPos = 0;
            if (!(message.HasStringPrefix(prefix, ref argPos) ||
                  message.HasMentionPrefix(_client.CurrentUser, ref argPos)))
                return;

            // We no longer have to create a new scope in here since its done in the Message dispatch
            var context = new SocketCommandContext(_client, message);
            var timer = new Stopwatch();
            timer.Start();
            IResult commandResult;
            try
            {
                commandResult = await _commandService.ExecuteAsync(context, argPos, _serviceProvider);
            }
            finally
            {
                timer.Stop();
                _logger.LogInformation("Executed command {Command} in {Time} ms", message.Content.Substring(argPos), timer.ElapsedMilliseconds.ToString());
            }

            if (!commandResult.IsSuccess)
            {
                await HandleErrorAsync(commandResult, context).ConfigureAwait(false);
            }
        }
        
        // TODO redo these error messages
        private async Task HandleErrorAsync(IResult result, SocketCommandContext context,
            CommandException exception = null)
        {
            switch (result.Error)
            {
                case CommandError.Exception:
                    if (exception != null && exception.InnerException != null)
                    {
                        _logger.LogError(exception.InnerException, $"Command Exception with: {exception.Command}");
                    }
                    break;
                case CommandError.BadArgCount:
                    await context.Channel.SendMessageAsync(result.ErrorReason);
                    break;
                case CommandError.UnknownCommand:
                    break;
                case CommandError.ParseFailed:
                    await context.Channel.SendMessageAsync("Parse Failed");
                    break;
                default:
                    await context.Channel.SendMessageAsync("Some other failiure");
                    break;
            }
        }
    }
}