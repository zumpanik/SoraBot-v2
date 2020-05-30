﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SoraBot.Data;

namespace SoraBot.Data.Migrations
{
    [DbContext(typeof(SoraContext))]
    [Migration("20200524110046_WaifuRequestNotify")]
    partial class WaifuRequestNotify
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("SoraBot.Data.Models.SoraDb.Guild", b =>
                {
                    b.Property<ulong>("Id")
                        .HasColumnType("bigint unsigned");

                    b.Property<string>("Prefix")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("Guilds");
                });

            modelBuilder.Entity("SoraBot.Data.Models.SoraDb.GuildUser", b =>
                {
                    b.Property<ulong>("GuildId")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("UserId")
                        .HasColumnType("bigint unsigned");

                    b.Property<uint>("Exp")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int unsigned")
                        .HasDefaultValue(0u);

                    b.HasKey("GuildId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("GuildUsers");
                });

            modelBuilder.Entity("SoraBot.Data.Models.SoraDb.Reminder", b =>
                {
                    b.Property<uint>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int unsigned");

                    b.Property<DateTime>("DueDateUtc")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<ulong>("UserId")
                        .HasColumnType("bigint unsigned");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Reminders");
                });

            modelBuilder.Entity("SoraBot.Data.Models.SoraDb.Sar", b =>
                {
                    b.Property<ulong>("RoleId")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("GuildId")
                        .HasColumnType("bigint unsigned");

                    b.HasKey("RoleId");

                    b.HasIndex("GuildId");

                    b.ToTable("Sars");
                });

            modelBuilder.Entity("SoraBot.Data.Models.SoraDb.Starboard", b =>
                {
                    b.Property<ulong>("GuildId")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("StarboardChannelId")
                        .HasColumnType("bigint unsigned");

                    b.Property<uint>("StarboardThreshold")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int unsigned")
                        .HasDefaultValue(1u);

                    b.HasKey("GuildId");

                    b.ToTable("Starboards");
                });

            modelBuilder.Entity("SoraBot.Data.Models.SoraDb.StarboardMessage", b =>
                {
                    b.Property<ulong>("MessageId")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("GuildId")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("PostedMsgId")
                        .HasColumnType("bigint unsigned");

                    b.HasKey("MessageId");

                    b.HasIndex("GuildId");

                    b.ToTable("StarboardMessages");
                });

            modelBuilder.Entity("SoraBot.Data.Models.SoraDb.User", b =>
                {
                    b.Property<ulong>("Id")
                        .HasColumnType("bigint unsigned");

                    b.Property<uint>("Coins")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("Exp")
                        .HasColumnType("int unsigned");

                    b.Property<int?>("FavoriteWaifuId")
                        .HasColumnType("int");

                    b.Property<bool>("HasCustomProfileBg")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("LastDaily")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("FavoriteWaifuId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("SoraBot.Data.Models.SoraDb.UserNotifiedOnRequestProcess", b =>
                {
                    b.Property<ulong>("UserId")
                        .HasColumnType("bigint unsigned");

                    b.HasKey("UserId");

                    b.ToTable("UserNotifiedOnRequestProcesses");
                });

            modelBuilder.Entity("SoraBot.Data.Models.SoraDb.UserWaifu", b =>
                {
                    b.Property<ulong>("UserId")
                        .HasColumnType("bigint unsigned");

                    b.Property<int>("WaifuId")
                        .HasColumnType("int");

                    b.Property<uint>("Count")
                        .HasColumnType("int unsigned");

                    b.HasKey("UserId", "WaifuId");

                    b.HasIndex("WaifuId");

                    b.ToTable("UserWaifus");
                });

            modelBuilder.Entity("SoraBot.Data.Models.SoraDb.Waifu", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("Rarity")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Waifus");
                });

            modelBuilder.Entity("SoraBot.Data.Models.SoraDb.WaifuRequest", b =>
                {
                    b.Property<uint>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int unsigned");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime?>("ProcessedTime")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Rarity")
                        .HasColumnType("int");

                    b.Property<int>("RequestState")
                        .HasColumnType("int");

                    b.Property<DateTime>("RequestTime")
                        .HasColumnType("datetime(6)");

                    b.Property<ulong>("UserId")
                        .HasColumnType("bigint unsigned");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("WaifuRequests");
                });

            modelBuilder.Entity("SoraBot.Data.Models.SoraDb.GuildUser", b =>
                {
                    b.HasOne("SoraBot.Data.Models.SoraDb.Guild", "Guild")
                        .WithMany("GuildUsers")
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SoraBot.Data.Models.SoraDb.User", "User")
                        .WithMany("GuildUsers")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SoraBot.Data.Models.SoraDb.Reminder", b =>
                {
                    b.HasOne("SoraBot.Data.Models.SoraDb.User", "User")
                        .WithMany("Reminders")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SoraBot.Data.Models.SoraDb.Sar", b =>
                {
                    b.HasOne("SoraBot.Data.Models.SoraDb.Guild", "Guild")
                        .WithMany("Sars")
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SoraBot.Data.Models.SoraDb.Starboard", b =>
                {
                    b.HasOne("SoraBot.Data.Models.SoraDb.Guild", "Guild")
                        .WithOne("Starboard")
                        .HasForeignKey("SoraBot.Data.Models.SoraDb.Starboard", "GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SoraBot.Data.Models.SoraDb.StarboardMessage", b =>
                {
                    b.HasOne("SoraBot.Data.Models.SoraDb.Guild", "Guild")
                        .WithMany("StarboardMessages")
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SoraBot.Data.Models.SoraDb.User", b =>
                {
                    b.HasOne("SoraBot.Data.Models.SoraDb.Waifu", "FavoriteWaifu")
                        .WithMany("UsersFavorite")
                        .HasForeignKey("FavoriteWaifuId")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("SoraBot.Data.Models.SoraDb.UserWaifu", b =>
                {
                    b.HasOne("SoraBot.Data.Models.SoraDb.User", "Owner")
                        .WithMany("UserWaifus")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SoraBot.Data.Models.SoraDb.Waifu", "Waifu")
                        .WithMany("UserWaifus")
                        .HasForeignKey("WaifuId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SoraBot.Data.Models.SoraDb.WaifuRequest", b =>
                {
                    b.HasOne("SoraBot.Data.Models.SoraDb.User", "User")
                        .WithMany("WaifuRequests")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
