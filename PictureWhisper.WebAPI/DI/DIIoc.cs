using Microsoft.Extensions.DependencyInjection;
using PictureWhisper.Domain.Abstract;
using PictureWhisper.Domain.Concrete;

namespace PictureWhisper.WebAPI.DI
{
    /// <summary>
    /// 数据仓库依赖注入
    /// </summary>
    public class DIIoc
    {
        /// <summary>
        /// 执行依赖注入，将数据仓库注入控制器
        /// </summary>
        /// <param name="services">依赖注入服务</param>
        public static void Injection(IServiceCollection services)
        {
            //Transient：每一次调用服务都会创建一个新的实例
            //Scoped：每一次请求只创建一个实例
            //Singleton：单例，在整个应用程序生命周期以内只创建一个实例

            services.AddScoped<IWallpaperRepository, WallpaperRepository>();

            services.AddScoped<IWallpaperTypeRepository, WallpaperTypeRepository>();

            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<ICommentRepository, CommentRepository>();

            services.AddScoped<IReplyRepository, ReplyRepository>();

            services.AddScoped<IFollowRepository, FollowRepository>();

            services.AddScoped<ILikeRepository, LikeRepository>();

            services.AddScoped<IFavoriteRepository, FavoriteRepository>();

            services.AddScoped<IReportRepository, ReportRepository>();

            services.AddScoped<IReportReasonRepository, ReportReasonRepository>();

            services.AddScoped<IReviewRepository, ReviewRepository>();
        }
    }
}
