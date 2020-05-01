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
    public class RecommendWallpaperListViewModel
    {
        public ObservableCollection<WallpaperDto> RecommendWallpapers { get; set; }
        private int UserId { get; set; }

        public RecommendWallpaperListViewModel()
        {
            RecommendWallpapers = new ObservableCollection<WallpaperDto>();
            UserId = SQLiteHelper.GetSigninInfo().SI_UserID;
        }

        public async Task GetRecommendWallpapersAsync(int limit)
        {
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
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
                foreach (var wallpaper in result)
                {
                    //if (SQLiteHelper.IsWallpaperHistory(wallpaper.W_ID))
                    //{
                    //    continue;
                    //}
                    url = HttpClientHelper.baseUrl
                        + "download/picture/small/" + wallpaper.W_Location;
                    var image = await ImageHelper.GetImageAsync(client, url);
                    this.RecommendWallpapers.Add(new WallpaperDto
                    {
                        WallpaperInfo = wallpaper,
                        Image = image
                    });
                }
            }
        }
    }
}
