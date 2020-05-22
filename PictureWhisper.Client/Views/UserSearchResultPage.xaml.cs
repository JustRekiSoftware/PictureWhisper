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
    /// 用户搜索结果
    /// </summary>
    public sealed partial class UserSearchResultPage : Page
    {
        private UserListViewModel UserLVM { get; set; }
        private int UserId;
        private readonly int PageSize = 20;
        private int PageNum { get; set; }
        private string Keyword { get; set; }

        public UserSearchResultPage()
        {
            UserLVM = new UserListViewModel();
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        /// <summary>
        /// 滑动到底部时自动加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void UserScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;
            if (scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
            {
                await LoadSearchResultAsync(PageNum++);
            }
        }

        /// <summary>
        /// 点击刷新按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            PageNum = 1;
            await LoadSearchResultAsync(PageNum++);
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
            UserLVM.FillInfo();
        }

        /// <summary>
        /// 导航到该页面时的事件
        /// </summary>
        /// <param name="e"></param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (MainPage.Page != null)
            {
                MainPage.Page.HyperLinkButtonFocusChange("UserSearchResultHyperlinkButton");
            }
            if (e.Parameter != null)
            {
                UserId = SQLiteHelper.GetSigninInfo().SI_UserID;
                Keyword = (string)e.Parameter;
                PageNum = 1;
                await LoadSearchResultAsync(PageNum++);
            }
            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// 加载用户搜索结果
        /// </summary>
        /// <param name="page">页数</param>
        /// <returns></returns>
        private async Task LoadSearchResultAsync(int page)
        {
            await UserLVM.GetSearchResultUsersAsync(Keyword, page, PageSize);
            if (UserLVM.SearchResultUsers.Count > 0)
            {
                UserLVM.FillInfo();
            }
        }
    }
}
