using Newtonsoft.Json.Linq;
using PictureWhisper.Client.Helper;
using PictureWhisper.Client.ViewModels;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class FollowPage : Page
    {
        private UserListViewModel UserLVM { get; set; }
        private readonly int PageSize = 20;
        private int PageNum { get; set; }
        private int UserId { get; set; }

        public FollowPage()
        {
            UserLVM = new UserListViewModel();
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private async void UserScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;
            if (scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
            {
                await LoadFollowUsersAsync(PageNum++);
            }
        }

        private void UserGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var userDto = (UserDto)e.ClickedItem;
            var rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(UserMainPage), userDto.UserInfo);
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            PageNum = 1;
            await LoadFollowUsersAsync(PageNum++);
        }

        private void AvatarButton_Click(object sender, RoutedEventArgs e)
        {
            var userDto = (UserDto)((Button)sender).DataContext;
            var rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(UserMainPage), userDto.UserInfo);
        }

        private async void UIDFollowButton_Click(object sender, RoutedEventArgs e)
        {
            var userDto = (UserDto)((Button)sender).DataContext;
            if (UserId == userDto.UserInfo.U_ID)
            {
                return;
            }
            userDto.IsFollow = !userDto.IsFollow;
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                if (userDto.IsFollow)
                {
                    var url = HttpClientHelper.baseUrl + "follow";
                    var followInfo = new T_Follow();
                    followInfo.FLW_FollowerID = UserId;
                    followInfo.FLW_FollowedID = userDto.UserInfo.U_ID;
                    var content = new HttpStringContent(JObject.FromObject(followInfo).ToString());
                    content.Headers.ContentType = new HttpMediaTypeHeaderValue("application/json");
                    var resp = await client.PostAsync(new Uri(url), content);
                    if (!resp.IsSuccessStatusCode)
                    {
                        userDto.IsFollow = !userDto.IsFollow;
                    }
                    else
                    {
                        userDto.UserInfo.U_FollowerNum++;
                    }
                }
                else
                {
                    var url = HttpClientHelper.baseUrl + "follow/" + UserId + "/" +
                        userDto.UserInfo.U_ID;
                    var resp = await client.DeleteAsync(new Uri(url));
                    if (!resp.IsSuccessStatusCode)
                    {
                        userDto.IsFollow = !userDto.IsFollow;
                    }
                    else
                    {
                        userDto.UserInfo.U_FollowerNum--;
                    }
                }
            }
            if (userDto.IsFollow)
            {
                userDto.FollowButtonText = "已关注";
            }
            else
            {
                UserLVM.FillInfo();
            }
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                UserId = (int)e.Parameter;
                PageNum = 1;
                await LoadFollowUsersAsync(PageNum++);
            }
            base.OnNavigatedTo(e);
        }

        private async Task LoadFollowUsersAsync(int page)
        {
            await UserLVM.GetFollowUsersAsync(UserId, page, PageSize);
            if (UserLVM.FollowUsers.Count > 0)
            {
                UserLVM.FillInfo();
            }
        }
    }
}
