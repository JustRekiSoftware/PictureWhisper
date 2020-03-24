using Newtonsoft.Json.Linq;
using PictureWhisper.Client.Helpers;
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
    public class RecommendWallpaperListViewModel : ISupportIncrementalLoading
    {
        public ObservableCollection<WallpaperDto> RecommendWallpapers { get; set; }
        private int UserId { get; set; }
        public readonly int PageSize = 20;
        public int PageNum { get; set; }

        public bool HasMoreItems { get; set; }

        public RecommendWallpaperListViewModel()
        {
            RecommendWallpapers = new ObservableCollection<WallpaperDto>();
            UserId = SQLiteHelper.GetSigninInfo().SI_UserID;
            PageNum = 1;
        }

        public async Task<int> GetRecommendWallpapersAsync(int id)
        {
            var count = 0;
            if (PageNum == 1)
            {
                RecommendWallpapers.Clear();
            }
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var url = string.Format("{0}wallpaper/recommend/{1}/{2}/{3}",
                    HttpClientHelper.baseUrl, id, PageNum, PageSize);
                var response = await client.GetAsync(new Uri(url));
                if (!response.IsSuccessStatusCode)
                {
                    return count;
                }
                var wallpapers = JArray.Parse(
                    await response.Content.ReadAsStringAsync());
                var result = wallpapers.ToObject<List<T_Wallpaper>>();
                if (result == null)
                {
                    HasMoreItems = false;
                    return count;
                }
                if (result.Count < PageSize)
                {
                    HasMoreItems = false;
                }
                else
                {
                    HasMoreItems = true;
                }
                foreach (var wallpaper in result)
                {
                    if (SQLiteHelper.IsWallpaperHistory(wallpaper.W_ID))
                    {
                        continue;
                    }
                    url = HttpClientHelper.baseUrl
                        + "download/picture/small/" + wallpaper.W_Location;
                    var image = await ImageHelper.GetImageAsync(client, url);
                    this.RecommendWallpapers.Add(new WallpaperDto
                    {
                        WallpaperInfo = wallpaper,
                        Image = image
                    });
                    count++;
                }
            }
            PageNum++;

            return count;
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return GetMoreItemsAsync(count).AsAsyncOperation();
        }

        public async Task<LoadMoreItemsResult> GetMoreItemsAsync(uint count)
        {
            var result = new LoadMoreItemsResult();
            result.Count = (uint)await GetRecommendWallpapersAsync(UserId);
            while (result.Count < count)
            {
                result.Count += (uint)await GetRecommendWallpapersAsync(UserId);
            }

            return result;
        }
    }
}
