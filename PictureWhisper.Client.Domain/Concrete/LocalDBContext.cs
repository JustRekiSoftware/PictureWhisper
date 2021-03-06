﻿using Microsoft.EntityFrameworkCore;
using PictureWhisper.Client.Domain.Entities;

namespace PictureWhisper.Client.Domain.Concrete
{
    /// <summary>
    /// LocalDB数据库上下文
    /// </summary>
    public class LocalDBContext : DbContext
    {
        public string DbPath { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=" + DbPath);

        public DbSet<T_SigninInfo> SigninInfos { get; set; }

        public DbSet<T_HistoryInfo> HistoryInfos { get; set; }

        public DbSet<T_SettingInfo> SettingInfos { get; set; }
    }
}
