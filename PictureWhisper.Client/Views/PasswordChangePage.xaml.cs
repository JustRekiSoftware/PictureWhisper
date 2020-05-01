using Newtonsoft.Json.Linq;
using PictureWhisper.Client.Domain.Entities;
using PictureWhisper.Client.Helper;
using PictureWhisper.Domain.Entites;
using PictureWhisper.Domain.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class PasswordChangePage : Page
    {
        private UserInfoDto UserInfo { get; set; }
        private T_SigninInfo SigninInfo { get; set; }
        private string Code { get; set; }
        private string FromPage { get; set; }

        public PasswordChangePage()
        {
            this.InitializeComponent();
        }

        private async void SendIdentifyCodeButton_Click(object sender, RoutedEventArgs e)
        {
            SendIdentifyCodeButton.IsEnabled = false;
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var url = HttpClientHelper.baseUrl + "user/identify/" + UserInfo.U_Name
                    + "/" + SigninInfo.SI_Email;
                var resp = await client.GetAsync(new Uri(url));
                if (resp.IsSuccessStatusCode)
                {
                    SendIdentifyCodeButton.Content = "已发送";
                    Code = await resp.Content.ReadAsStringAsync();
                }
            }
            await Task.Run(() =>
            {
                Thread.Sleep(60000);
            });
            SendIdentifyCodeButton.IsEnabled = true;
            SendIdentifyCodeButton.Content = "发送验证码到注册邮箱";
        }

        private async void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorMessageTextBlock.Text = "错误信息：" + Environment.NewLine;
            if (Code == string.Empty)
            {
                ErrorMessageTextBlock.Text += "· 验证码未输入" + Environment.NewLine;
            }
            else
            {
                if (Code == IdentifyCodeTextBox.Text)
                {
                    if (NewPwdTextBox.Password.TrimEnd() != string.Empty 
                            && RepeatNewPwdtextBox.Password.TrimEnd() != string.Empty
                            && NewPwdTextBox.Password.TrimEnd() == RepeatNewPwdtextBox.Password.TrimEnd())
                    {
                        var pwd = EncryptHelper.SHA256Encrypt(NewPwdTextBox.Password.TrimEnd());
                        using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
                        {
                            var url = HttpClientHelper.baseUrl + "user/" + SigninInfo.SI_UserID;
                            var userOp = new List<dynamic>();
                            userOp.Add(new
                            {
                                op = "replace",
                                path = "U_Password",
                                value = pwd
                            });
                            var content = new HttpStringContent(JArray.FromObject(userOp).ToString());
                            content.Headers.ContentType = new HttpMediaTypeHeaderValue("application/json-patch+json");
                            var request = new HttpRequestMessage(HttpMethod.Patch, new Uri(url))
                            {
                                Content = content
                            };
                            var resp = await client.SendRequestAsync(request);
                            if (resp.IsSuccessStatusCode)
                            {
                                if (UserMainPage.PageFrame.CanGoBack)
                                {
                                    UserMainPage.PageFrame.GoBack();
                                }
                            }
                            else
                            {
                                ErrorMessageTextBlock.Text += "· 修改失败" + Environment.NewLine;
                            }
                        }
                    }
                    else
                    {
                        ErrorMessageTextBlock.Text += "· 新密码未输入或两次密码输入不一致" + Environment.NewLine;
                    }
                }
                else
                {
                    ErrorMessageTextBlock.Text += "· 验证码输入错误" + Environment.NewLine;
                }
            }
            if (ErrorMessageTextBlock.Text.Contains("·"))
            {
                ErrorMessageTextBlock.Visibility = Visibility.Visible;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Code = string.Empty;
            NewPwdTextBox.Password = string.Empty;
            RepeatNewPwdtextBox.Password = string.Empty;
            if (FromPage == "SigininPage")
            {
                var rootFrame = Window.Current.Content as Frame;
                if (rootFrame.CanGoBack)
                {
                    rootFrame.GoBack();
                }
            }
            else if (FromPage == "UserMainPage")
            {
                if (UserMainPage.PageFrame.CanGoBack)
                {
                    UserMainPage.PageFrame.GoBack();
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                UserInfo = ((dynamic)e.Parameter).UserInfoDto;
                FromPage = ((dynamic)e.Parameter).FromPage;
                SigninInfo = SQLiteHelper.GetSigninInfo();
                if (UserInfo.U_ID != SigninInfo.SI_UserID)
                {
                    return;
                }
            }
            if (Code != null || Code != string.Empty)
            {
                Code = string.Empty;
            }
            base.OnNavigatedTo(e);
        }
    }
}
