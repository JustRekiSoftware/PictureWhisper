using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json.Linq;
using PictureWhisper.Domain.Entites;

namespace PictureWhisper.Domain.Abstract
{
    public interface IWallpaperRepository
    {
        Task<T_Wallpaper> QueryAsync(int id);

        Task<List<T_Wallpaper>> QueryAsync(string queryData, short filterData,
            string orderData, int page, int pageSize);

        Task<bool> InsertAsync(T_Wallpaper entity);

        Task<bool> DeleteAsync(int id);

        Task<List<T_Wallpaper>> GetTypeWallpaperAsync(short type, int page, int pageSize);

        Task<List<T_Wallpaper>> GetRecommendWallpaperAsync(int id, int count);

        Task<List<T_Wallpaper>> GetUnReviewedWallpaperAsync(int page, int pageSize);

        Task<List<T_Wallpaper>> GetPublishedWallpaperAsync(int id, int page, int pageSize);

        Task<List<T_Wallpaper>> GetFavoriteWallpaperAsync(int id, int page, int pageSize);

        Task<List<T_Wallpaper>> GetSpaceWallpaperAsync(int id, int page, int pageSize);

        Task<string> GetTodayWallpaperAsync();
    }
}
