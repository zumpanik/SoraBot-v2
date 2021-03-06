﻿using System.Linq;
using System.Threading.Tasks;
using ArgonautCore.Lw;
using Microsoft.EntityFrameworkCore;
using SoraBot.Data.Extensions;
using SoraBot.Data.Models.SoraDb;
using SoraBot.Data.Repositories.Interfaces;

namespace SoraBot.Data.Repositories
{
    public class GuildRepository : IGuildRepository
    {
        private readonly ITransactor<SoraContext> _soraTransactor;

        public GuildRepository(ITransactor<SoraContext> soraTransactor)
        {
            _soraTransactor = soraTransactor;
        }

        public async Task<string> GetGuildPrefix(ulong id)
            => await _soraTransactor.DoAsync(async context =>
                await context.Guilds.Where(g => g.Id == id).Select(x => x.Prefix).FirstOrDefaultAsync()
            ).ConfigureAwait(false);

        public async Task<bool> SetGuildPrefix(ulong id, string prefix)
        {
            // let's at least test this
            if (string.IsNullOrWhiteSpace(prefix)) return false;
            return await _soraTransactor.TryDoInTransactionAsync(async context =>
            {
                var guild = await GetOrSetAndGetGuild(id, context).ConfigureAwait(false);
                guild.Prefix = prefix;
                await context.SaveChangesAsync().ConfigureAwait(false);
                return true;
            }).ConfigureAwait(false);
        }

        public async Task<Option<Guild>> GetOrSetAndGetGuild(ulong id)
            => await _soraTransactor.DoInTransactionAndGetAsync(async context
                => Option.Some(await GetOrSetAndGetGuild(id, context).ConfigureAwait(false))
            ).ConfigureAwait(false);

        public async Task<Guild> GetGuild(ulong id)
            => await _soraTransactor.DoAsync(async context
                => await context.Guilds.FindAsync(id).ConfigureAwait(false)
            ).ConfigureAwait(false);

        public async Task RemoveGuild(ulong id)
            => await _soraTransactor.DoInTransactionAsync(async context =>
            {
                var guild = await context.Guilds.FindAsync(id).ConfigureAwait(false);
                if (guild == null) return;
                context.Guilds.Remove(guild);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }).ConfigureAwait(false);

        public async Task TryAddGuildUserExp(ulong guildId, ulong userId, uint expToAdd)
            => await _soraTransactor.DoInTransactionAsync(async context =>
            {
                var guildUser = await GetOrCreateGuildUser(guildId, userId, context).ConfigureAwait(false);
                guildUser.Exp += expToAdd;
                await context.SaveChangesAsync().ConfigureAwait(false);
            }).ConfigureAwait(false);

        /// <summary>
        /// Tries to get a GuildUser. If it cannot find one it creates one and adds it to the guildUser Dbset to be tracked.
        /// This does NOT save tho!
        /// If it cannot find a user it will CREATE AND SAVE a guild object bcs of the foreign key constraint!
        /// </summary>
        public static async Task<GuildUser> GetOrCreateGuildUser(ulong guildId, ulong userId, SoraContext context)
        {
            var guildUser = await context.GuildUsers
                .FirstOrDefaultAsync(x => x.UserId == userId && x.GuildId == guildId).ConfigureAwait(false);
            if (guildUser != null) return guildUser;
            // Create a user and return him
            // Because of the foreign key constraints we have to make sure a guild exists and a user
            // This will create the guild
            await GetOrSetAndGetGuildNoSave(guildId, context).ConfigureAwait(false);
            await context.Users.GetOrCreateUserNoSaveAsync(userId).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);
            // Now we add the user
            guildUser = new GuildUser(userId, guildId, 0);
            context.GuildUsers.Add(guildUser);
            return guildUser;
        }
        
        /// <summary>
        /// Tries to find a Guild and if it can't it'll create one and already save! 
        /// </summary>
        public static async Task<Guild> GetOrSetAndGetGuild(ulong id, SoraContext context)
        {
            var guild = await context.Guilds.FindAsync(id).ConfigureAwait(false);
            if (guild != null) return guild;
            // Create guild, save it and give it back
            guild = new Guild(id);
            // ReSharper disable once MethodHasAsyncOverload
            context.Guilds.Add(guild);
            await context.SaveChangesAsync().ConfigureAwait(false);
            return guild;
        }
        
        /// <summary>
        /// Tries to find a Guild and if it can't it'll create one but DONT save 
        /// </summary>
        public static async Task<Guild> GetOrSetAndGetGuildNoSave(ulong id, SoraContext context)
        {
            var guild = await context.Guilds.FindAsync(id).ConfigureAwait(false);
            if (guild != null) return guild;
            // Create guild, give it back
            guild = new Guild(id);
            // ReSharper disable once MethodHasAsyncOverload
            context.Guilds.Add(guild);
            return guild;
        }
    }
}