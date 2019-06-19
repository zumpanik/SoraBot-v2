﻿using Microsoft.EntityFrameworkCore;
using SoraBot_v2.Data.Entities;
using SoraBot_v2.Data.Entities.SubEntities;
using SoraBot_v2.Services;
using System.IO;
using SoraBot_v2.WebApiModels;

namespace SoraBot_v2.Data
{
    public class SoraContext : DbContext
    {
        //User Database
        public DbSet<User> Users { get; set; }
        public DbSet<Interactions> Interactions { get; set; }
        public DbSet<Afk> Afk { get; set; }
        public DbSet<Reminders> Reminders { get; set; }
        public DbSet<Marriage> Marriages { get; set; }
        public DbSet<ShareCentral> ShareCentrals { get; set; }
        public DbSet<Voting> Votings { get; set; }
        public DbSet<UserWaifu> UserWaifus { get; set; }
        
        // Waifu
        public DbSet<Waifu> Waifus { get; set; }
        public DbSet<WaifuRequest> WaifuRequests { get; set; }
        public DbSet<RequestLog> RequestLogs { get; set; }

        //Guild Database
        public DbSet<Guild> Guilds { get; set; }
        public DbSet<Tags> Tags { get; set; }
        public DbSet<StarMessage> StarMessages { get; set; }
        public DbSet<Role> SelfAssignableRoles { get; set; }
        public DbSet<ExpiringRole> ExpiringRoles { get; set; }
        public DbSet<ModCase> Cases { get; set; }
        public DbSet<GuildUser> GuildUsers { get; set; }
        public DbSet<GuildLevelRole> GuildLevelRoles { get; set; }
        
        //Clan
        public DbSet<Clan> Clans { get; set; }
        public DbSet<ClanInvite> ClanInvites { get; set; }

        //Song list
        public DbSet<Song> Songs { get; set; }
        
        // Bans
        public DbSet<Ban> Bans { get; set; }

        //private static volatile object _padlock = new Object();

        /*
        public SoraContext(string con)
        {
            _connectionString =con;
        }*/
        public SoraContext() : base()
        {

        }

        /*
        public SoraContext()
        {
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(@"");
        }*/


        //// Added by Catherine Renelle - Memory Leak Fix (also improves migration code)
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString;

            if (!ConfigService.GetConfig().TryGetValue("connectionString", out connectionString))
            {
                throw new IOException
                {
                    Source = "Couldn't find a \"connectionString\" entry in the config.json file. Exiting."
                };
            }

            optionsBuilder.UseMySql(connectionString);
        }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RequestLog>(x => { x.HasKey(d => d.Id); });
            
            modelBuilder.Entity<WaifuRequest>(x => { x.HasKey(d => d.Id); });
            
            modelBuilder.Entity<Interactions>(x =>
            {
                x.HasOne(d => d.User)
                    .WithOne(p => p.Interactions)
                    .HasForeignKey<Interactions>(g => g.UserForeignId);
            });

            modelBuilder.Entity<UserWaifu>(x =>
            {
                x.HasOne(u => u.User)
                    .WithMany(w => w.UserWaifus)
                    .HasForeignKey(k => k.UserForeignId);
            });
            
            modelBuilder.Entity<ShareCentral>(x =>
            {
                x.HasOne(d => d.User)
                    .WithMany(p => p.ShareCentrals)
                    .HasForeignKey(p => p.CreatorId);
            });

            modelBuilder.Entity<Voting>(x =>
            {
                x.HasOne(d => d.User)
                    .WithMany(p => p.Votings)
                    .HasForeignKey(p => p.ShareLink)
                    .HasForeignKey(p => p.VoterId);
            });

            modelBuilder.Entity<Tags>(x =>
            {
                x.HasOne(g => g.Guild)
                    .WithMany(p => p.Tags)
                    .HasForeignKey(g => g.GuildForeignId);
            });

            modelBuilder.Entity<GuildLevelRole>(x =>
            {
                x.HasOne(g => g.Guild)
                    .WithMany(p => p.LevelRoles)
                    .HasForeignKey(g => g.GuildId);
            });

            modelBuilder.Entity<Role>(x =>
            {
                x.HasOne(g => g.Guild)
                    .WithMany(p => p.SelfAssignableRoles)
                    .HasForeignKey(g => g.GuildForeignId);
            });

            modelBuilder.Entity<StarMessage>(x =>
            {
                x.HasOne(g => g.Guild)
                    .WithMany(s => s.StarMessages)
                    .HasForeignKey(g => g.GuildForeignId);
            });

            modelBuilder.Entity<GuildUser>(u =>
            {
                u.HasOne(g => g.Guild)
                    .WithMany(i => i.Users)
                    .HasForeignKey(g => g.GuildId);
            });

            modelBuilder.Entity<Reminders>(x =>
            {
                x.HasOne(g => g.User)
                    .WithMany(p => p.Reminders)
                    .HasForeignKey(g => g.UserForeignId);
            });

            modelBuilder.Entity<Marriage>(x =>
            {
                x.HasOne(g => g.User)
                    .WithMany(p => p.Marriages)
                    .HasForeignKey(g => g.UserForeignId);
            });
        }
    }
}