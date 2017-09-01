﻿using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SoraBot_v2.Data.Entities;
using SoraBot_v2.Services;

namespace SoraBot_v2.Module
{
    public class FunModule : ModuleBase<SocketCommandContext>
    {

        private string[] ball = new[]
        {
            "Signs point to yes. ",
            "Yes.",
            "Reply hazy, try again.",
            "Without a doubt. ",
            "My sources say no. ",
            "As I see it, yes. ",
            "You may rely on it.",
            "Concentrate and ask again.",
            "Outlook not so good. ",
            "It is decidedly so.",
            "Better not tell you now.",
            "Very doubtful. ",
            "Yes - definitely. ",
            "It is certain. ",
            "Cannot predict now. ",
            "Most likely. ",
            "Ask again later. ",
            "My reply is no. ",
            "Outlook good. ",
            "Don't count on it."
        };

        [Command("8ball"), Alias("8b"), Summary("Ask and get an 8ball answer")]
        public async Task Ball([Summary("Question"), Remainder] string question)
        {
            Random r = new Random();
            await Context.Channel.SendMessageAsync("", embed: Utility.ResultFeedback(
                Utility.PurpleEmbed, Utility.SuccessLevelEmoji[4], "🎱 "+ball[r.Next(ball.Length)]));
        } 

        [Command("roll"), Alias("dice"), Summary("Roll a dice")]
        public async Task RollDice()
        {
            Random r = new Random();
            await Context.Channel.SendMessageAsync("", embed: Utility.ResultFeedback(
                Utility.PurpleEmbed, Utility.SuccessLevelEmoji[4], "🎲 Rolled: "+r.Next(1,7)));
        }

        [Command("lenny"), Summary("Lenny's the Chat")]
        public async Task Lenny()
        {
            await ReplyAsync("( ͡° ͜ʖ ͡°)");
        }

        [Command("swag", RunMode = RunMode.Async), Summary("Swags the chat")]
        public async Task Swag()
        {
            var msg = await ReplyAsync("( ͡° ͜ʖ ͡°)>⌐■-■");
            await Task.Delay(1500);
            await msg.ModifyAsync(x => { x.Content = "( ͡⌐■ ͜ʖ ͡-■)"; });
        }

        [Command("door"), Summary("Shows the door to someone")]
        public async Task Door(params SocketUser[] Users)
        {
            if (Users.Length < 1)
            {
                await ReplyAsync("",
                    embed: Utility.ResultFeedback(Utility.RedFailiureEmbed, Utility.SuccessLevelEmoji[2],
                        "You need to specify at least one person to show the door to"));
                return;
            }

            string showDoors = "";
            foreach (var socketUser in Users)
            {
                showDoors += $"{Utility.GiveUsernameDiscrimComb(socketUser)}, ";
            }
            showDoors = showDoors.Remove(showDoors.Length - 2);
            var eb = new EmbedBuilder()
            {
                Color = Utility.PurpleEmbed,
                Description = $"**{showDoors}** 👉🚪"
            };
            await ReplyAsync("", embed: eb);
        }

        [Command("google"), Summary("Googles shit for you")]
        public async Task GoogleForMeOnegaishimasu([Remainder] string google)
        {
            string search = google.Replace(" ", "%20");
            await ReplyAsync("",
                embed: Utility.ResultFeedback(Utility.BlueInfoEmbed, Utility.SuccessLevelEmoji[3], $"{($"Results for \"{google}\"".Length > 200 ? $"Results for \"{google}".Remove(200)+"...\"":$"Results for \"{google}\"")}")
                    .WithUrl($"https://lmgtfy.com/?q={search}"));
        }
        
    }
}