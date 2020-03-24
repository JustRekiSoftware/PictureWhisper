﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PictureWhisper.Client.Domain.Concrete;

namespace PictureWhisper.Client.Domain.Migrations
{
    [DbContext(typeof(LocalDBContext))]
    partial class LocalDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.2");

            modelBuilder.Entity("PictureWhisper.Client.Domain.Entities.T_HistoryInfo", b =>
                {
                    b.Property<int>("HI_ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("HI_WallpaperID")
                        .HasColumnType("INTEGER");

                    b.HasKey("HI_ID");

                    b.ToTable("T_HistoryInfo");
                });

            modelBuilder.Entity("PictureWhisper.Client.Domain.Entities.T_RecommendInfo", b =>
                {
                    b.Property<int>("RI_ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("RI_Num")
                        .HasColumnType("INTEGER");

                    b.HasKey("RI_ID");

                    b.ToTable("T_RecommendInfo");
                });

            modelBuilder.Entity("PictureWhisper.Client.Domain.Entities.T_SettingInfo", b =>
                {
                    b.Property<int>("SI_ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("SI_AutoSetWallpaper")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SI_WallpaperSavePath")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("SI_ID");

                    b.ToTable("T_SettingInfo");
                });

            modelBuilder.Entity("PictureWhisper.Client.Domain.Entities.T_SigninInfo", b =>
                {
                    b.Property<int>("SI_ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("SI_Avatar")
                        .HasColumnType("TEXT")
                        .HasMaxLength(128);

                    b.Property<string>("SI_Email")
                        .HasColumnType("TEXT")
                        .HasMaxLength(32);

                    b.Property<string>("SI_Password")
                        .HasColumnType("TEXT")
                        .HasMaxLength(64);

                    b.Property<short>("SI_Status")
                        .HasColumnType("INTEGER");

                    b.Property<short>("SI_Type")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SI_UserID")
                        .HasColumnType("INTEGER");

                    b.HasKey("SI_ID");

                    b.ToTable("T_SigninInfo");
                });
#pragma warning restore 612, 618
        }
    }
}
