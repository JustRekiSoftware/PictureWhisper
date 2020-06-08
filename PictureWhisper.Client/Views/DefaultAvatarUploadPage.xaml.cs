using Newtonsoft.Json.Linq;
using PictureWhisper.Client.Helper;
using PictureWhisper.Client.ViewModels;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
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
    /// 默认头像上传页面
    /// </summary>
    public sealed partial class DefaultAvatarUploadPage : Page
    {
        private UserViewModel UserVM { get; set; }
        private int UserId { get; set; }

        public DefaultAvatarUploadPage()
        {
            UserVM = new UserViewModel();
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;//启用缓存
        }

        /// <summary>
        /// 点击上传头像按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void UploadAvatarButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".bmp");
            StorageFile file = await picker.PickSingleFileAsync();//获取文件
            var fileSize = file == null ? 0.0 : (await file.GetBasicPropertiesAsync()).Size;
            if (fileSize > 0 && fileSize <= 2097152)//文件大小限制
            {
                UserVM.User.UserAvatar = await ImageHelper.FromFileAsync(file);//显示头像
                using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
                {
                    using (var stream = await file.OpenReadAsync())
                    {
                        var url = HttpClientHelper.baseUrl
                            + "upload/picture/default/avatar";
                        var form = new HttpMultipartFormDataContent();
                        var fileContent = new HttpStreamContent(stream);
                        fileContent.Headers.ContentDisposition = new HttpContentDispositionHeaderValue("form-data")
                        {
                            Name = "fileToUpload",
                            FileName = file.Name
                        };
                        form.Add(fileContent);
                        var resp = await client.PostAsync(new Uri(url), form);//上传头像
                        if (resp.IsSuccessStatusCode)//成功后再次获取头像确保无误
                        {
                            url = HttpClientHelper.baseUrl +
                                    "download/picture/origin/default/avatar.png";
                            UserVM.User.UserAvatar = await ImageHelper.GetImageAsync(client, url);//获取头像
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
                UploadErrorMsgTextBlock.Text += "· 获取图片失败或图片大于2M" + Environment.NewLine;
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
                AdminMainPage.Page.HyperLinkButtonFocusChange("UserInfoHyperlinkButton");
            }
            UserId = SQLiteHelper.GetSigninInfo().SI_UserID;
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var url = HttpClientHelper.baseUrl +
                    "download/picture/origin/default/avatar.png";
                UserVM.User.UserAvatar = await ImageHelper.GetImageAsync(client, url);//获取头像
            }
            base.OnNavigatedTo(e);
        }
    }
}
