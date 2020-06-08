using Newtonsoft.Json.Linq;
using PictureWhisper.Client.BackgroundTask;
using PictureWhisper.Client.Domain.Entities;
using PictureWhisper.Client.Helper;
using PictureWhisper.Client.Views;
using PictureWhisper.Domain.Entites;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PictureWhisper.Client
{
    /// <summary>
    /// 登录页面
    /// </summary>
    public sealed partial class SigninPage : Page
    {
        public SigninPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 点击登录按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            await SigninNow();
        }

        /// <summary>
        /// 点击注册超链接按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SignupHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(SignupPage));
        }

        /// <summary>
        /// 点击忘记密码超链接按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ForgotPwdHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            dynamic param = new
            {
                UserId = 0,
                FromPage = "SigninPage"
            };//配置跳转参数
            var rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(PasswordChangePage), param);
        }

        /// <summary>
        /// 跳转到该页面的事件
        /// </summary>
        /// <param name="e"></param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null && (string)e.Parameter != string.Empty)
            {
                var signinSuccess = (bool)e.Parameter;
                if (!signinSuccess)//自动登录失败
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
            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// 拦截邮箱输入框回车事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void EmailTextBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                await SigninNow();
            }
        }

        /// <summary>
        /// 拦截密码输入框回车事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void PwdPasswordBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                await SigninNow();
            }
        }

        /// <summary>
        /// 执行登录
        /// </summary>
        private async Task SigninNow()
        {
            ErrorMsgTextBlock.Text = "错误信息：" + Environment.NewLine;
            //输入格式验证
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
            ErrorMsgTextBlock.Visibility = Visibility.Collapsed;
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                //登录
                var pwd = EncryptHelper.SHA256Encrypt(PwdPasswordBox.Password.TrimEnd());//加密登录密码
                var url = HttpClientHelper.baseUrl + "user/signin/"
                    + EmailTextBox.Text + "/" + pwd;
                var resp = await client.GetAsync(new Uri(url));
                if (resp.IsSuccessStatusCode)//登录成功
                {
                    var userSigninDto = JObject.Parse(await resp.Content.ReadAsStringAsync())
                            .ToObject<UserSigninDto>();
                    var rootFrame = Window.Current.Content as Frame;
                    var signinInfo = SQLiteHelper.GetSigninInfo();
                    if (signinInfo != null)//更新登录信息
                    {
                        signinInfo.SI_UserID = userSigninDto.U_ID;
                        signinInfo.SI_Email = EmailTextBox.Text;
                        signinInfo.SI_Password = pwd;
                        signinInfo.SI_Avatar = userSigninDto.U_Avatar;
                        signinInfo.SI_Type = userSigninDto.U_Type;
                        signinInfo.SI_Status = userSigninDto.U_Status;
                        await SQLiteHelper.UpdateSigninInfoAsync(signinInfo);
                    }
                    else//添加登录信息
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
                    if (settingInfo == null)//添加默认设置信息
                    {
                        settingInfo = new T_SettingInfo();
                        settingInfo.STI_AutoSetWallpaper = false;
                        settingInfo.STI_LastCheckMessageDate = DateTime.Now;
                        await SQLiteHelper.AddSettingInfoAsync(settingInfo);
                    }
                    else
                    {
                        if (settingInfo.STI_AutoSetWallpaper)//启用自动设置壁纸
                        {
                            await BackgroundTaskHelper
                                .RegisterBackgroundTaskAsync(
                                typeof(AutoSetWallpaperTask),
                                typeof(AutoSetWallpaperTask).Name,
                                new TimeTrigger(60, false),
                                null,
                                true);
                        }
                    }
                    if (userSigninDto.U_Type == (short)UserType.注册用户)//页面跳转
                    {
                        rootFrame.Navigate(typeof(MainPage));
                    }
                    else
                    {
                        rootFrame.Navigate(typeof(ReviewMainPage));
                    }
                }
                else//登录失败
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
    }
}
