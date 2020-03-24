using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PictureWhisper.Domain.Abstract;
using PictureWhisper.Domain.Concrete;

namespace PictureWhisper.WebAPI.DI
{
    public class DIIoc
    {
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
