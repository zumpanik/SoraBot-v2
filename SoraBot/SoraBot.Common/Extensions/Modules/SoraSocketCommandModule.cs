﻿using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using SoraBot.Common.Utils;

namespace SoraBot.Common.Extensions.Modules
{
    public class SoraSocketCommandModule : ModuleBase<SocketCommandContext>
    {
        public static readonly Color Purple = new Color(109, 41, 103);
        public static readonly Color Yellow = new Color(255, 204, 77);
        public static readonly Color Green = new Color(119, 178, 85);
        public static readonly Color Red = new Color(221, 46, 68);
        public static readonly Color Blue = new Color(59, 136, 195);

        public const string SuccessEmoji = "✅";
        public const string WarnEmoji = "⚠";
        public const string FailureEmoji = "❌";
        public const string InfoEmoji = "ℹ";
        public const string PartyEmoji = "🎉";
        public const string MusicalNote = "🎵";

        public async Task<IUserMessage> ReplyEmbed(string message,
            Color? embedColor = null, string emoji = null)
        {
            Color color = embedColor ?? Purple;
            return await ReplyAsync("", embed: SimpleEmbed(color, message, emoji).Build());
        }

        public async Task<IUserMessage> ReplySuccessEmbed(string message)
        {
            return await ReplyAsync("", embed: SimpleEmbed(Green, message, SuccessEmoji).Build());
        }

        public async Task<IUserMessage> ReplyFailureEmbed(string message)
        {
            return await ReplyAsync("", embed: SimpleEmbed(Red, message, FailureEmoji).Build());
        }
        
        public async Task<IUserMessage> ReplyWarningEmbed(string message)
        {
            return await ReplyAsync("", embed: SimpleEmbed(Yellow, message, WarnEmoji).Build());
        }
        
        public async Task<IUserMessage> ReplyInfoEmbed(string message)
        {
            return await ReplyAsync("", embed: SimpleEmbed(Blue, message, InfoEmoji).Build());
        }
        
        public async Task<IUserMessage> ReplyDefaultEmbed(string message)
        {
            return await ReplyAsync("", embed: SimpleEmbed(Purple, message).Build());
        }

        public static EmbedFooterBuilder RequestedByFooter(IUser user)
        {
            return new EmbedFooterBuilder()
            {
                Text = $"Requested by {Formatter.UsernameDiscrim(user)}",
                IconUrl = user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl()
            };
        }

        public static EmbedBuilder SimpleEmbed(Color color, string text)
        {
            var eb = new EmbedBuilder()
            {
                Color = color,
                Title = text
            };
            return eb;
        }
        
        public static EmbedBuilder SimpleEmbed(Color color, string text, string symbol)
        {
            var eb = new EmbedBuilder()
            {
                Color = color,
                Title = $"{symbol} {text}"
            };
            return eb;
        }
    }
}