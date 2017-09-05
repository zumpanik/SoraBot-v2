﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using SoraBot_v2.Data;
using System;

namespace SoraBotv2.Migrations
{
    [DbContext(typeof(SoraContext))]
    partial class SoraContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .HasAnnotation("ProductVersion", "2.0.0-rtm-26452");

            modelBuilder.Entity("SoraBot_v2.Data.Entities.Guild", b =>
                {
                    b.Property<ulong>("GuildId");

                    b.Property<bool>("IsDjRestricted");

                    b.Property<string>("Prefix");

                    b.Property<bool>("RestrictTags");

                    b.Property<ulong>("StarChannelId");

                    b.Property<int>("StarMinimum");

                    b.HasKey("GuildId");

                    b.ToTable("Guilds");
                });

            modelBuilder.Entity("SoraBot_v2.Data.Entities.ShareCentral", b =>
                {
                    b.Property<string>("ShareLink");

                    b.Property<ulong>("CreatorId");

                    b.Property<int>("Downvotes");

                    b.Property<bool>("IsPrivate");

                    b.Property<string>("Tags");

                    b.Property<string>("Titel");

                    b.Property<int>("Upvotes");

                    b.HasKey("ShareLink");

                    b.HasIndex("CreatorId");

                    b.ToTable("ShareCentrals");
                });

            modelBuilder.Entity("SoraBot_v2.Data.Entities.SubEntities.Afk", b =>
                {
                    b.Property<int>("AfkId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsAfk");

                    b.Property<string>("Message");

                    b.Property<DateTime>("TimeToTriggerAgain");

                    b.Property<ulong>("UserForeignId");

                    b.HasKey("AfkId");

                    b.HasIndex("UserForeignId")
                        .IsUnique();

                    b.ToTable("Afk");
                });

            modelBuilder.Entity("SoraBot_v2.Data.Entities.SubEntities.Interactions", b =>
                {
                    b.Property<int>("InteractionsId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("High5");

                    b.Property<int>("High5Given");

                    b.Property<int>("Hugs");

                    b.Property<int>("HugsGiven");

                    b.Property<int>("Kisses");

                    b.Property<int>("KissesGiven");

                    b.Property<int>("Pats");

                    b.Property<int>("PatsGiven");

                    b.Property<int>("Pokes");

                    b.Property<int>("PokesGiven");

                    b.Property<int>("Punches");

                    b.Property<int>("PunchesGiven");

                    b.Property<int>("Slaps");

                    b.Property<int>("SlapsGiven");

                    b.Property<ulong>("UserForeignId");

                    b.HasKey("InteractionsId");

                    b.HasIndex("UserForeignId")
                        .IsUnique();

                    b.ToTable("Interactions");
                });

            modelBuilder.Entity("SoraBot_v2.Data.Entities.SubEntities.Marriage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

                    b.Property<ulong>("PartnerId");

                    b.Property<DateTime>("Since");

                    b.Property<ulong>("UserForeignId");

                    b.HasKey("Id");

                    b.HasIndex("UserForeignId");

                    b.ToTable("Marriages");
                });

            modelBuilder.Entity("SoraBot_v2.Data.Entities.SubEntities.Reminders", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Message");

                    b.Property<DateTime>("Time");

                    b.Property<ulong>("UserForeignId");

                    b.HasKey("Id");

                    b.HasIndex("UserForeignId");

                    b.ToTable("Reminders");
                });

            modelBuilder.Entity("SoraBot_v2.Data.Entities.SubEntities.Song", b =>
                {
                    b.Property<string>("Base64EncodedLink");

                    b.Property<DateTime>("Added");

                    b.Property<string>("Name");

                    b.Property<ulong>("RequestorUserId");

                    b.HasKey("Base64EncodedLink");

                    b.ToTable("Songs");
                });

            modelBuilder.Entity("SoraBot_v2.Data.Entities.SubEntities.StarMessage", b =>
                {
                    b.Property<ulong>("MessageId");

                    b.Property<ulong>("GuildForeignId");

                    b.Property<byte>("HitZeroCount");

                    b.Property<bool>("IsPosted");

                    b.Property<ulong>("PostedMsgId");

                    b.Property<int>("StarCount");

                    b.HasKey("MessageId");

                    b.HasIndex("GuildForeignId");

                    b.ToTable("StarMessages");
                });

            modelBuilder.Entity("SoraBot_v2.Data.Entities.SubEntities.Tags", b =>
                {
                    b.Property<int>("TagId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AttachmentString");

                    b.Property<ulong>("CreatorId");

                    b.Property<bool>("ForceEmbed");

                    b.Property<ulong>("GuildForeignId");

                    b.Property<string>("Name");

                    b.Property<bool>("PictureAttachment");

                    b.Property<string>("Value");

                    b.HasKey("TagId");

                    b.HasIndex("GuildForeignId");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("SoraBot_v2.Data.Entities.SubEntities.Voting", b =>
                {
                    b.Property<int>("VoteId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ShareLink");

                    b.Property<bool>("UpOrDown");

                    b.Property<ulong>("VoterId");

                    b.HasKey("VoteId");

                    b.HasIndex("ShareLink");

                    b.HasIndex("VoterId");

                    b.ToTable("Votings");
                });

            modelBuilder.Entity("SoraBot_v2.Data.Entities.User", b =>
                {
                    b.Property<ulong>("UserId");

                    b.Property<DateTime>("CanGainAgain");

                    b.Property<float>("Exp");

                    b.Property<bool>("HasBg");

                    b.Property<bool>("Notified");

                    b.Property<DateTime>("ShowProfileCardAgain");

                    b.Property<DateTime>("UpdateBgAgain");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("SoraBot_v2.Data.Entities.ShareCentral", b =>
                {
                    b.HasOne("SoraBot_v2.Data.Entities.User", "User")
                        .WithMany("ShareCentrals")
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SoraBot_v2.Data.Entities.SubEntities.Afk", b =>
                {
                    b.HasOne("SoraBot_v2.Data.Entities.User", "User")
                        .WithOne("Afk")
                        .HasForeignKey("SoraBot_v2.Data.Entities.SubEntities.Afk", "UserForeignId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SoraBot_v2.Data.Entities.SubEntities.Interactions", b =>
                {
                    b.HasOne("SoraBot_v2.Data.Entities.User", "User")
                        .WithOne("Interactions")
                        .HasForeignKey("SoraBot_v2.Data.Entities.SubEntities.Interactions", "UserForeignId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SoraBot_v2.Data.Entities.SubEntities.Marriage", b =>
                {
                    b.HasOne("SoraBot_v2.Data.Entities.User", "User")
                        .WithMany("Marriages")
                        .HasForeignKey("UserForeignId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SoraBot_v2.Data.Entities.SubEntities.Reminders", b =>
                {
                    b.HasOne("SoraBot_v2.Data.Entities.User", "User")
                        .WithMany("Reminders")
                        .HasForeignKey("UserForeignId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SoraBot_v2.Data.Entities.SubEntities.StarMessage", b =>
                {
                    b.HasOne("SoraBot_v2.Data.Entities.Guild", "Guild")
                        .WithMany("StarMessages")
                        .HasForeignKey("GuildForeignId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SoraBot_v2.Data.Entities.SubEntities.Tags", b =>
                {
                    b.HasOne("SoraBot_v2.Data.Entities.Guild", "Guild")
                        .WithMany("Tags")
                        .HasForeignKey("GuildForeignId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SoraBot_v2.Data.Entities.SubEntities.Voting", b =>
                {
                    b.HasOne("SoraBot_v2.Data.Entities.ShareCentral", "ShareCentral")
                        .WithMany()
                        .HasForeignKey("ShareLink");

                    b.HasOne("SoraBot_v2.Data.Entities.User", "User")
                        .WithMany("Votings")
                        .HasForeignKey("VoterId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
