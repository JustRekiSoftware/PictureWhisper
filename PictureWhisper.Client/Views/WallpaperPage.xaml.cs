using Newtonsoft.Json.Linq;
using PictureWhisper.Client.Helper;
using PictureWhisper.Client.ViewModels;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System.UserProfile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
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
            var fileSavePicker = new FileSavePicker();
            fileSavePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            fileSavePicker.FileTypeChoices.Add("图像文件", new List<string>() { ".png" });
            fileSavePicker.SuggestedFileName = fileName;
            var saveFile = await fileSavePicker.PickSaveFileAsync();
            await SaveWallpaperToLocalAsync(saveFile);
        }

        private async Task SaveWallpaperToLocalAsync(StorageFile saveFile)
        {
            var writeableBitmap = await BitmapFactory.FromContent(WallpaperVM.Wallpaper.Image.UriSource);
            var outputBitmap = SoftwareBitmap.CreateCopyFromBuffer(
                writeableBitmap.PixelBuffer,
                BitmapPixelFormat.Bgra8,
                writeableBitmap.PixelWidth,
                writeableBitmap.PixelHeight
            );
            await SaveSoftwareBitmapToFileAsync(outputBitmap, saveFile);
        }

        private async Task TempSaveWallpaperToLocalAsync()
        {
            var writeableBitmap = await BitmapFactory.FromContent(WallpaperVM.Wallpaper.Image.UriSource);
            var outputBitmap = SoftwareBitmap.CreateCopyFromBuffer(
                writeableBitmap.PixelBuffer,
                BitmapPixelFormat.Bgra8,
                writeableBitmap.PixelWidth,
                writeableBitmap.PixelHeight
            );
            var saveFile = await ApplicationData.Current.TemporaryFolder
                .CreateFileAsync("temp.png", CreationCollisionOption.ReplaceExisting);
            await SaveSoftwareBitmapToFileAsync(outputBitmap, saveFile);
        }

        private async Task SaveSoftwareBitmapToFileAsync(SoftwareBitmap softwareBitmap, StorageFile outputFile)
        {
            using (var stream = await outputFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                var propertySet = new BitmapPropertySet();
                var qualityValue = new BitmapTypedValue(
                    1.0, // Maximum quality
                    PropertyType.Single);
                propertySet.Add("ImageQuality", qualityValue);
                // Create an encoder with the desired format
                var encoder = await BitmapEncoder
                    .CreateAsync(BitmapEncoder.PngEncoderId, stream, propertySet);

                // Set the software bitmap
                encoder.SetSoftwareBitmap(softwareBitmap);

                try
                {
                    await encoder.FlushAsync();
                }
                catch (Exception err)
                {
                    const int WINCODEC_ERR_UNSUPPORTEDOPERATION = unchecked((int)0x88982F81);
                    switch (err.HResult)
                    {
                        case WINCODEC_ERR_UNSUPPORTEDOPERATION:
                            // If the encoder does not support writing a thumbnail, then try again
                            // but disable thumbnail generation.
                            encoder.IsThumbnailGenerated = false;
                            break;
                        default:
                            throw;
                    }
                }

                if (encoder.IsThumbnailGenerated == false)
                {
                    await encoder.FlushAsync();
                }
            }
        }

        private async void WallpaperSetButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserProfilePersonalizationSettings.IsSupported() == true)
            {
                var current = UserProfilePersonalizationSettings.Current;
                await TempSaveWallpaperToLocalAsync();
                var path = Path.Combine(ApplicationData.Current.TemporaryFolder.Name, "temp.png");
                var file = await StorageFile.GetFileFromPathAsync(path);
                await current.TrySetWallpaperImageAsync(file);
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
                    }
                    else
                    {
                        IsFavorite = false;
                    }
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
