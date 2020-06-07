using Newtonsoft.Json.Linq;
using PictureWhisper.Client.Helper;
using PictureWhisper.Client.ViewModels;
using PictureWhisper.Domain.Entites;
using System;
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
    /// 关注页面
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

        /// <summary>
        /// 滑动到底部自动加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void UserScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;
            if (scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
            {
                await LoadFollowUsersAsync(PageNum++);
            }
        }

        /// <summary>
        /// 点击用户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var userDto = (UserDto)e.ClickedItem;
            var rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(UserMainPage), userDto.UserInfo);
        }

        /// <summary>
        /// 点击刷新按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            PageNum = 1;
            await LoadFollowUsersAsync(PageNum++);
        }

        /// <summary>
        /// 点击用户头像
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AvatarButton_Click(object sender, RoutedEventArgs e)
        {
            var userDto = (UserDto)((Button)sender).DataContext;
            var rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(UserMainPage), userDto.UserInfo);
        }

        /// <summary>
        /// 点击关注按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                if (userDto.IsFollow)//关注
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
                else//取消关注
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
            UserLVM.FillInfo();//补充关注信息
        }

        /// <summary>
        /// 导航到该页面的事件
        /// </summary>
        /// <param name="e"></param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (UserMainPage.Page != null)
            {
                UserMainPage.Page.HyperLinkButtonFocusChange("FollowHyperlinkButton");
            }
            if (e.Parameter != null)
            {
                UserId = (int)e.Parameter;
                PageNum = 1;
                await LoadFollowUsersAsync(PageNum++);
            }
            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// 加载关注用户
        /// </summary>
        /// <param name="page">页数</param>
        /// <returns></returns>
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
