using Newtonsoft.Json.Linq;
using PictureWhisper.Client.Helper;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PictureWhisper.Client.Views
{
    /// <summary>
    /// 更改密码页面
    /// </summary>
    public sealed partial class PasswordChangePage : Page
    {
        private int UserId { get; set; }
        private string Code { get; set; }
        private string FromPage { get; set; }

        public PasswordChangePage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 点击发送验证码按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SendIdentifyCodeButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorMessageTextBlock.Text = "错误信息：" + Environment.NewLine;
            var userInfo = SQLiteHelper.GetSigninInfo();
            //验证邮箱是否输入、格式是否正确
            if (EmailTextBox.Text != string.Empty
                && EmailTextBox.Text.Contains("@"))
            {
                ErrorMessageTextBlock.Visibility = Visibility.Collapsed;
            }
            else if (userInfo.SI_Email != EmailTextBox.Text)
            {
                ErrorMessageTextBlock.Text += "· 邮箱输入错误" + Environment.NewLine;
                ErrorMessageTextBlock.Visibility = Visibility.Visible;
                return;
            }
            else
            {
                ErrorMessageTextBlock.Text += "· 邮箱未输入或格式不正确" + Environment.NewLine;
                ErrorMessageTextBlock.Visibility = Visibility.Visible;
                return;
            }
            SendIdentifyCodeButton.IsEnabled = false;
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var url = HttpClientHelper.baseUrl + "user/identify/" + UserId
                    + "/" + EmailTextBox.Text;
                var resp = await client.GetAsync(new Uri(url));//发送获取验证码请求
                if (resp.IsSuccessStatusCode)//获取成功
                {
                    SendIdentifyCodeButton.Content = "已发送";
                    dynamic identifyInfo = JObject.Parse(await resp.Content.ReadAsStringAsync())
                            .ToObject<dynamic>();
                    if (UserId == 0)//忘记密码
                    {
                        UserId = (int)identifyInfo.userId;
                    }
                    else if (UserId != (int)identifyInfo.userId)//修改密码，邮箱非注册邮箱
                    {
                        ErrorMessageTextBlock.Text += "· 发送失败，请检查邮箱是否正确" + Environment.NewLine;
                        ErrorMessageTextBlock.Visibility = Visibility.Visible;
                        return;
                    }
                    Code = (string)identifyInfo.code;//验证码
                }
                else
                {
                    ErrorMessageTextBlock.Text += "· 发送失败，请检查邮箱是否正确" + Environment.NewLine;
                    ErrorMessageTextBlock.Visibility = Visibility.Visible;
                    return;
                }
            }
            var count = 60;
            while (count > 0)
            {
                await Task.Run(() =>
                {
                    Thread.Sleep(1000);//等待1秒
                });
                SendIdentifyCodeButton.Content = count-- + "秒后可再次发送";
            }
            SendIdentifyCodeButton.IsEnabled = true;
            SendIdentifyCodeButton.Content = "发送验证码到注册邮箱";
        }

        /// <summary>
        /// 点击完成按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorMessageTextBlock.Text = "错误信息：" + Environment.NewLine;
            if (Code == string.Empty)
            {
                ErrorMessageTextBlock.Text += "· 验证码未输入" + Environment.NewLine;
            }
            else
            {
                if (Code == IdentifyCodeTextBox.Text)//验证码正确
                {
                    //检查输入是否正确
                    if (NewPwdTextBox.Password.TrimEnd() != string.Empty
                        && RepeatNewPwdtextBox.Password.TrimEnd() != string.Empty
                        && NewPwdTextBox.Password.TrimEnd() == RepeatNewPwdtextBox.Password.TrimEnd())
                    {
                        var pwd = EncryptHelper.SHA256Encrypt(NewPwdTextBox.Password.TrimEnd());//加密密码
                        using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
                        {
                            var url = HttpClientHelper.baseUrl + "user/" + UserId;
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
                            var resp = await client.SendRequestAsync(request);//发送修改密码请求
                            if (resp.IsSuccessStatusCode)//修改成功
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

        /// <summary>
        /// 点击取消按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Code = string.Empty;
            NewPwdTextBox.Password = string.Empty;
            RepeatNewPwdtextBox.Password = string.Empty;
            if (FromPage == "SigninPage")
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

        /// <summary>
        /// 导航到该页面的事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (UserMainPage.Page != null)
            {
                UserMainPage.Page.HyperLinkButtonFocusChange("PasswordChangeHyperlinkButton");
            }
            if (e.Parameter != null)
            {
                UserId = (int)((dynamic)e.Parameter).UserId;
                FromPage = (string)((dynamic)e.Parameter).FromPage;
            }
            if (Code != null || Code != string.Empty)
            {
                Code = string.Empty;
            }
            base.OnNavigatedTo(e);
        }
    }
}
