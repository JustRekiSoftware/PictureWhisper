using Newtonsoft.Json.Linq;
using PictureWhisper.Client.Helper;
using PictureWhisper.Domain.Abstract;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace PictureWhisper.Client.ViewModels
{
    public class WallpaperViewModel : BindableBase
    {
        private WallpaperDto wallpaper;
        public WallpaperDto Wallpaper
        {
            get { return wallpaper; }
            set { SetProperty(ref wallpaper, value); }
        }

        public WallpaperViewModel()
        {
            Wallpaper = new WallpaperDto();
        }

        public async Task GetWallpaperAsync(int id)
        {
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var url = HttpClientHelper.baseUrl + "wallpaper/" + id;
                var response = await client.GetAsync(new Uri(url));
                if (!response.IsSuccessStatusCode)
                {
                    return;
                }
                var wallpaper = JObject.Parse(
                    await response.Content.ReadAsStringAsync());
                var result = wallpaper.ToObject<T_Wallpaper>();
                if (result == null)
                {
                    return;
                }
                Wallpaper.WallpaperInfo = result;
                url = HttpClientHelper.baseUrl + 
                    "download/picture/origin/" + Wallpaper.WallpaperInfo.W_Location;
                Wallpaper.Image = await ImageHelper.GetImageAsync(client, url);
                url = HttpClientHelper.baseUrl + "user/" + Wallpaper.WallpaperInfo.W_PublisherID;
                response = await client.GetAsync(new Uri(url));
                if (!response.IsSuccessStatusCode)
                {
                    return;
                }
                var userInfoDto = JObject.Parse(await response.Content.ReadAsStringAsync())
                    .ToObject<UserInfoDto>();
                url = HttpClientHelper.baseUrl
                    + "download/picture/small/" + userInfoDto.U_Avatar;
                var image = await ImageHelper.GetImageAsync(client, url);
                Wallpaper.PublisherInfo = userInfoDto;
                Wallpaper.PublisherAvatar = image;
            }
        }

        public async Task GetWallpaperAsync()
        {
            if (Wallpaper.WallpaperInfo == null)
            {
                return;
            }
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var url = HttpClientHelper.baseUrl +
                    "download/picture/origin/" + Wallpaper.WallpaperInfo.W_Location;
                Wallpaper.Image = await ImageHelper.GetImageAsync(client, url);
                url = HttpClientHelper.baseUrl + "user/" + Wallpaper.WallpaperInfo.W_PublisherID;
                var response = await client.GetAsync(new Uri(url));
                if (!response.IsSuccessStatusCode)
                {
                    return;
                }
                var userInfoDto = JObject.Parse(await response.Content.ReadAsStringAsync())
                    .ToObject<UserInfoDto>();
                url = HttpClientHelper.baseUrl
                    + "download/picture/small/" + userInfoDto.U_Avatar;
                var image = await ImageHelper.GetImageAsync(client, url);
                Wallpaper.PublisherInfo = userInfoDto;
                Wallpaper.PublisherAvatar = image;
            }
        }

        public async Task GetImageAsync(string imagePath)
        {
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var url = HttpClientHelper.baseUrl +
                    "download/picture/origin/" + imagePath;
                Wallpaper.Image = await ImageHelper.GetImageAsync(client, url);
            }
        }
    }
}
