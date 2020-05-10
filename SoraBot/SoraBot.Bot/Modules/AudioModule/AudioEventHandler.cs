﻿using System;
using System.Threading.Tasks;
using Discord;
using Microsoft.Extensions.Logging;
using SoraBot.Common.Extensions.Modules;
using Victoria;
using Victoria.EventArgs;

namespace SoraBot.Bot.Modules.AudioModule
{
    public class AudioEventHandler : IDisposable
    {
        private readonly ILogger<AudioEventHandler> _log;
        private readonly LavaNode _node;

        public AudioEventHandler(ILogger<AudioEventHandler> log, LavaNode node)
        {
            log.LogInformation("Initialized Audio Event Handlers");
            _log = log;
            _node = node;
            
            _node.OnLog += OnLog;
            _node.OnTrackEnded += OnTrackEnded;
            _node.OnTrackStuck += OnTrackStuck;
            _node.OnTrackException += OnTrackException;
            _node.OnWebSocketClosed += OnWebSocketClosed;
            _node.OnStatsReceived += OnStatsReceived;
        }

        private Task OnStatsReceived(StatsEventArgs arg)
        {
            throw new NotImplementedException();
        }

        private Task OnWebSocketClosed(WebSocketClosedEventArgs arg)
        {
            throw new NotImplementedException();
        }

        private async Task OnTrackException(TrackExceptionEventArgs e)
        {
            if (e.Player == null || e.Track == null) 
                return;   
            
            e.Player.Queue.Remove(e.Track);

            var eb = this.GetSimpleMusicEmbed("Track threw and exception. Attempting to play next track");
            if (!string.IsNullOrWhiteSpace(e.ErrorMessage))
                eb.WithDescription(e.ErrorMessage);
            
            await e.Player.TextChannel.SendMessageAsync(
                embed: eb.Build());
            
            await e.Player.SkipAsync();
        }

        private async Task OnTrackStuck(TrackStuckEventArgs e)
        {
            if (e.Player == null || e.Track == null) 
                return;   
            
            e.Player.Queue.Remove(e.Track);

            await e.Player.TextChannel.SendMessageAsync(
                embed: this.GetSimpleMusicEmbed("Track got stuck. Attempting to play next track").Build());
            
            await e.Player.SkipAsync();
        }

        private EmbedBuilder GetSimpleMusicEmbed(string message)
            => new EmbedBuilder()
            {
                Color = SoraSocketCommandModule.Blue,
                Title = $"{SoraSocketCommandModule.MusicalNote} {message}"
            };

        private async Task<EmbedBuilder> GetExtendedMusicEmbed(LavaTrack track)
        {
            var eb = new EmbedBuilder()
            {
                Color = SoraSocketCommandModule.Blue,
                Title = $"{SoraSocketCommandModule.MusicalNote} Next: [{track.Duration.ToString(@"mm\:ss")}] - **{track.Title}**",
                Footer = new EmbedFooterBuilder()
                {
                    Text = $"Video by {track.Author}"
                },
                Url = track.Url,
            };
            var imageUrl = await track.FetchArtworkAsync();
            if (!string.IsNullOrWhiteSpace(imageUrl))
                eb.WithThumbnailUrl(imageUrl);
            return eb;
        }

        private async Task OnTrackEnded(TrackEndedEventArgs e)
        {
            if (!e.Reason.ShouldPlayNext() || e.Player == null)
                return;

            if (!e.Player.Queue.TryDequeue(out var queueable))
            {
                await e.Player.TextChannel.SendMessageAsync(embed: this.GetSimpleMusicEmbed("No more tracks in queue.").Build());
                return;
            }

            if (!(queueable is LavaTrack track))
            {
                await e.Player.TextChannel.SendMessageAsync(
                    embed: this.GetSimpleMusicEmbed("Next item in queue was not a track. Stopped playback..").Build());
                return;
            }
            
            // Queue next song
            await e.Player.PlayAsync(track);
            var eb = await this.GetExtendedMusicEmbed(track);
            await e.Player.TextChannel.SendMessageAsync(embed: eb.Build());
        }

        private Task OnLog(LogMessage log)
        {
            switch (log.Severity)
            {
                case LogSeverity.Critical:
                    _log.LogWarning(log.Exception, log.Message);
                    break;
                case LogSeverity.Error:
                    _log.LogWarning(log.Exception, log.Message);
                    break;
                case LogSeverity.Warning:
                    _log.LogWarning(log.Exception, log.Message);
                    break;
                case LogSeverity.Info:
                    _log.LogInformation(log.Message);
                    break;
                case LogSeverity.Verbose:
                    _log.LogTrace(log.Message);
                    break;
                case LogSeverity.Debug:
                    _log.LogDebug(log.Message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _node.OnLog -= OnLog;
            _node.OnTrackEnded -= OnTrackEnded;
            _node.OnTrackStuck -= OnTrackStuck;
            _node.OnTrackException -= OnTrackException;
            _node.OnWebSocketClosed -= OnWebSocketClosed;
            _node.OnStatsReceived -= OnStatsReceived;
        }
    }
}