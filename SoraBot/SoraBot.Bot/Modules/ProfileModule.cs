﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SoraBot.Bot.Models;
using SoraBot.Common.Extensions.Modules;
using SoraBot.Common.Utils;
using SoraBot.Data.Dtos.Profile;
using SoraBot.Data.Repositories.Interfaces;
using SoraBot.Services.Cache;
using SoraBot.Services.Profile;
using SoraBot.Services.Utils;

namespace SoraBot.Bot.Modules
{
    [Name("Profile")]
    [Summary("All commands concerning your profile card, level etc.")]
    public class ProfileModule : SoraSocketCommandModule
    {
        private readonly ImageGenerator _imgGen;
        private readonly IProfileRepository _profileRepo;
        private readonly ICacheService _cacheService;
        private readonly HttpClientHelper _hch;
        private readonly ILogger<ProfileModule> _log;

        private const int _SET_BG_COOLDOWN_S = 45;

        public ProfileModule(
            ImageGenerator imgGen, 
            IProfileRepository profileRepo, 
            ICacheService cacheService,
            HttpClientHelper hch,
            ILogger<ProfileModule> log)
        {
            _imgGen = imgGen;
            _profileRepo = profileRepo;
            _cacheService = cacheService;
            _hch = hch;
            _log = log;
        }

        [Command("removebg"), Alias("rmbg")]
        [Summary("Remove your custom profile card background to reset it to the default")]
        public async Task RemoveBg()
        {
            var path = Path.Combine(_imgGen.ImageGenPath, ImageGenerator.PROFILE_BG,
                $"{Context.User.Id.ToString()}.png");
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            await _profileRepo.SetUserHasBgBoolean(Context.User.Id, false);
            await ReplySuccessEmbed("Successfully reset to default background image");
        }

        [Command("setbg"), Alias("setbackground", "sbg")]
        [Summary("Give Sora a link to an image or attach an image to set it as your " +
                 "profile card background")]
        public async Task SetBg(
            [Summary("Direct link to image. If you leave this blank you must provide an attachment!")]
            string url = null)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                if (Context.Message.Attachments.Count != 1)
                {
                    await ReplyFailureEmbed("If you do not provide an image link you MUST provide 1 Attached image");
                    return;
                }
                url = Context.Message.Attachments.First().Url;
            }
            // Check if URL is valid
            if (Helper.LinkIsNoImage(url))
            {
                await ReplyFailureEmbedExtended("The provided link or attachment is not an image!",
                    "Make sure the link ends with any of these extensions: `.jpg, .png, .gif, .jpeg`");
                return;
            }
            // Check cooldown
            var cd = _cacheService.Get<DateTime>(CacheId.BgCooldownId(Context.User.Id));
            if (cd.HasValue)
            {
                var secondsRemaining = cd.Some().Subtract(DateTime.UtcNow.TimeOfDay).Second;
                await ReplyFailureEmbed(
                    $"Dont break me >.< Please wait another {secondsRemaining.ToString()} seconds!");
                return;
            }
            // No cooldown so we do the actual stuff. 
            try
            {
                // Download and resize image
                string imageTemp = Path.Combine(_imgGen.ImageGenPath, ImageGenerator.PROFILE_BG,
                    $"{Context.User.Id.ToString()}_temp.png");

                await _hch.DownloadAndSaveFile(new Uri(url), imageTemp).ConfigureAwait(false);
                
                _imgGen.ResizeAndSaveImage(
                    imageTemp, 
                    Path.Combine(_imgGen.ImageGenPath, ImageGenerator.PROFILE_BG, $"{Context.User.Id.ToString()}.png"), 
                    new Size(470, 265));
                
                // Remove temporary one
                File.Delete(imageTemp);
            }
            catch (Exception e)
            {
                await ReplyFailureEmbed("Failed downloading the picture! Try another link.");
                _log.LogWarning(e, $"Failed to download background image for {Context.User.Id.ToString()}");
                return;
            }
            // Set BG on user
            await _profileRepo.SetUserHasBgBoolean(Context.User.Id, true);
            // Add cooldown
            _cacheService.Set(CacheId.BgCooldownId(Context.User.Id), DateTime.UtcNow.AddSeconds(_SET_BG_COOLDOWN_S), TimeSpan.FromSeconds(_SET_BG_COOLDOWN_S));

            await ReplySuccessEmbed("Successfully updated your profile card background :>");
        }

        [Command("profile"), Alias("p")]
        [Summary("Shows your or the @mentioned user's profile card with level and rank stats")]
        public async Task GenerateImage(
            [Summary("@User or leave blank to get your own")]
            DiscordGuildUser userT = null)
        {
            var user = userT?.GuildUser ?? (IGuildUser)Context.User;
            
            var userStatsM = await _profileRepo.GetProfileStatistics(user.Id, Context.Guild.Id).ConfigureAwait(false);
            if (!userStatsM.HasValue)
            {
                await ReplyFailureEmbed(
                    $"{Formatter.UsernameDiscrim(user)} is not in my Database :/ Make sure he used or chatted with Sora at least once.");
                return;
            }

            try
            {
                // First get his avatar
                await _hch.DownloadAndSaveFile(
                        new Uri(user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl()),
                        Path.Combine(_imgGen.ImageGenPath, ImageGenerator.AVATAR_CACHE, $"{user.Id.ToString()}.png"))
                    .ConfigureAwait(false);

                // Now generate image
                string filePath = Path.Combine(_imgGen.ImageGenPath, ImageGenerator.PROFILE_CARDS,
                    $"{user.Id.ToString()}.png");

                var us = ~userStatsM;
                var globalLevel = ExpService.CalculateLevel(us.GlobalExp);
                var localLevel = ExpService.CalculateLevel(us.LocalExp);
                _imgGen.GenerateProfileImage(new ProfileImageGenDto()
                {
                    UserId = user.Id,
                    Name = user.Username,
                    GlobalExp = us.GlobalExp,
                    GlobalLevel = globalLevel,
                    GlobalRank = us.GlobalRank,
                    GlobalNextLevelExp = ExpService.CalculateNeededExp(globalLevel + 1),
                    HasCustomBg = us.HasCustomBg,
                    LocalExp = us.LocalExp,
                    LocalRank = us.LocalRank,
                    LocalLevel = localLevel,
                    LocalNextLevelExp = ExpService.CalculateNeededExp(localLevel + 1),
                    ClanName = us.ClanName
                }, filePath);
                await Context.Channel.SendFileAsync(filePath);
            }
            catch (Exception e)
            {
                await ReplyFailureEmbedExtended(
                    "Failed to generate image. Something went wrong sorry :/",
                    "This could have multiple reasons. One of them could be that your username has characters that " +
                    "are currently not supported. This is any weird character that you wouldn't naturally find on your standard " +
                    "keyboard.");
                if (e.InnerException is NotImplementedException)
                    return; // Since we dont care about exceptions about not supported text and shit. Can't do anything about that
                _log.LogError(e, $"Failed to generate image for {user.Id.ToString()} ({user.Username})");
            }
            finally
            {
                // Remove avatar
                string avatar = Path.Combine(_imgGen.ImageGenPath, ImageGenerator.AVATAR_CACHE,
                    $"{user.Id.ToString()}.png");
                if (File.Exists(avatar))
                    File.Delete(avatar);

                // Remove profile image
                string profileImg = Path.Combine(_imgGen.ImageGenPath, ImageGenerator.PROFILE_CARDS,
                    $"{user.Id.ToString()}.png");
                if (File.Exists(profileImg))
                    File.Delete(profileImg);
            }
        }
    }
}