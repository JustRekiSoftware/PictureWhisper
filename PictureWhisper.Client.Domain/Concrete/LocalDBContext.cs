using Microsoft.EntityFrameworkCore;
using PictureWhisper.Client.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PictureWhisper.Client.Domain.Concrete
{
    public class LocalDBContext : DbContext
    {
        public string DbPath { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=" + DbPath);

        public DbSet<T_SigninInfo> SigninInfos { get; set; }

        public DbSet<T_RecommendInfo> RecommendInfos { get; set; }

        public DbSet<T_HistoryInfo> HistoryInfos { get; set; }

        public DbSet<T_SettingInfo> SettingInfos { get; set; }
    }
}
