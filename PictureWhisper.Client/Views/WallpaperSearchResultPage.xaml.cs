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
    public sealed partial class WallpaperSearchResultPage : Page
    {
        private WallpaperListViewModel WallpaperLVM { get; set; }
        private WallpaperTypeListViewModel WallpaperTypeLVM { get; set; }
        private WallpaperSearchOrderViewModel WallpaperSearchOrderLVM { get; set; }
        private readonly int PageSize = 20;
        private int PageNum { get; set; }
        private string Keyword { get; set; }

        public WallpaperSearchResultPage()
        {
            WallpaperLVM = new WallpaperListViewModel();
            WallpaperTypeLVM = new WallpaperTypeListViewModel();
            WallpaperSearchOrderLVM = new WallpaperSearchOrderViewModel();
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private void WallpaperGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var wallpaperDto = (WallpaperDto)e.ClickedItem;
            var rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(WallpaperMainPage), wallpaperDto.WallpaperInfo);
        }

        private async void WallpaperScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;
            if (scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
            {
                await LoadSearchResultAsync(PageNum++);
            }
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            PageNum = 1;
            await LoadSearchResultAsync(PageNum++);
        }

        private async void TypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count == 1)
            {
                PageNum = 1;
                await LoadSearchResultAsync(PageNum++);
            }
        }

        private async void OrderbyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count == 1)
            {
                PageNum = 1;
                await LoadSearchResultAsync(PageNum++);
            }
        }

        private void HideSearchOddsHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            if (SearchOddsStackPanel.Visibility == Visibility.Visible)
            {
                SearchOddsStackPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                SearchOddsStackPanel.Visibility = Visibility.Visible;
            }
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                Keyword = (string)e.Parameter;
                PageNum = 1;
                await WallpaperTypeLVM.GetWallpaperTypesAsync();
                var wallpaperType = new T_WallpaperType
                {
                    WT_ID = 0,
                    WT_Name = "全部"
                };
                if (!WallpaperTypeLVM.WallpaperTypes.Contains(wallpaperType))
                {
                    WallpaperTypeLVM.WallpaperTypes.Add(wallpaperType);
                }
                TypeComboBox.SelectedIndex = WallpaperTypeLVM.WallpaperTypes.Count - 1;
                OrderbyComboBox.SelectedIndex = 0;
                await LoadSearchResultAsync(PageNum++);
            }
            base.OnNavigatedTo(e);
        }

        private async Task LoadSearchResultAsync(int page)
        {
            await WallpaperLVM.GetSearchResultWallpapersAsync(Keyword,
                (short)TypeComboBox.SelectedValue, (string)OrderbyComboBox.SelectedValue, page, PageSize);
        }
    }
}
