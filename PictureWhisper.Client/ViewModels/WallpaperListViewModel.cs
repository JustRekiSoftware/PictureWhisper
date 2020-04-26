using Newtonsoft.Json.Linq;
using PictureWhisper.Client.Helper;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureWhisper.Client.ViewModels
{
    public class WallpaperListViewModel
    {
        public ObservableCollection<WallpaperDto> SearchResultWallpapers { get; set; }

        public ObservableCollection<WallpaperDto> TypeResultWallpapers { get; set; }

        public ObservableCollection<WallpaperDto> ReviewWallpapers { get; set; }

        public ObservableCollection<WallpaperDto> PublishedWallpapers { get; set; }

        public ObservableCollection<WallpaperDto> FavoriteWallpapers { get; set; }

        public ObservableCollection<WallpaperDto> SpaceWallpapers { get; set; }

        public WallpaperListViewModel()
        {
            SearchResultWallpapers = new ObservableCollection<WallpaperDto>();
            TypeResultWallpapers = new ObservableCollection<WallpaperDto>();
            ReviewWallpapers = new ObservableCollection<WallpaperDto>();
            PublishedWallpapers = new ObservableCollection<WallpaperDto>();
            FavoriteWallpapers = new ObservableCollection<WallpaperDto>();
            SpaceWallpapers = new ObservableCollection<WallpaperDto>();
        }

        public async Task GetSearchResultWallpapersAsync(string queryData,
            short filterData, string orderData, int page, int pageSize)
        {
            if (page == 1)
            {
                SearchResultWallpapers.Clear();
            }
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var url = string.Format("{0}wallpaper/query/{1}/{2}/{3}/{4}/{5}",
                    HttpClientHelper.baseUrl, queryData, filterData, orderData, page, pageSize);
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
                    url = HttpClientHelper.baseUrl 
                        + "download/picture/small/" + wallpaper.W_Location;
                    var image = await ImageHelper.GetImageAsync(client, url);
                    this.SearchResultWallpapers.Add(new WallpaperDto
                    {
                        WallpaperInfo = wallpaper,
                        Image = image
                    });
                }
            }
        }

        public async Task GetTypeResultWallpapersAsync(short type, int page, int pageSize)
        {
            if (page == 1)
            {
                TypeResultWallpapers.Clear();
            }
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var url = string.Format("{0}wallpaper/type/{1}/{2}/{3}",
                    HttpClientHelper.baseUrl, type, page, pageSize);
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
                    url = HttpClientHelper.baseUrl
                        + "download/picture/small/" + wallpaper.W_Location;
                    var image = await ImageHelper.GetImageAsync(client, url);
                    this.TypeResultWallpapers.Add(new WallpaperDto
                    {
                        WallpaperInfo = wallpaper,
                        Image = image
                    });
                }
            }
        }

        public async Task GetReviewWallpapersAsync(int page, int pageSize)
        {
            if (page == 1)
            {
                ReviewWallpapers.Clear();
            }
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var url = string.Format("{0}wallpaper/review/{1}/{2}",
                    HttpClientHelper.baseUrl, page, pageSize);
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
                    url = HttpClientHelper.baseUrl
                        + "download/picture/origin/" + wallpaper.W_Location;
                    var image = await ImageHelper.GetImageAsync(client, url);
                    this.ReviewWallpapers.Add(new WallpaperDto
                    {
                        WallpaperInfo = wallpaper,
                        Image = image
                    });
                }
            }
        }

        public async Task GetPublishedWallpapersAsync(int id, int page, int pageSize)
        {
            if (page == 1)
            {
                PublishedWallpapers.Clear();
            }
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var url = string.Format("{0}wallpaper/published/{1}/{2}/{3}",
                    HttpClientHelper.baseUrl, id, page, pageSize);
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
                    url = HttpClientHelper.baseUrl
                        + "download/picture/small/" + wallpaper.W_Location;
                    var image = await ImageHelper.GetImageAsync(client, url);
                    this.PublishedWallpapers.Add(new WallpaperDto
                    {
                        WallpaperInfo = wallpaper,
                        Image = image
                    });
                }
            }
        }

        public async Task GetFavoriteWallpapersAsync(int id, int page, int pageSize)
        {
            if (page == 1)
            {
                FavoriteWallpapers.Clear();
            }
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var url = string.Format("{0}wallpaper/favorite/{1}/{2}/{3}",
                    HttpClientHelper.baseUrl, id, page, pageSize);
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
                    url = HttpClientHelper.baseUrl
                        + "download/picture/small/" + wallpaper.W_Location;
                    var image = await ImageHelper.GetImageAsync(client, url);
                    this.FavoriteWallpapers.Add(new WallpaperDto
                    {
                        WallpaperInfo = wallpaper,
                        Image = image
                    });
                }
            }
        }

        public async Task GetSpaceWallpapersAsync(int id, int page, int pageSize)
        {
            if (page == 1)
            {
                SpaceWallpapers.Clear();
            }
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var url = string.Format("{0}wallpaper/space/{1}/{2}/{3}",
                    HttpClientHelper.baseUrl, id, page, pageSize);
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
                    url = HttpClientHelper.baseUrl
                        + "download/picture/origin/" + wallpaper.W_Location;
                    var image = await ImageHelper.GetImageAsync(client, url);
                    this.SpaceWallpapers.Add(new WallpaperDto
                    {
                        WallpaperInfo = wallpaper,
                        Image = image
                    });
                }
            }
        }
    }
}
