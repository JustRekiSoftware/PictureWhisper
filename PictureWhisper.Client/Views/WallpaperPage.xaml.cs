using Newtonsoft.Json.Linq;
using PictureWhisper.Client.Helper;
using PictureWhisper.Client.ViewModels;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System.UserProfile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PictureWhisper.Client.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WallpaperPage : Page
    { 
        private WallpaperViewModel WallpaperVM { get; set; }
        private int UserId { get; set; }
        private bool IsLike { get; set; }
        private bool IsFavorite { get; set; }

        public WallpaperPage()
        {
            WallpaperVM = new WallpaperViewModel();
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private void WallpaperReportButton_Click(object sender, RoutedEventArgs e)
        {
            var reportInfo = new T_Report();
            reportInfo.RPT_ReporterID = UserId;
            reportInfo.RPT_ReportedID = WallpaperVM.Wallpaper.WallpaperInfo.W_ID;
            reportInfo.RPT_Type = (short)ReportType.壁纸;
            dynamic param = new
            {
                ReportInfo = reportInfo,
                MainPageName = "WallpaperMainPage"
            };
            WallpaperMainPage.PageFrame.Navigate(typeof(ReportPage), param);
        }

        private async void WallpaperDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var contentDialog = new ContentDialog
            {
                Title = "删除壁纸",
                Content = "是否删除壁纸？",
                PrimaryButtonText = "是",
                SecondaryButtonText = "否"
            };
            contentDialog.PrimaryButtonClick += async (_sender, _e) =>
            {
                using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
                {
                    var url = HttpClientHelper.baseUrl + "wallpaper/" + WallpaperVM.Wallpaper.WallpaperInfo.W_ID;
                    var resp = await client.DeleteAsync(new Uri(url));
                    if (resp.IsSuccessStatusCode)
                    {
                        if (MainPage.PageFrame.CanGoBack)
                        {
                            MainPage.PageFrame.GoBack();
                        }
                    }
                    else
                    {
                        contentDialog.Hide();
                    }
                }
            };
            contentDialog.SecondaryButtonClick += (_sender, _e) =>
            {
                contentDialog.Hide();
            };
            await contentDialog.ShowAsync();
        }

        private async void LikeButton_Click(object sender, RoutedEventArgs e)
        {
            IsLike = !IsLike;
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                if (IsLike)
                {
                    var url = HttpClientHelper.baseUrl + "like";
                    var likeInfo = new T_Like();
                    likeInfo.L_LikerID = UserId;
                    likeInfo.L_WallpaperID = WallpaperVM.Wallpaper.WallpaperInfo.W_ID;
                    var content = new HttpStringContent(JObject.FromObject(likeInfo).ToString());
                    content.Headers.ContentType = new HttpMediaTypeHeaderValue("application/json");
                    var resp = await client.PostAsync(new Uri(url), content);
                    if (!resp.IsSuccessStatusCode)
                    {
                        IsLike = !IsLike;
                    }
                    else
                    {
                        WallpaperVM.Wallpaper.WallpaperInfo.W_LikeNum++;
                    }
                }
                else
                {
                    var url = HttpClientHelper.baseUrl + "like/" + UserId + "/" +
                        WallpaperVM.Wallpaper.WallpaperInfo.W_ID;
                    var resp = await client.DeleteAsync(new Uri(url));
                    if (!resp.IsSuccessStatusCode)
                    {
                        IsLike = !IsLike;
                    }
                    else
                    {
                        WallpaperVM.Wallpaper.WallpaperInfo.W_LikeNum--;
                    }
                }
            }
            if (IsLike)
            {
                LikeButton.Foreground = new SolidColorBrush(ColorHelper.GetAccentColor());
            }
            else
            {
                LikeButton.Foreground = new SolidColorBrush(ColorHelper.GetForegroudColor());
            }
        }

        private async void FavoriteButton_Click(object sender, RoutedEventArgs e)
        {
            IsFavorite = !IsFavorite;
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                if (IsFavorite)
                {
                    var url = HttpClientHelper.baseUrl + "favorite";
                    var favoriteInfo = new T_Favorite();
                    favoriteInfo.FVRT_FavoritorID = UserId;
                    favoriteInfo.FVRT_WallpaperID = WallpaperVM.Wallpaper.WallpaperInfo.W_ID;
                    var content = new HttpStringContent(JObject.FromObject(favoriteInfo).ToString());
                    content.Headers.ContentType = new HttpMediaTypeHeaderValue("application/json");
                    var resp = await client.PostAsync(new Uri(url), content);
                    if (!resp.IsSuccessStatusCode)
                    {
                        IsFavorite = !IsFavorite;
                    }
                    else
                    {
                        WallpaperVM.Wallpaper.WallpaperInfo.W_FavoriteNum++;
                    }
                }
                else
                {
                    var url = HttpClientHelper.baseUrl + "favorite/" + UserId + "/" +
                        WallpaperVM.Wallpaper.WallpaperInfo.W_ID;
                    var resp = await client.DeleteAsync(new Uri(url));
                    if (!resp.IsSuccessStatusCode)
                    {
                        IsFavorite = !IsFavorite;
                    }
                    else
                    {
                        WallpaperVM.Wallpaper.WallpaperInfo.W_FavoriteNum--;
                    }
                }
            }
            if (IsFavorite)
            {
                FavoriteButton.Foreground = new SolidColorBrush(ColorHelper.GetAccentColor());
            }
            else
            {
                FavoriteButton.Foreground = new SolidColorBrush(ColorHelper.GetForegroudColor());
            }
        }

        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            var fileName = WallpaperVM.Wallpaper.WallpaperInfo.W_Location
                .Split("/").ToList().LastOrDefault();
            var url = HttpClientHelper.baseUrl + "download/picture/origin/"
                + WallpaperVM.Wallpaper.WallpaperInfo.W_Location;
            var fileSavePicker = new FileSavePicker();
            fileSavePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            fileSavePicker.FileTypeChoices.Add("图像文件", new List<string>() { ".png" });
            fileSavePicker.SuggestedFileName = fileName;
            var saveFile = await fileSavePicker.PickSaveFileAsync();
            await SaveWallpaperToLocalAsync(saveFile, url);
        }

        private async Task SaveWallpaperToLocalAsync(StorageFile saveFile, string url)
        {
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var buffer = await ImageHelper.GetImageBufferAsync(client, url);
                await FileIO.WriteBufferAsync(saveFile, buffer);
            }
        }

        private async Task TempSaveWallpaperToLocalAsync(string url)
        {
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var buffer = await ImageHelper.GetImageBufferAsync(client, url);
                if (buffer == null)
                {
                    return;
                }
                //必须放在LocalFolder里才能正确设置壁纸
                var saveFile = await ApplicationData.Current.LocalFolder
                    .CreateFileAsync("temp.png", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteBufferAsync(saveFile, buffer);
            }
        }

        private async void WallpaperSetButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserProfilePersonalizationSettings.IsSupported() == true)
            {
                var current = UserProfilePersonalizationSettings.Current;
                var url = HttpClientHelper.baseUrl + "download/picture/origin/"
                    + WallpaperVM.Wallpaper.WallpaperInfo.W_Location;
                await TempSaveWallpaperToLocalAsync(url);
                //必须放在LocalFolder里才能正确设置壁纸
                var path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "temp.png");
                var file = await StorageFile.GetFileFromPathAsync(path);
                await current.TrySetWallpaperImageAsync(file);
                MoreButtonFlyout.Hide();
            }
            else
            {
                var contentDialog = new ContentDialog
                {
                    Title = "无法设置壁纸",
                    Content = "系统不支持软件设置壁纸，请下载后自行设置。",
                    PrimaryButtonText = "关闭",
                };
                contentDialog.PrimaryButtonClick += (_sender, _e) =>
                {
                    contentDialog.Hide();
                };
                await contentDialog.ShowAsync();
            }
        }

        private void PublisherAvatarButton_Click(object sender, RoutedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(UserMainPage), WallpaperVM.Wallpaper.PublisherInfo);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            WallpaperDeleteButton.Visibility = Visibility.Collapsed;
            if (e.Parameter != null)
            {
                WallpaperVM.Wallpaper.WallpaperInfo = (T_Wallpaper)e.Parameter;
                await WallpaperVM.GetImageAsync(WallpaperVM.Wallpaper.WallpaperInfo.W_Location);
                UserId = SQLiteHelper.GetSigninInfo().SI_UserID;
                using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
                {
                    var url = HttpClientHelper.baseUrl + "like/" + UserId + "/" +
                        WallpaperVM.Wallpaper.WallpaperInfo.W_ID;
                    var resp = await client.GetAsync(new Uri(url));
                    if (resp.IsSuccessStatusCode)
                    {
                        IsLike = bool.Parse(await resp.Content.ReadAsStringAsync());
                        if (IsLike)
                        {
                            LikeButton.Foreground = new SolidColorBrush(ColorHelper.GetAccentColor());
                        }
                        else
                        {
                            LikeButton.Foreground = new SolidColorBrush(ColorHelper.GetForegroudColor());
                        }
                    }
                    else
                    {
                        IsLike = false;
                    }
                    url = HttpClientHelper.baseUrl + "favorite/" + UserId + "/" +
                        WallpaperVM.Wallpaper.WallpaperInfo.W_ID;
                    resp = await client.GetAsync(new Uri(url));
                    if (resp.IsSuccessStatusCode)
                    {
                        IsFavorite = bool.Parse(await resp.Content.ReadAsStringAsync());
                        if (IsFavorite)
                        {
                            FavoriteButton.Foreground = new SolidColorBrush(ColorHelper.GetAccentColor());
                        }
                        else
                        {
                            FavoriteButton.Foreground = new SolidColorBrush(ColorHelper.GetForegroudColor());
                        }
                    }
                    else
                    {
                        IsFavorite = false;
                    }
                    url = HttpClientHelper.baseUrl + "user/" +
                        WallpaperVM.Wallpaper.WallpaperInfo.W_PublisherID;
                    WallpaperVM.Wallpaper.PublisherInfo =
                        JObject.Parse(await client.GetStringAsync(new Uri(url))).ToObject<UserInfoDto>();
                    url = HttpClientHelper.baseUrl + "download/picture/origin/" +
                        WallpaperVM.Wallpaper.PublisherInfo.U_Avatar;
                    WallpaperVM.Wallpaper.PublisherAvatar =
                        await ImageHelper.GetImageAsync(client, url);
                }
                if (UserId == WallpaperVM.Wallpaper.WallpaperInfo.W_PublisherID)
                {
                    WallpaperDeleteButton.Visibility = Visibility.Visible;
                }
            }
            base.OnNavigatedTo(e);
        }
    }
}
