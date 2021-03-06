﻿using PictureWhisper.Client.ViewModels;
using PictureWhisper.Domain.Entites;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PictureWhisper.Client.Views
{
    /// <summary>
    /// 壁纸搜索结果页面
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
            NavigationCacheMode = NavigationCacheMode.Enabled;//启用缓存
        }

        /// <summary>
        /// 点击壁纸
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WallpaperAdaptiveGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var wallpaperDto = (WallpaperDto)e.ClickedItem;
            var rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(WallpaperMainPage), wallpaperDto.WallpaperInfo);
        }

        /// <summary>
        /// 滑动到底部时自动加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void WallpaperScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
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
        /// 改变分区筛选条件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void TypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count == 1)
            {
                PageNum = 1;
                await LoadSearchResultAsync(PageNum++);
            }
        }

        /// <summary>
        /// 改变排序筛选条件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OrderbyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count == 1)
            {
                PageNum = 1;
                await LoadSearchResultAsync(PageNum++);
            }
        }

        /// <summary>
        /// 点击显示/隐藏搜索条件超链接按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 导航到该页面时的事件
        /// </summary>
        /// <param name="e"></param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (MainPage.Page != null)
            {
                MainPage.Page.HyperLinkButtonFocusChange("WallpaperSearchResultHyperlinkButton");
            }
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
                if (WallpaperTypeLVM.WallpaperTypes.FirstOrDefault(p => p.WT_Name == "全部") == null)
                {
                    WallpaperTypeLVM.WallpaperTypes.Add(wallpaperType);
                }
                TypeComboBox.SelectedIndex = WallpaperTypeLVM.WallpaperTypes.Count - 1;//最后一个分区筛选条件为全部
                OrderbyComboBox.SelectedIndex = 0;
                await LoadSearchResultAsync(PageNum++);
            }
            ChangeDesiredWidth(WallpaperAdaptiveGridView.ActualWidth);
            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// 加载搜索结果
        /// </summary>
        /// <param name="page">页数</param>
        /// <returns></returns>
        private async Task LoadSearchResultAsync(int page)
        {
            await WallpaperLVM.GetSearchResultWallpapersAsync(Keyword,
                (short)TypeComboBox.SelectedValue, (string)OrderbyComboBox.SelectedValue, page, PageSize);
        }

        /// <summary>
        /// WallpaperAdaptiveGridView的大小改变时的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WallpaperAdaptiveGridView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ChangeDesiredWidth(e.NewSize.Width);
        }

        /// <summary>
        /// 改变WallpaperAdaptiveGridView的物品期望宽度
        /// </summary>
        /// <param name="width">WallpaperAdaptiveGridView的宽度</param>
        public void ChangeDesiredWidth(double width)
        {
            var colCount = 1;
            //计算列数
            if (width >= 1900)
            {
                colCount = 5;
            }
            else if (width >= 1400)
            {
                colCount = 4;
            }
            else if (width >= 1000)
            {
                colCount = 3;
            }
            else if (width >= 600)
            {
                colCount = 2;
            }
            if (WallpaperLVM.SearchResultWallpapers.Count > 0
                && colCount > WallpaperLVM.SearchResultWallpapers.Count)//要显示的物品少于列数
            {
                colCount = WallpaperLVM.SearchResultWallpapers.Count;
            }
            var desiredWidth = width / colCount;
            WallpaperAdaptiveGridView.ItemHeight = desiredWidth * (1080.0 / 1920.0);
            WallpaperAdaptiveGridView.DesiredWidth = desiredWidth;
        }
    }
}
