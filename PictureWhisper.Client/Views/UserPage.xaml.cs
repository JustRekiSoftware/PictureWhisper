using Newtonsoft.Json.Linq;
using PictureWhisper.Client.BackgroundTask;
using PictureWhisper.Client.Helper;
using PictureWhisper.Client.ViewModels;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PictureWhisper.Client.Views
{
    /// <summary>
    /// 用户页面
    /// </summary>
    public sealed partial class UserPage : Page
    {
        private UserViewModel UserVM { get; set; }
        private int UserId { get; set; }

        public UserPage()
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
                            + "upload/picture/" + UserId + "/avatar";
                        var form = new HttpMultipartFormDataContent();
                        var fileContent = new HttpStreamContent(stream);
                        fileContent.Headers.ContentDisposition = new HttpContentDispositionHeaderValue("form-data")
                        {
                            Name = "fileToUpload",
                            FileName = file.Name
                        };
                        form.Add(fileContent);
                        var resp = await client.PostAsync(new Uri(url), form);//上传头像
                        if (resp.IsSuccessStatusCode)//成功后修改头像的下载路径
                        {
                            var imagePath = await resp.Content.ReadAsStringAsync();
                            url = HttpClientHelper.baseUrl + "user/" + UserVM.User.UserInfo.U_ID;
                            var userOp = new List<dynamic>();
                            userOp.Add(new
                            {
                                op = "replace",
                                path = "U_Avatar",
                                value = imagePath
                            });
                            var content = new HttpStringContent(JArray.FromObject(userOp).ToString());
                            content.Headers.ContentType = new HttpMediaTypeHeaderValue("application/json-patch+json");
                            var request = new HttpRequestMessage(HttpMethod.Patch, new Uri(url))
                            {
                                Content = content
                            };
                            resp = await client.SendRequestAsync(request);
                            if (resp.IsSuccessStatusCode)//修改成功后再次获取头像，确保修改成功
                            {
                                await UserVM.GetAvatarAsync(imagePath);
                            }
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
                UploadErrorMsgTextBlock.Text += "· 获取图片失败或图片大于2M" + Environment.NewLine;
            }
            if (UploadErrorMsgTextBlock.Text.Contains("·"))
            {
                UploadErrorMsgTextBlock.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// 点击关注按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void UIDFollowButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserId == UserVM.User.UserInfo.U_ID)
            {
                return;
            }
            UserVM.User.IsFollow = !UserVM.User.IsFollow;
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                if (UserVM.User.IsFollow)//关注
                {
                    var url = HttpClientHelper.baseUrl + "follow";
                    var followInfo = new T_Follow();
                    followInfo.FLW_FollowerID = UserId;
                    followInfo.FLW_FollowedID = UserVM.User.UserInfo.U_ID;
                    var content = new HttpStringContent(JObject.FromObject(followInfo).ToString());
                    content.Headers.ContentType = new HttpMediaTypeHeaderValue("application/json");
                    var resp = await client.PostAsync(new Uri(url), content);
                    if (!resp.IsSuccessStatusCode)
                    {
                        UserVM.User.IsFollow = !UserVM.User.IsFollow;
                    }
                    else
                    {
                        UserVM.User.UserInfo.U_FollowerNum++;
                    }
                }
                else//取消关注
                {
                    var url = HttpClientHelper.baseUrl + "follow/" + UserId + "/" +
                        UserVM.User.UserInfo.U_ID;
                    var resp = await client.DeleteAsync(new Uri(url));
                    if (!resp.IsSuccessStatusCode)
                    {
                        UserVM.User.IsFollow = !UserVM.User.IsFollow;
                    }
                    else
                    {
                        UserVM.User.UserInfo.U_FollowerNum--;
                    }
                }
            }
            UserVM.FillInfo();//补充显示信息
        }

        /// <summary>
        /// 点击修改信息按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UIDEditInfoButton_Click(object sender, RoutedEventArgs e)
        {
            UserInfoEditStackPanel.Visibility = Visibility.Visible;
            UserInfoDisplayStackPanel.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 点击完成按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            var userOp = new List<dynamic>();
            EditErrorMsgTextBlock.Text = "错误信息：" + Environment.NewLine;
            //检查输入是否正确
            if (NameTextBox.Text == string.Empty)
            {
                EditErrorMsgTextBlock.Text = "· 未输入昵称" + Environment.NewLine;
            }
            else
            {
                userOp.Add(new
                {
                    op = "replace",
                    path = "U_Name",
                    value = NameTextBox.Text
                });
            }
            if (InfoTextBox.Text == string.Empty)
            {
                EditErrorMsgTextBlock.Text = "· 未输入简介" + Environment.NewLine;
            }
            else
            {
                userOp.Add(new
                {
                    op = "replace",
                    path = "U_Info",
                    value = InfoTextBox.Text
                });
            }
            if (EditErrorMsgTextBlock.Text.Contains("·"))
            {
                EditErrorMsgTextBlock.Visibility = Visibility.Visible;
                return;
            }
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var url = HttpClientHelper.baseUrl + "user/" + UserVM.User.UserInfo.U_ID;
                var content = new HttpStringContent(JArray.FromObject(userOp).ToString());
                content.Headers.ContentType = new HttpMediaTypeHeaderValue("application/json-patch+json");
                var request = new HttpRequestMessage(HttpMethod.Patch, new Uri(url))
                {
                    Content = content
                };
                var resp = await client.SendRequestAsync(request);//发送更新请求
                if (resp.IsSuccessStatusCode)
                {
                    UserVM.User.UserInfo.U_Name = NameTextBox.Text;
                    UserVM.User.UserInfo.U_Info = InfoTextBox.Text;
                    UserInfoEditStackPanel.Visibility = Visibility.Collapsed;
                    UserInfoDisplayStackPanel.Visibility = Visibility.Visible;
                }
                else
                {
                    EditErrorMsgTextBlock.Text = "· 修改失败" + Environment.NewLine;
                    EditErrorMsgTextBlock.Visibility = Visibility.Visible;
                }
            }
        }

        /// <summary>
        /// 点击取消按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            UserInfoEditStackPanel.Visibility = Visibility.Collapsed;
            UserInfoDisplayStackPanel.Visibility = Visibility.Visible;
            NameTextBox.Text = UserVM.User.UserInfo.U_Name;
            InfoTextBox.Text = UserVM.User.UserInfo.U_Info;
        }

        /// <summary>
        /// 点击举报按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserReportButton_Click(object sender, RoutedEventArgs e)
        {
            var reportInfo = new T_Report();
            reportInfo.RPT_ReporterID = UserId;
            reportInfo.RPT_ReportedID = UserVM.User.UserInfo.U_ID;
            reportInfo.RPT_Type = (short)ReportType.用户;
            dynamic param = new
            {
                ReportInfo = reportInfo,
                MainPageName = "UserMainPage"
            };
            UserMainPage.PageFrame.Navigate(typeof(ReportPage), param);
        }

        /// <summary>
        /// 点击删除账号按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void UserDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var contentDialog = new ContentDialog
            {
                Title = "删除账号",
                Content = "是否确认删除账号？",
                PrimaryButtonText = "确认",
                SecondaryButtonText = "取消"
            };
            contentDialog.PrimaryButtonClick += async (_sender, _e) =>
            {
                using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
                {
                    var url = HttpClientHelper.baseUrl + "user/" + UserId;
                    var resp = await client.DeleteAsync(new Uri(url));
                    if (resp.IsSuccessStatusCode)
                    {
                        await SQLiteHelper.RemoveSigninInfoAsync(SQLiteHelper.GetSigninInfo());
                        BackgroundTaskHelper.UnRegisterBackgroundTask(typeof(AutoSetWallpaperTask).Name);
                        var rootFrame = Window.Current.Content as Frame;
                        if (rootFrame.CanGoBack)
                        {
                            rootFrame.GoBack();
                        }
                    }
                }
            };
            contentDialog.SecondaryButtonClick += (_sender, _e) =>
            {
                contentDialog.Hide();
            };
            await contentDialog.ShowAsync();
        }

        /// <summary>
        /// 点击注销登录按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void UIDSignoutButton_Click(object sender, RoutedEventArgs e)
        {
            await SQLiteHelper.RemoveSigninInfoAsync(SQLiteHelper.GetSigninInfo());
            BackgroundTaskHelper.UnRegisterBackgroundTask(typeof(AutoSetWallpaperTask).Name);
            var rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(SigninPage));
        }

        /// <summary>
        /// 导航到该页面时的事件
        /// </summary>
        /// <param name="e"></param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (UserMainPage.Page != null)
            {
                UserMainPage.Page.HyperLinkButtonFocusChange("UserInfoHyperlinkButton");
            }
            if (e.Parameter != null)
            {
                UserId = SQLiteHelper.GetSigninInfo().SI_UserID;
                UserVM.User.UserInfo = (UserInfoDto)e.Parameter;
                await UserVM.GetAvatarAsync(UserVM.User.UserInfo.U_Avatar);
                await UserVM.GetIsFollowAsync(UserVM.User.UserInfo.U_ID);
                UserVM.FillInfo();
            }
            UserId = SQLiteHelper.GetSigninInfo().SI_UserID;
            UploadErrorMsgTextBlock.Visibility = Visibility.Collapsed;
            EditErrorMsgTextBlock.Visibility = Visibility.Collapsed;
            UserInfoEditStackPanel.Visibility = Visibility.Collapsed;
            UserInfoDisplayStackPanel.Visibility = Visibility.Visible;
            UIDEditInfoButton.Visibility = Visibility.Collapsed;
            UserDeleteButton.Visibility = Visibility.Collapsed;
            UIDSignoutButton.Visibility = Visibility.Collapsed;
            UploadAvatarButton.Visibility = Visibility.Collapsed;
            if (UserId == UserVM.User.UserInfo.U_ID)
            {
                UIDEditInfoButton.Visibility = Visibility.Visible;
                UserDeleteButton.Visibility = Visibility.Visible;
                UIDSignoutButton.Visibility = Visibility.Visible;
                UploadAvatarButton.Visibility = Visibility.Visible;
            }
            base.OnNavigatedTo(e);
        }
    }
}
