using PictureWhisper.Client.Helper;
using PictureWhisper.Client.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PictureWhisper.Client.Views
{
    /// <summary>
    /// 今日壁纸上传页面
    /// </summary>
    public sealed partial class TodayWallpaperUploadPage : Page
    {
        private ImageViewModel ImageVM { get; set; }
        private string ImageCloudPath { get; set; }
        private int UserId { get; set; }

        public TodayWallpaperUploadPage()
        {
            ImageVM = new ImageViewModel();
            this.InitializeComponent();
        }

        /// <summary>
        /// 点击上传图片按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void UploadPictureButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".bmp");
            StorageFile file = await picker.PickSingleFileAsync();//选择图片
            var fileSize = file == null ? 0.0 : (await file.GetBasicPropertiesAsync()).Size;
            if (fileSize > 0 && fileSize <= 10485760)//图片大小限制
            {
                ImageVM.Image = await ImageHelper.FromFileAsync(file);
                using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
                {
                    using (var stream = await file.OpenReadAsync())
                    {
                        var url = HttpClientHelper.baseUrl
                            + "upload/picture/today";
                        var form = new HttpMultipartFormDataContent();
                        var fileContent = new HttpStreamContent(stream);
                        fileContent.Headers.ContentDisposition = new HttpContentDispositionHeaderValue("form-data")
                        {
                            Name = "fileToUpload",
                            FileName = file.Name
                        };
                        form.Add(fileContent);
                        var resp = await client.PostAsync(new Uri(url), form);//上传图片
                        if (resp.IsSuccessStatusCode)
                        {
                            ImageCloudPath = await resp.Content.ReadAsStringAsync();
                            UploadErrorMsgTextBlock.Visibility = Visibility.Collapsed;
                            UploadErrorMsgTextBlock.Text = string.Empty;
                        }
                        else
                        {
                            UploadErrorMsgTextBlock.Text = "错误信息：" + Environment.NewLine;
                            UploadErrorMsgTextBlock.Text += "· 图片上传失败" + Environment.NewLine;
                        }
                    }
                }
            }
            else
            {
                UploadErrorMsgTextBlock.Text = "错误信息：" + Environment.NewLine;
                UploadErrorMsgTextBlock.Text += "· 获取图片失败或图片大于10M" + Environment.NewLine;
            }
            if (UploadErrorMsgTextBlock.Text.Contains("·"))
            {
                UploadErrorMsgTextBlock.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// 导航到该页面时的事件
        /// </summary>
        /// <param name="e"></param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (AdminMainPage.Page != null)
            {
                AdminMainPage.Page.HyperLinkButtonFocusChange("TodayWallpaperUploadHyperlinkButton");
            }
            if (e.Parameter != null)
            {
                UserId = (int)e.Parameter;
            }
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var url = HttpClientHelper.baseUrl +
                    "download/picture/origin/today/" + DateTime.Now.ToString("yyyy-MM-dd") + ".png";
                ImageVM.Image = await ImageHelper.GetImageAsync(client, url);//获取今日壁纸
            }
            UploadErrorMsgTextBlock.Visibility = Visibility.Collapsed;
            UploadPictureButton.Content = "上传图片";
            base.OnNavigatedTo(e);
        }
    }
}
