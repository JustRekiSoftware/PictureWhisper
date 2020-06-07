using PictureWhisper.Domain.Entites;
using System.Collections.Generic;

namespace PictureWhisper.Domain.Helper
{
    /// <summary>
    /// 审核帮助类
    /// </summary>
    public class ReviewHelper
    {
        public static object syncRoot = new object();//同步锁
        public static HashSet<T_Report> Reports { get; set; } = new HashSet<T_Report>();
        public static HashSet<T_Wallpaper> Wallpapers { get; set; } = new HashSet<T_Wallpaper>();

        /// <summary>
        /// 添加要处理的举报
        /// </summary>
        /// <param name="result">要返回的举报处理列表</param>
        /// <param name="entity">举报信息</param>
        public static void AddReport(ref List<T_Report> result, T_Report entity)
        {
            lock (syncRoot)
            {
                if (Reports.Contains(entity))
                {
                    return;
                }
                Reports.Add(entity);
                result.Add(entity);
            }
        }

        /// <summary>
        /// 添加要审核的壁纸
        /// </summary>
        /// <param name="result">要返回的壁纸列表</param>
        /// <param name="entity">壁纸信息</param>
        public static void AddWallpaper(ref List<T_Wallpaper> result, T_Wallpaper entity)
        {
            lock (syncRoot)
            {
                if (Wallpapers.Contains(entity))
                {
                    return;
                }
                Wallpapers.Add(entity);
                result.Add(entity);
            }
        }

        /// <summary>
        /// 移除已处理的举报
        /// </summary>
        /// <param name="entity">举报信息</param>
        public static void RemoveReport(T_Report entity)
        {
            lock (syncRoot)
            {
                if (Reports.Contains(entity))
                {
                    Reports.Remove(entity);
                }
            }
        }

        /// <summary>
        /// 移除已处理的壁纸
        /// </summary>
        /// <param name="entity">壁纸信息</param>
        public static void RemoveWallpaper(T_Wallpaper entity)
        {
            lock (syncRoot)
            {
                if (Wallpapers.Contains(entity))
                {
                    Wallpapers.Remove(entity);
                }
            }
        }
    }
}
