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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PictureWhisper.Client.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UserSearchResultPage : Page
    {
        private UserListViewModel UserLVM { get; set; }
        private readonly int PageSize = 20;
        private int PageNum { get; set; }
        private string Keyword { get; set; }

        public UserSearchResultPage()
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
                await LoadSearchResultAsync(PageNum++);
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
            await LoadSearchResultAsync(PageNum++);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                Keyword = (string)e.Parameter;
                PageNum = 1;
                await LoadSearchResultAsync(PageNum++);
            }
            base.OnNavigatedTo(e);
        }

        private async Task LoadSearchResultAsync(int page)
        {
            await UserLVM.GetSearchResultUsersAsync(Keyword, page, PageSize);
        }
    }
}
