﻿using Newtonsoft.Json.Linq;
using PictureWhisper.Client.Helpers;
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
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UserPage : Page
    {
        private UserViewModel UserVM { get; set; }
        private int UserId { get; set; }
        private bool IsFollow { get; set; }

        public UserPage()
        {
            UserVM = new UserViewModel();
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private async void UploadAvatarButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;

            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".bmp");

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                UserVM.User.UserAvatar = await ImageHelper.FromFileAsync(file);
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
                        var resp = await client.PostAsync(new Uri(url), form);
                        if (resp.IsSuccessStatusCode)
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
                            if (resp.IsSuccessStatusCode)
                            {
                                await UserVM.GetAvatarAsync(imagePath);
                            }
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
                UploadErrorMsgTextBlock.Text += "· 获取图片失败" + Environment.NewLine;
            }
            if (UploadErrorMsgTextBlock.Text.Contains("·"))
            {
                UploadErrorMsgTextBlock.Visibility = Visibility.Visible;
            }
        }

        private async void UIDFollowButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserId == UserVM.User.UserInfo.U_ID)
            {
                return;
            }
            IsFollow = !IsFollow;
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                if (IsFollow)
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
                        IsFollow = !IsFollow;
                    }
                    else
                    {
                        UserVM.User.UserInfo.U_FollowerNum++;
                    }
                }
                else
                {
                    var url = HttpClientHelper.baseUrl + "follow/" + UserId + "/" +
                        UserVM.User.UserInfo.U_ID;
                    var resp = await client.DeleteAsync(new Uri(url));
                    if (!resp.IsSuccessStatusCode)
                    {
                        IsFollow = !IsFollow;
                    }
                    else
                    {
                        UserVM.User.UserInfo.U_FollowerNum--;
                    }
                }
            }
            if (IsFollow)
            {
                UIDFollowButton.Content = UserVM.User.FollowButtonText;
            }
            else
            {
                UIDFollowButton.Content = "已关注";
            }
        }

        private void UIDEditInfoButton_Click(object sender, RoutedEventArgs e)
        {
            UserInfoEditStackPanel.Visibility = Visibility.Visible;
            UserInfoDisplayStackPanel.Visibility = Visibility.Collapsed;
        }

        private async void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            var userOp = new List<dynamic>();
            EditErrorMsgTextBlock.Text = "错误信息：" + Environment.NewLine;
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
                var resp = await client.SendRequestAsync(request);
                if (resp.IsSuccessStatusCode)
                {
                    UserVM.User.UserInfo.U_Name = NameTextBox.Text;
                    UserVM.User.UserInfo.U_Info = InfoTextBox.Text;
                }
                else
                {
                    EditErrorMsgTextBlock.Text = "· 修改失败" + Environment.NewLine;
                    EditErrorMsgTextBlock.Visibility = Visibility.Visible;
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            UserInfoEditStackPanel.Visibility = Visibility.Collapsed;
            UserInfoDisplayStackPanel.Visibility = Visibility.Visible;
            NameTextBox.Text = UserVM.User.UserInfo.U_Name;
            InfoTextBox.Text = UserVM.User.UserInfo.U_Info;
        }

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

        private async void UIDSignoutButton_Click(object sender, RoutedEventArgs e)
        {
            await SQLiteHelper.RemoveSigninInfoAsync(SQLiteHelper.GetSigninInfo());
            var rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(SigninPage));
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                UserVM.User.UserInfo = (UserInfoDto)e.Parameter;
                await UserVM.GetAvatarAsync(UserVM.User.UserInfo.U_Avatar);
                UserVM.FillInfo();
                if (UserId != UserVM.User.UserInfo.U_ID)
                {
                    using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
                    {
                        var url = HttpClientHelper.baseUrl + "follow/" + UserId +
                            "/" + UserVM.User.UserInfo.U_ID;
                        var resp = await client.GetAsync(new Uri(url));
                        if (resp.IsSuccessStatusCode)
                        {
                            IsFollow = true;
                            UIDFollowButton.Content = "已关注";
                        }
                        else
                        {
                            IsFollow = false;
                        }
                    }
                }
            }
            UserId = SQLiteHelper.GetSigninInfo().SI_UserID;
            UploadErrorMsgTextBlock.Visibility = Visibility.Collapsed;
            EditErrorMsgTextBlock.Visibility = Visibility.Collapsed;
            UserInfoEditStackPanel.Visibility = Visibility.Collapsed;
            UserInfoDisplayStackPanel.Visibility = Visibility.Visible;
            UIDEditInfoButton.Visibility = Visibility.Collapsed;
            UserDeleteButton.Visibility = Visibility.Collapsed;
            UIDSignoutButton.Visibility = Visibility.Collapsed;
            if (UserId == UserVM.User.UserInfo.U_ID)
            {
                UIDEditInfoButton.Visibility = Visibility.Visible;
                UserDeleteButton.Visibility = Visibility.Visible;
                UIDSignoutButton.Visibility = Visibility.Visible;
            }
            base.OnNavigatedTo(e);
        }
    }
}