using Microsoft.EntityFrameworkCore;
using PictureWhisper.Domain.Entites;
using System.Threading;
using System.Threading.Tasks;

namespace PictureWhisper.Domain.Concrete
{
    /// <summary>
    /// DB_PictureWhisper数据库上下文
    /// </summary>
    public class DB_PictureWhisperContext : DbContext
    {
        public DB_PictureWhisperContext(DbContextOptions<DB_PictureWhisperContext> options)
            : base(options)
        {
        }

        public DbSet<T_Wallpaper> Wallpapers { get; set; }//壁纸表

        public DbSet<T_WallpaperType> WallpaperTypes { get; set; }//壁纸分区表

        public DbSet<T_User> Users { get; set; }//用户表

        public DbSet<T_Comment> Comments { get; set; }//评论表

        public DbSet<T_Reply> Replies { get; set; }//回复表

        public DbSet<T_Follow> Follows { get; set; }//关注表

        public DbSet<T_Like> Likes { get; set; }//点赞表

        public DbSet<T_Favorite> Favorites { get; set; }//收藏表

        public DbSet<T_Report> Reports { get; set; }//举报表

        public DbSet<T_ReportReason> ReportReasons { get; set; }//举报理由表

        public DbSet<T_Review> Reviews { get; set; }//审核表
    }
}
