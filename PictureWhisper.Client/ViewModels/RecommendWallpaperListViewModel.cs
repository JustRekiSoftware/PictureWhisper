using Newtonsoft.Json.Linq;
using PictureWhisper.Client.Helper;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace PictureWhisper.Client.ViewModels
{
    /// <summary>
    /// 推荐壁纸的ViewModel
    /// </summary>
    public class RecommendWallpaperListViewModel
    {
        public ObservableCollection<WallpaperDto> RecommendWallpapers { get; set; }
        private int UserId { get; set; }

        public RecommendWallpaperListViewModel()
        {
            RecommendWallpapers = new ObservableCollection<WallpaperDto>();
            UserId = SQLiteHelper.GetSigninInfo().SI_UserID;
        }

        /// <summary>
        /// 获取推荐壁纸
        /// </summary>
        /// <param name="limit">获取数量</param>
        /// <returns></returns>
        public async Task GetRecommendWallpapersAsync(int limit)
        {
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                //获取推荐壁纸
                var url = string.Format("{0}wallpaper/recommend/{1}/{2}",
                    HttpClientHelper.baseUrl, UserId, limit);
                var response = await client.GetAsync(new Uri(url));
                if (!response.IsSuccessStatusCode)
                {
                    return;
                }
                var wallpapers = JArray.Parse(
                    await response.Content.ReadAsStringAsync());
                var result = wallpapers.ToObject<List<T_Wallpaper>>();
                if (result == null)
                {
                    return;
                }
                //补充显示信息
                foreach (var wallpaper in result)
                {
                    //if (SQLiteHelper.IsWallpaperHistory(wallpaper.W_ID))
                    //{
                    //    continue;
                    //}
                    url = HttpClientHelper.baseUrl
                        + "download/picture/small/" + wallpaper.W_Location;
                    var image = await ImageHelper.GetImageAsync(client, url);
                    var wallpaperDto = new WallpaperDto
                    {
                        WallpaperInfo = wallpaper,
                        Image = image
                    };
                    if (RecommendWallpapers.Where(p => p.WallpaperInfo.W_ID 
                        == wallpaperDto.WallpaperInfo.W_ID).Count() > 0)
                    {
                        continue;
                    }
                    this.RecommendWallpapers.Add(wallpaperDto);
                }
            }
        }
    }
}
