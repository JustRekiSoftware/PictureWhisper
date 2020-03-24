﻿using Microsoft.EntityFrameworkCore;
using PictureWhisper.Client.Domain.Concrete;
using PictureWhisper.Client.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace PictureWhisper.Client.BackgroundTask.Helpers
{
    public class SQLiteHelper
    {
        public static readonly string DbPath =
            Path.Combine(ApplicationData.Current.LocalFolder.Path, "local.db");

        public static T_SigninInfo GetSigninInfo()
        {
            using (var db = new LocalDBContext())
            {
                db.DbPath = DbPath;
                return db.SigninInfos.FirstOrDefault();
            }
        }

        public async static Task AddSigninInfoAsync(T_SigninInfo signinInfo)
        {
            using (var db = new LocalDBContext())
            {
                db.DbPath = DbPath;
                db.SigninInfos.Add(signinInfo);
                await db.SaveChangesAsync();
            }
        }

        public async static Task RemoveSigninInfoAsync(T_SigninInfo signinInfo)
        {
            using (var db = new LocalDBContext())
            {
                db.DbPath = DbPath;
                db.SigninInfos.Remove(signinInfo);
                await db.SaveChangesAsync();
            }
        }

        public async static Task UpdateSigninInfoAsync(T_SigninInfo signinInfo)
        {
            using (var db = new LocalDBContext())
            {
                db.DbPath = DbPath;
                db.Entry(signinInfo).State = EntityState.Modified;
                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {

                }
            }
        }

        public static T_SettingInfo GetSettingInfo()
        {
            using (var db = new LocalDBContext())
            {
                db.DbPath = DbPath;
                return db.SettingInfos.FirstOrDefault();
            }
        }

        public async static Task AddSettingInfoAsync(T_SettingInfo settingInfo)
        {
            using (var db = new LocalDBContext())
            {
                db.DbPath = DbPath;
                db.SettingInfos.Add(settingInfo);
                await db.SaveChangesAsync();
            }
        }

        public async static Task UpdateSettingInfoAsync(T_SettingInfo settingInfo)
        {
            using (var db = new LocalDBContext())
            {
                db.DbPath = DbPath;
                db.Entry(settingInfo).State = EntityState.Modified;
                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {

                }
            }
        }

        public static T_RecommendInfo GetRecommendInfo()
        {
            using (var db = new LocalDBContext())
            {
                db.DbPath = DbPath;
                return db.RecommendInfos.FirstOrDefault();
            }
        }

        public async static Task AddRecommendInfoAsync(T_RecommendInfo recommendInfo)
        {
            using (var db = new LocalDBContext())
            {
                db.DbPath = DbPath;
                db.RecommendInfos.Add(recommendInfo);
                await db.SaveChangesAsync();
            }
        }

        public async static Task UpdateRecommendInfoAsync(T_RecommendInfo recommendInfo)
        {
            using (var db = new LocalDBContext())
            {
                db.DbPath = DbPath;
                db.Entry(recommendInfo).State = EntityState.Modified;
                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {

                }
            }
        }

        public static bool IsWallpaperHistory(int id)
        {
            using (var db = new LocalDBContext())
            {
                db.DbPath = DbPath;
                var result = db.HistoryInfos.Find(id);
                if (result == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public async static Task AddHistoryInfoAsync(T_HistoryInfo historyInfo)
        {
            using (var db = new LocalDBContext())
            {
                db.DbPath = DbPath;
                db.HistoryInfos.Add(historyInfo);
                await db.SaveChangesAsync();
            }
        }

        public async static Task UpdateHistoryInfoAsync(T_HistoryInfo historyInfo)
        {
            using (var db = new LocalDBContext())
            {
                db.DbPath = DbPath;
                db.Entry(historyInfo).State = EntityState.Modified;
                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {

                }
            }
        }
    }
}