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
    public class RecommendWallpaperListViewModel : ISupportIncrementalLoading
    {
        public ObservableCollection<WallpaperDto> RecommendWallpapers { get; set; }
        private int UserId { get; set; }
        private int TotalCount { get; set; }
        private bool IsBusy { get; set; }

        public bool HasMoreItems { get; set; }

        public RecommendWallpaperListViewModel()
        {
            RecommendWallpapers = new ObservableCollection<WallpaperDto>();
            UserId = SQLiteHelper.GetSigninInfo().SI_UserID;
            IsBusy = false;
        }

        public async Task<int> GetRecommendWallpapersAsync(int id, uint limit)
        {
            var count = 0;
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var url = string.Format("{0}wallpaper/recommend/{1}/{2}",
                    HttpClientHelper.baseUrl, id, limit);
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
                if (result.Count < TotalCount + limit)
                {
                    HasMoreItems = false;
                }
                else
                {
                    HasMoreItems = true;
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
                    count++;
                }
                TotalCount += count;
            }

            return count;
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            if (IsBusy)
            {
                return null;
            }
            IsBusy = true;
            return GetMoreItemsAsync(count).AsAsyncOperation();
        }

        public async Task<LoadMoreItemsResult> GetMoreItemsAsync(uint count)
        {
            var result = new LoadMoreItemsResult();
            result.Count = (uint)await GetRecommendWallpapersAsync(UserId, count);
            while (result.Count < count)
            {
                if (HasMoreItems)
                {
                    result.Count += (uint)await GetRecommendWallpapersAsync(UserId, count);
                }
                else
                {
                    break;
                }
            }
            IsBusy = false;
            return result;
        }

        public async Task Refresh()
        {
            HasMoreItems = true;
            IsBusy = false;
            TotalCount = 0;
            RecommendWallpapers.Clear();
            await LoadMoreItemsAsync(20);
        }
    }
}
