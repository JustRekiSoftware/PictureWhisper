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
    /// <summary>
    /// 壁纸列表的ViewModel
    /// </summary>
    public class WallpaperListViewModel
    {
        public ObservableCollection<WallpaperDto> SearchResultWallpapers { get; set; }
        public ObservableCollection<WallpaperDto> TypeResultWallpapers { get; set; }
        public ObservableCollection<WallpaperDto> UnReviewedWallpapers { get; set; }
        public ObservableCollection<WallpaperDto> PublishedWallpapers { get; set; }
        public ObservableCollection<WallpaperDto> FavoriteWallpapers { get; set; }
        public ObservableCollection<WallpaperDto> SpaceWallpapers { get; set; }

        public WallpaperListViewModel()
        {
            SearchResultWallpapers = new ObservableCollection<WallpaperDto>();
            TypeResultWallpapers = new ObservableCollection<WallpaperDto>();
            UnReviewedWallpapers = new ObservableCollection<WallpaperDto>();
            PublishedWallpapers = new ObservableCollection<WallpaperDto>();
            FavoriteWallpapers = new ObservableCollection<WallpaperDto>();
            SpaceWallpapers = new ObservableCollection<WallpaperDto>();
        }

        /// <summary>
        /// 获取壁纸搜索结果
        /// </summary>
        /// <param name="queryData">搜索关键字</param>
        /// <param name="filterData">分区条件</param>
        /// <param name="orderData">排序条件</param>
        /// <param name="page">页数</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns></returns>
        public async Task GetSearchResultWallpapersAsync(string queryData,
            short filterData, string orderData, int page, int pageSize)
        {
            if (page == 1)
            {
                SearchResultWallpapers.Clear();
            }
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                //获取壁纸列表
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
                //补充壁纸信息
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

        /// <summary>
        /// 获取分区壁纸
        /// </summary>
        /// <param name="type">分区Id</param>
        /// <param name="page">页数</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns></returns>
        public async Task GetTypeResultWallpapersAsync(short type, int page, int pageSize)
        {
            if (page == 1)
            {
                TypeResultWallpapers.Clear();
            }
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                //获取壁纸列表
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
                //补充显示信息
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

        /// <summary>
        /// 获取未审核壁纸
        /// </summary>
        /// <param name="count">获取数量</param>
        /// <returns></returns>
        public async Task GetUnReviewedWallpapersAsync(int count)
        {
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                //获取壁纸列表
                var url = string.Format("{0}wallpaper/unreviewed/{1}",
                    HttpClientHelper.baseUrl, count);
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
                    url = HttpClientHelper.baseUrl
                        + "download/picture/origin/" + wallpaper.W_Location;
                    var image = await ImageHelper.GetImageAsync(client, url);
                    this.UnReviewedWallpapers.Add(new WallpaperDto
                    {
                        WallpaperInfo = wallpaper,
                        Image = image
                    });
                }
            }
        }

        /// <summary>
        /// 获取已发布壁纸
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="page">页数</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns></returns>
        public async Task GetPublishedWallpapersAsync(int id, int page, int pageSize)
        {
            if (page == 1)
            {
                PublishedWallpapers.Clear();
            }
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                //获取壁纸列表
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
                //补充显示信息
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

        /// <summary>
        /// 获取收藏壁纸
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="page">页数</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns></returns>
        public async Task GetFavoriteWallpapersAsync(int id, int page, int pageSize)
        {
            if (page == 1)
            {
                FavoriteWallpapers.Clear();
            }
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                //获取壁纸列表
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
                //补充显示信息
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

        /// <summary>
        /// 获取用户动态
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="page">页数</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns></returns>
        public async Task GetSpaceWallpapersAsync(int id, int page, int pageSize)
        {
            if (page == 1)
            {
                SpaceWallpapers.Clear();
            }
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                //获取壁纸列表
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
                //补充显示信息
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
