using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using PictureWhisper.Client.Domain.Concrete;
using PictureWhisper.Client.Domain.Entities;
using PictureWhisper.Client.Helpers;
using PictureWhisper.Client.ViewModels;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
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

namespace PictureWhisper.Client
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SignupPage : Page
    {
        private WallpaperTypeListViewModel WallpaperTypeLVM;
        private List<string> TagPickResult;

        public SignupPage()
        {
            this.InitializeComponent();
            WallpaperTypeLVM = new WallpaperTypeListViewModel();
            TagPickResult = new List<string>();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            TagPickerProgressRing.IsActive = true;
            TagPickerProgressRing.Visibility = Visibility.Visible;

            await WallpaperTypeLVM.GetWallpaperTypesAsync();

            TagPickerProgressRing.IsActive = false;
            TagPickerProgressRing.Visibility = Visibility.Collapsed;
        }

        private async void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            var userInfo = new T_User();
            ErrorMsgTextBlock.Text = "错误信息：" + Environment.NewLine;
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var checkUrl = HttpClientHelper.baseUrl + "user/email/" + EmailTextBox.Text;
                if (EmailTextBox.Text != string.Empty && EmailTextBox.Text.Contains("@"))
                {
                    var resp = await client.GetAsync(new Uri(checkUrl));
                    if (!resp.IsSuccessStatusCode)
                    {
                        ErrorMsgTextBlock.Text += "· 邮箱已存在" + Environment.NewLine;
                    }
                    else
                    {
                        userInfo.U_Email = EmailTextBox.Text;
                    }
                }
                else
                {
                    ErrorMsgTextBlock.Text += "· 邮箱未输入或邮箱格式错误" + Environment.NewLine;
                }
                checkUrl = HttpClientHelper.baseUrl + "user/name/" + NameTextBox.Text;
                if (NameTextBox.Text != string.Empty)
                {
                    var resp = await client.GetAsync(new Uri(checkUrl));
                    if (!resp.IsSuccessStatusCode)
                    {
                        ErrorMsgTextBlock.Text += "· 昵称已存在" + Environment.NewLine;
                    }
                    else
                    {
                        userInfo.U_Name = NameTextBox.Text;
                    }
                }
                else
                {
                    ErrorMsgTextBlock.Text += "· 昵称未输入" + Environment.NewLine;
                }
                if (PwdPasswordBox.Password.TrimEnd()
                    == PwdRepeatPasswordBox.Password.TrimEnd() 
                    && PwdPasswordBox.Password.TrimEnd() != string.Empty)
                {
                    userInfo.U_Password =
                        EncryptHelper.SHA256Encrypt(PwdPasswordBox.Password.TrimEnd());
                }
                else
                {
                    ErrorMsgTextBlock.Text += "· 两次密码输入不同或未输入密码" + Environment.NewLine;
                }
                if (TagPickResult.Count != 0)
                {
                    var builder = new StringBuilder();
                    foreach (var tag in TagPickResult)
                    {
                        builder.Append(tag + " ");
                    }
                    userInfo.U_Tag = builder.ToString();
                }
                else
                {
                    userInfo.U_Tag = string.Empty;
                }
                if (ErrorMsgTextBlock.Text.Contains("·"))
                {
                    ErrorMsgTextBlock.Visibility = Visibility.Visible;
                    return;
                }
                var url = HttpClientHelper.baseUrl + "user";
                var content = new HttpStringContent(JObject.FromObject(userInfo).ToString());
                content.Headers.ContentType = new HttpMediaTypeHeaderValue("application/json");
                var response = await client.PostAsync(new Uri(url), content);
                if (!response.IsSuccessStatusCode)
                {
                    var contentDialog = new ContentDialog
                    {
                        Title = "注册失败",
                        Content = "注册失败，请检查网络连接后重试",
                        PrimaryButtonText = "关闭"
                    };
                    contentDialog.PrimaryButtonClick += (_sender, _e) =>
                    {
                        contentDialog.Hide();
                    };
                    await contentDialog.ShowAsync();
                }
                else
                {
                    var rootFrame = Window.Current.Content as Frame;
                    rootFrame.Navigate(typeof(SigninPage));
                }
            }
        }

        private void TagPicker_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as T_WallpaperType;
            if (!TagPickResult.Contains(item.WT_Name))
            {
                TagPickResult.Add(item.WT_Name);
            }
            else
            {
                TagPickResult.Remove(item.WT_Name);
            }
        }
    }
}
