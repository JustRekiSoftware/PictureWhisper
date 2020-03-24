using Newtonsoft.Json.Linq;
using PictureWhisper.Client.BackgroundTask.Tasks;
using PictureWhisper.Client.Domain.Concrete;
using PictureWhisper.Client.Domain.Entities;
using PictureWhisper.Client.Helpers;
using PictureWhisper.Client.Views;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PictureWhisper.Client
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SigninPage : Page
    {
        public SigninPage()
        {
            this.InitializeComponent();
        }

        private async void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorMsgTextBlock.Text = "错误信息：" + Environment.NewLine;
            if (EmailTextBox.Text == string.Empty 
                || !EmailTextBox.Text.Contains("@"))
            {
                ErrorMsgTextBlock.Text += "· 邮箱未输入或邮箱格式不正确" + Environment.NewLine;
            }
            if (PwdPasswordBox.Password.TrimEnd() == string.Empty)
            {
                ErrorMsgTextBlock.Text += "· 密码未输入" + Environment.NewLine;
            }
            if (ErrorMsgTextBlock.Text.Contains("·"))
            {
                ErrorMsgTextBlock.Visibility = Visibility.Visible;
                return;
            }
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var pwd = EncryptHelper.SHA256Encrypt(PwdPasswordBox.Password.TrimEnd());
                var url = HttpClientHelper.baseUrl + "user/signin/"
                    + EmailTextBox.Text + "/" + pwd;
                var resp = await client.GetAsync(new Uri(url));
                if (resp.IsSuccessStatusCode)
                {
                    var userSigninDto = JObject.Parse(await resp.Content.ReadAsStringAsync())
                            .ToObject<UserSigninDto>();
                    var signinInfo = SQLiteHelper.GetSigninInfo();
                    if (signinInfo != null)
                    {
                        signinInfo.SI_UserID = userSigninDto.U_ID;
                        signinInfo.SI_Email = EmailTextBox.Text;
                        signinInfo.SI_Password = pwd;
                        signinInfo.SI_Avatar = userSigninDto.U_Avatar;
                        signinInfo.SI_Type = userSigninDto.U_Type;
                        signinInfo.SI_Status = userSigninDto.U_Status;
                        await SQLiteHelper.UpdateSigninInfoAsync(signinInfo);
                    }
                    else
                    {
                        signinInfo = new T_SigninInfo
                        {
                            SI_UserID = userSigninDto.U_ID,
                            SI_Email = EmailTextBox.Text,
                            SI_Password = pwd,
                            SI_Avatar = userSigninDto.U_Avatar,
                            SI_Type = userSigninDto.U_Type,
                            SI_Status = userSigninDto.U_Status
                        };
                        await SQLiteHelper.AddSigninInfoAsync(signinInfo);
                    }
                    var settingInfo = SQLiteHelper.GetSettingInfo();
                    if (settingInfo == null)
                    {
                        settingInfo = new T_SettingInfo();
                        settingInfo.STI_AutoSetWallpaper = false;
                        await SQLiteHelper.AddSettingInfoAsync(settingInfo);
                    }
                    else
                    {
                        if (settingInfo.STI_AutoSetWallpaper)
                        {
                            await BackgroundTask.Helpers.BackgroundTaskHelper
                                .RegisterBackgroundTaskAsync(
                                typeof(AutoSetWallpaperTask),
                                typeof(AutoSetWallpaperTask).Name,
                                new TimeTrigger(60, false),
                                null,
                                true);
                        }
                    }
                    var rootFrame = Window.Current.Content as Frame;
                    rootFrame.Navigate(typeof(MainPage));
                }
                else
                {
                    var contentDialog = new ContentDialog
                    {
                        Title = "登录失败",
                        Content = "登录失败，密码已修改或账号已注销",
                        PrimaryButtonText = "确定"
                    };
                    contentDialog.PrimaryButtonClick += (_sender, _e) =>
                    {
                        contentDialog.Hide();
                    };
                    await contentDialog.ShowAsync();
                }
            }
        }

        private void SignupHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(SignupPage));
        }

        private async void ForgotPwdHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var userId = SQLiteHelper.GetSigninInfo().SI_UserID;
                var url = HttpClientHelper.baseUrl + "user/query/" + userId;
                var resp = await client.GetAsync(new Uri(url));
                if (resp.IsSuccessStatusCode)
                {
                    var userInfo = JObject.Parse(await resp.Content.ReadAsStringAsync())
                            .ToObject<UserInfoDto>();
                    dynamic param = new
                    {
                        UserInfoDto = userInfo,
                        FromPage = "SigninPage"
                    };
                    var rootFrame = Window.Current.Content as Frame;
                    rootFrame.Navigate(typeof(PasswordChangePage), param);
                }
                else
                {
                    return;
                }
            }
        }
    }
}
