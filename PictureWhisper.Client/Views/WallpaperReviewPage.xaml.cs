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
    /// 壁纸审核页面
    /// </summary>
    public sealed partial class WallpaperReviewPage : Page
    {
        private WallpaperListViewModel WallpaperLVM { get; set; }
        private int CurrentIndex { get; set; }
        private ReviewViewModel ReviewVM { get; set; }
        private readonly int Count = 10;
        private int UserId { get; set; }

        public WallpaperReviewPage()
        {
            WallpaperLVM = new WallpaperListViewModel();
            ReviewVM = new ReviewViewModel();
            this.InitializeComponent();
        }

        /// <summary>
        /// 点击审核通过按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void PassButton_Click(object sender, RoutedEventArgs e)
        {
            await SendReviewInfoAsync(true);
        }

        /// <summary>
        /// 点击审核不通过按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void NotPassButton_Click(object sender, RoutedEventArgs e)
        {
            await SendReviewInfoAsync(false);
        }

        /// <summary>
        /// 点击上一个或刷新按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentIndex == 0)//刷新
            {
                WallpaperLVM.UnReviewedWallpapers.Clear();
                await LoadReviewWallpapersAsync();
            }
            if (CurrentIndex > 0)//上一个
            {
                PrevFontIcon.Glyph = "\xE76B";
                CurrentIndex--;
                ReviewVM.Image = WallpaperLVM.UnReviewedWallpapers[CurrentIndex].Image;
                ReviewVM.Wallpaper = WallpaperLVM.UnReviewedWallpapers[CurrentIndex].WallpaperInfo;
            }
            if (CurrentIndex == 0)//当前壁纸为列表的第一个，则将上一个按钮变为刷新按钮
            {
                PrevFontIcon.Glyph = "\xE72C";
            }
        }

        /// <summary>
        /// 点击下一个按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void NextButton_Click(object sender, RoutedEventArgs e)
        {
            PrevFontIcon.Glyph = "\xE76B";
            if (CurrentIndex < WallpaperLVM.UnReviewedWallpapers.Count - 1)
            {
                CurrentIndex++;
                ReviewVM.Image = WallpaperLVM.UnReviewedWallpapers[CurrentIndex].Image;
                ReviewVM.Wallpaper = WallpaperLVM.UnReviewedWallpapers[CurrentIndex].WallpaperInfo;
            }
            if (CurrentIndex > WallpaperLVM.UnReviewedWallpapers.Count - 5)//要到列表尾部时自动加载
            {
                //CurrentIndex++;
                //ReviewVM.Image = WallpaperLVM.UnReviewedWallpapers[CurrentIndex].Image;
                //ReviewVM.Wallpaper = WallpaperLVM.UnReviewedWallpapers[CurrentIndex].WallpaperInfo;
                await LoadReviewWallpapersAsync();
            }
        }

        /// <summary>
        /// 导航到该页面时的事件
        /// </summary>
        /// <param name="e"></param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            UserId = SQLiteHelper.GetSigninInfo().SI_UserID;
            WallpaperLVM.UnReviewedWallpapers.Clear();
            CurrentIndex = 0;
            await LoadReviewWallpapersAsync();
            if (WallpaperLVM.UnReviewedWallpapers.Count == 0)
            {
                PrevButton.Visibility = Visibility.Collapsed;
                NextButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                ReviewVM.Image = WallpaperLVM.UnReviewedWallpapers[CurrentIndex].Image;
                ReviewVM.Wallpaper = WallpaperLVM.UnReviewedWallpapers[CurrentIndex].WallpaperInfo;
                PrevButton.Visibility = Visibility.Visible;
                NextButton.Visibility = Visibility.Visible;
            }
            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// 加载未审核的壁纸
        /// </summary>
        /// <returns></returns>
        private async Task LoadReviewWallpapersAsync()
        {
            if (WallpaperLVM.UnReviewedWallpapers.Count == 0)
            {
                await WallpaperLVM.GetUnReviewedWallpapersAsync(5);
            }
            else
            {
                await WallpaperLVM.GetUnReviewedWallpapersAsync(Count);
            }
            if (CurrentIndex < WallpaperLVM.UnReviewedWallpapers.Count)
            {
                ReviewVM.Image = WallpaperLVM.UnReviewedWallpapers[CurrentIndex].Image;
                ReviewVM.Wallpaper = WallpaperLVM.UnReviewedWallpapers[CurrentIndex].WallpaperInfo;
            }
        }

        /// <summary>
        /// 发送审核信息
        /// </summary>
        /// <param name="isPass"></param>
        /// <returns></returns>
        private async Task SendReviewInfoAsync(bool isPass)
        {
            if (WallpaperLVM.UnReviewedWallpapers.Count == 0)
            {
                ReviewVM.Image = new BitmapImage();
                ReviewVM.Wallpaper = new T_Wallpaper();
                return;
            }
            ErrorMessageTextBlock.Text += "错误信息：" + Environment.NewLine;
            var reviewInfo = new T_Review()
            {
                RV_ReviewerID = UserId,
                RV_ReviewedID = WallpaperLVM
                    .UnReviewedWallpapers[CurrentIndex].WallpaperInfo.W_ID,
                RV_Type = (short)ReviewType.壁纸审核,
                RV_Result = isPass,
                RV_MsgToReporterID = 0,
                RV_MsgToReportedID = WallpaperLVM
                    .UnReviewedWallpapers[CurrentIndex].WallpaperInfo.W_PublisherID
            };
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var url = HttpClientHelper.baseUrl + "review";
                var content = new HttpStringContent(JObject.FromObject(reviewInfo).ToString());
                content.Headers.ContentType = new HttpMediaTypeHeaderValue("application/json");
                var resp = await client.PostAsync(new Uri(url), content);//发送审核信息
                if (!resp.IsSuccessStatusCode)
                {
                    ErrorMessageTextBlock.Text += "· 发送失败" + Environment.NewLine;
                    ErrorMessageTextBlock.Visibility = Visibility.Visible;
                }
                else//审核信息发送成功
                {
                    WallpaperLVM.UnReviewedWallpapers.Remove(WallpaperLVM
                        .UnReviewedWallpapers[CurrentIndex]);//审核信息发送成功后从列表中去除该壁纸
                    if (WallpaperLVM.UnReviewedWallpapers.Count == 0)//列表中的壁纸都已审核
                    {
                        CurrentIndex = 0;
                        ReviewVM.Image = new BitmapImage();
                        ReviewVM.Wallpaper = new T_Wallpaper();
                    }
                    if (CurrentIndex < WallpaperLVM.UnReviewedWallpapers.Count - 1)//自动加载下一个
                    {
                        CurrentIndex++;
                        ReviewVM.Image = WallpaperLVM.UnReviewedWallpapers[CurrentIndex].Image;
                        ReviewVM.Wallpaper = WallpaperLVM.UnReviewedWallpapers[CurrentIndex].WallpaperInfo;
                    }
                    if (CurrentIndex > WallpaperLVM.UnReviewedWallpapers.Count - 5)//要到列表尾部则自动加载更多
                    {
                        await LoadReviewWallpapersAsync();
                    }
                    ErrorMessageTextBlock.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
