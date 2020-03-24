using Microsoft.EntityFrameworkCore;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace PictureWhisper.Domain.Concrete
{
    public class DB_PictureWhisperContext : DbContext
    {
        public DB_PictureWhisperContext(DbContextOptions<DB_PictureWhisperContext> options)
            : base(options)
        {
        }

        public DbSet<T_Wallpaper> Wallpapers { get; set; }

        public DbSet<T_WallpaperType> WallpaperTypes { get; set; }

        public DbSet<T_User> Users { get; set; }

        public DbSet<T_Comment> Comments { get; set; }

        public DbSet<T_Reply> Replies { get; set; }

        public DbSet<T_Follow> Follows { get; set; }

        public DbSet<T_Like> Likes { get; set; }

        public DbSet<T_Favorite> Favorites { get; set; }

        public DbSet<T_Report> Reports { get; set; }

        public DbSet<T_ReportReason> ReportReasons { get; set; }

        public DbSet<T_Review> Reviews { get; set; }
    }
}
