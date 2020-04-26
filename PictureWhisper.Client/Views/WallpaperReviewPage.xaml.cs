using Newtonsoft.Json.Linq;
using PictureWhisper.Client.Helper;
using PictureWhisper.Client.ViewModels;
using PictureWhisper.Domain.Entites;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
    public sealed partial class WallpaperReviewPage : Page
    {
        private WallpaperListViewModel WallpaperLVM { get; set; }
        private int CurrentIndex { get; set; }
        private BitmapImage CurrentImage { get; set; }
        private T_Wallpaper CurrentWallpaper { get; set; }
        private readonly int PageSize = 20;
        private int PageNum { get; set; }
        private int UserId { get; set; }

        public WallpaperReviewPage()
        {
            WallpaperLVM = new WallpaperListViewModel();
            this.InitializeComponent();
        }

        private async void PassButton_Click(object sender, RoutedEventArgs e)
        {
            await SendReviewInfoAsync(true);
        }

        private async void NotPassButton_Click(object sender, RoutedEventArgs e)
        {
            await SendReviewInfoAsync(false);
        }

        private async void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentIndex == 0)
            {
                PageNum = 1;
                await LoadReviewWallpapersAsync(PageNum++);
            }
            if (CurrentIndex > 0)
            {
                PrevFontIcon.Glyph = "\xE76B";
                CurrentIndex--;
                CurrentImage = WallpaperLVM.ReviewWallpapers[CurrentIndex].Image;
                CurrentWallpaper = WallpaperLVM.ReviewWallpapers[CurrentIndex].WallpaperInfo;
            }
            if (CurrentIndex == 0)
            {
                PrevFontIcon.Glyph = "\xE72C";
            }
        }

        private async void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentIndex < WallpaperLVM.ReviewWallpapers.Count)
            {
                CurrentIndex++;
                CurrentImage = WallpaperLVM.ReviewWallpapers[CurrentIndex].Image;
                CurrentWallpaper = WallpaperLVM.ReviewWallpapers[CurrentIndex].WallpaperInfo;
            }
            if (CurrentIndex > WallpaperLVM.ReviewWallpapers.Count - 5)
            {
                await LoadReviewWallpapersAsync(PageNum++);
            }
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            UserId = SQLiteHelper.GetSigninInfo().SI_UserID;
            PageNum = 1;
            CurrentIndex = 0;
            await LoadReviewWallpapersAsync(PageNum++);
            base.OnNavigatedTo(e);
        }

        private async Task LoadReviewWallpapersAsync(int page)
        {
            await WallpaperLVM.GetReviewWallpapersAsync(page, PageSize);
        }

        private async Task SendReviewInfoAsync(bool isPass)
        {
            ErrorMessageTextBlock.Text += "错误信息：" + Environment.NewLine;
            var reviewInfo = new T_Review()
            {
                RV_ReviewerID = UserId,
                RV_ReviewedID = WallpaperLVM
                    .ReviewWallpapers[CurrentIndex].WallpaperInfo.W_ID,
                RV_Type = (short)ReviewType.壁纸审核,
                RV_Result = isPass,
                RV_MsgToReporterID = 0,
                RV_MsgToReportedID = WallpaperLVM
                    .ReviewWallpapers[CurrentIndex].WallpaperInfo.W_PublisherID
            };
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var url = HttpClientHelper.baseUrl + "review";
                var content = new HttpStringContent(JObject.FromObject(reviewInfo).ToString());
                content.Headers.ContentType = new HttpMediaTypeHeaderValue("application/json");
                var resp = await client.PostAsync(new Uri(url), content);
                if (!resp.IsSuccessStatusCode)
                {
                    ErrorMessageTextBlock.Text += "· 发送失败" + Environment.NewLine;
                    ErrorMessageTextBlock.Visibility = Visibility.Visible;
                }
                else
                {
                    WallpaperLVM.ReviewWallpapers.Remove(WallpaperLVM
                        .ReviewWallpapers[CurrentIndex]);
                    if (CurrentIndex < WallpaperLVM.ReviewWallpapers.Count)
                    {
                        CurrentIndex++;
                        CurrentImage = WallpaperLVM.ReviewWallpapers[CurrentIndex].Image;
                        CurrentWallpaper = WallpaperLVM.ReviewWallpapers[CurrentIndex].WallpaperInfo;
                    }
                    if (CurrentIndex > WallpaperLVM.ReviewWallpapers.Count - 5)
                    {
                        await LoadReviewWallpapersAsync(PageNum++);
                    }
                    ErrorMessageTextBlock.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
