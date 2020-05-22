using Microsoft.EntityFrameworkCore;
using PictureWhisper.Client.Domain.Concrete;
using PictureWhisper.Client.Domain.Entities;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace PictureWhisper.Client.Helper
{
    /// <summary>
    /// SQLite帮助类
    /// </summary>
    public class SQLiteHelper
    {
        public static readonly string DbPath =
            Path.Combine(ApplicationData.Current.LocalFolder.Path, "local.db");//SQLite文件路径

        /// <summary>
        /// 获取登录信息
        /// </summary>
        /// <returns>获取成功返回登录信息，否则返回null</returns>
        public static T_SigninInfo GetSigninInfo()
        {
            using (var db = new LocalDBContext())
            {
                db.DbPath = DbPath;
                try
                {
                    return db.SigninInfos.FirstOrDefault();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 添加登录信息
        /// </summary>
        /// <param name="signinInfo">登录信息</param>
        /// <returns></returns>
        public async static Task AddSigninInfoAsync(T_SigninInfo signinInfo)
        {
            using (var db = new LocalDBContext())
            {
                db.DbPath = DbPath;
                db.SigninInfos.Add(signinInfo);
                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {

                }
            }
        }

        /// <summary>
        /// 删除登录信息
        /// </summary>
        /// <param name="signinInfo">登录信息</param>
        /// <returns></returns>
        public async static Task RemoveSigninInfoAsync(T_SigninInfo signinInfo)
        {
            using (var db = new LocalDBContext())
            {
                db.DbPath = DbPath;
                db.SigninInfos.Remove(signinInfo);
                db.HistoryInfos.RemoveRange(db.HistoryInfos);
                db.SettingInfos.RemoveRange(db.SettingInfos);
                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {

                }
            }
        }

        /// <summary>
        /// 更新登录信息
        /// </summary>
        /// <param name="signinInfo">登录信息</param>
        /// <returns></returns>
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

        /// <summary>
        /// 获取设置信息
        /// </summary>
        /// <returns>获取成功返回设置信息，否则返回null</returns>
        public static T_SettingInfo GetSettingInfo()
        {
            using (var db = new LocalDBContext())
            {
                db.DbPath = DbPath;
                try
                {
                    return db.SettingInfos.FirstOrDefault();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 添加设置信息
        /// </summary>
        /// <param name="settingInfo">设置信息</param>
        /// <returns></returns>
        public async static Task AddSettingInfoAsync(T_SettingInfo settingInfo)
        {
            using (var db = new LocalDBContext())
            {
                db.DbPath = DbPath;
                db.SettingInfos.Add(settingInfo);
                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {

                }
            }
        }

        /// <summary>
        /// 更新设置信息
        /// </summary>
        /// <param name="settingInfo">设置信息</param>
        /// <returns></returns>
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

        /// <summary>
        /// 获取壁纸是否已浏览
        /// </summary>
        /// <param name="id">壁纸Id</param>
        /// <returns>已浏览返回true，否则返回false</returns>
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

        /// <summary>
        /// 添加浏览记录
        /// </summary>
        /// <param name="historyInfo">浏览记录</param>
        /// <returns></returns>
        public async static Task AddHistoryInfoAsync(T_HistoryInfo historyInfo)
        {
            using (var db = new LocalDBContext())
            {
                db.DbPath = DbPath;
                db.HistoryInfos.Add(historyInfo);
                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {

                }
            }
        }

        /// <summary>
        /// 删除浏览记录
        /// </summary>
        /// <param name="historyInfo">浏览记录</param>
        /// <returns></returns>
        public async static Task RemoveHistoryInfoAsync(T_HistoryInfo historyInfo)
        {
            using (var db = new LocalDBContext())
            {
                db.DbPath = DbPath;
                db.HistoryInfos.Remove(historyInfo);
                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {

                }
            }
        }
    }
}
