using Org.BouncyCastle.Asn1.X509.Qualified;
using PictureWhisper.Domain.Entites;
using System.Collections.Generic;
using System.Linq;

namespace PictureWhisper.Domain.Helper
{
    /// <summary>
    /// 审核帮助类
    /// </summary>
    public class ReviewHelper
    {
        public static object syncRoot = new object();//同步锁
        public static List<T_Report> Reports { get; set; } = new List<T_Report>();
        public static List<T_Wallpaper> Wallpapers { get; set; } = new List<T_Wallpaper>();
        public static Dictionary<int, List<T_Report>> UserReports { get; set; } = new Dictionary<int, List<T_Report>>();
        public static Dictionary<int, List<T_Wallpaper>> UserWallpapers { get; set; } = new Dictionary<int, List<T_Wallpaper>>();

        /// <summary>
        /// 添加要处理的举报
        /// </summary>
        /// <param name="result">要返回的举报处理列表</param>
        /// <param name="entity">举报信息</param>
        /// <param name="userId">举报处理人员Id</param>
        public static void AddReport(ref List<T_Report> result, T_Report entity, int userId)
        {
            lock (syncRoot)
            {
                if (Reports.Count(p => p.RPT_ID == entity.RPT_ID) > 0)
                {
                    return;
                }
                if (!UserReports.ContainsKey(userId))
                {
                    UserReports.Add(userId, new List<T_Report>());
                }
                Reports.Add(entity);
                UserReports[userId].Add(entity);
                result.Add(entity);
            }
        }

        /// <summary>
        /// 添加要审核的壁纸
        /// </summary>
        /// <param name="result">要返回的壁纸列表</param>
        /// <param name="entity">壁纸信息</param>
        /// <param name="userId">壁纸审核人员Id</param>
        public static void AddWallpaper(ref List<T_Wallpaper> result, T_Wallpaper entity, int userId)
        {
            lock (syncRoot)
            {
                if (Wallpapers.Count(p => p.W_ID == entity.W_ID) > 0)
                {
                    return;
                }
                if (!UserWallpapers.ContainsKey(userId))
                {
                    UserWallpapers.Add(userId, new List<T_Wallpaper>());
                }
                Wallpapers.Add(entity);
                UserWallpapers[userId].Add(entity);
                result.Add(entity);
            }
        }

        /// <summary>
        /// 移除已处理的举报
        /// </summary>
        /// <param name="entity">举报信息</param>
        /// <param name="userId">举报处理人员Id</param>
        public static void RemoveReport(T_Report entity, int userId)
        {
            lock (syncRoot)
            {
                if (UserReports[userId].Contains(entity))
                {
                    UserReports[userId].Remove(entity);
                    Reports.Remove(entity);
                }
            }
        }

        /// <summary>
        /// 移除已处理的壁纸
        /// </summary>
        /// <param name="entity">壁纸信息</param>
        /// <param name="userId">壁纸审核人员Id</param>
        public static void RemoveWallpaper(T_Wallpaper entity, int userId)
        {
            lock (syncRoot)
            {
                if (UserWallpapers[userId].Contains(entity))
                {
                    UserWallpapers[userId].Remove(entity);
                    Wallpapers.Remove(entity);
                }
            }
        }

        /// <summary>
        /// 移除某用户进行的所有举报处理或审核
        /// </summary>
        /// <param name="userId">用户Id</param>
        public static void RemoveAllByUserId(int userId)
        {
            lock (syncRoot)
            {
                if (UserReports.ContainsKey(userId))
                {
                    foreach (var report in UserReports[userId])
                    {
                        Reports.Remove(report);
                    }
                    UserReports.Remove(userId);
                }
                if (UserWallpapers.ContainsKey(userId))
                {
                    foreach (var wallpaper in UserWallpapers[userId])
                    {
                        Wallpapers.Remove(wallpaper);
                    }
                    UserWallpapers.Remove(userId);
                }
            }
        }
    }
}
