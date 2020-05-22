using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PictureWhisper.Client.Helper;
using PictureWhisper.Domain.Entites;

namespace PictureWhisper.Client.ViewModels
{
    /// <summary>
    /// 壁纸分区列表的ViewModel
    /// </summary>
    public class WallpaperTypeListViewModel
    {
        public ObservableCollection<T_WallpaperType> WallpaperTypes { get; set; }

        public WallpaperTypeListViewModel()
        {
            WallpaperTypes = new ObservableCollection<T_WallpaperType>();
        }

        /// <summary>
        /// 获取壁纸分区
        /// </summary>
        /// <returns></returns>
        public async Task GetWallpaperTypesAsync()
        {
            if (WallpaperTypes.Count != 0)
            {
                return;
            }
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                //获取壁纸分区列表
                var url = HttpClientHelper.baseUrl + "wallpaper/type";
                var response = await client.GetAsync(new Uri(url));
                if (!response.IsSuccessStatusCode)
                {
                    return;
                }
                var wallpaperTypes = JArray.Parse(
                    await response.Content.ReadAsStringAsync());
                var result = wallpaperTypes.ToObject<List<T_WallpaperType>>();
                if (result == null)
                {
                    return;
                }

                foreach (var type in result)
                {
                    this.WallpaperTypes.Add(type);
                }
            }
        }
    }
}
