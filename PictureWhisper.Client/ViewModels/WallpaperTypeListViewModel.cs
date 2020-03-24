using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PictureWhisper.Client.Helpers;
using PictureWhisper.Domain.Entites;

namespace PictureWhisper.Client.ViewModels
{
    public class WallpaperTypeListViewModel
    {
        public ObservableCollection<T_WallpaperType> WallpaperTypes { get; set; }

        public WallpaperTypeListViewModel()
        {
            WallpaperTypes = new ObservableCollection<T_WallpaperType>();
        }

        public async Task GetWallpaperTypesAsync()
        {
            if (WallpaperTypes.Count != 0)
            {
                return;
            }
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
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
