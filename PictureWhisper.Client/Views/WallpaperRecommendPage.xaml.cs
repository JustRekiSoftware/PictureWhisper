using PictureWhisper.Client.Helpers;
using PictureWhisper.Client.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class WallpaperRecommendPage : Page
    {
        public RecommendWallpaperListViewModel RecommendLVM { get; set; }
        public int UserId { get; set; }

        public WallpaperRecommendPage()
        {
            RecommendLVM = new RecommendWallpaperListViewModel();
            this.InitializeComponent();
            UserId = SQLiteHelper.GetSigninInfo().SI_UserID;
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private void ImageButton_Click(object sender, RoutedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;
            var wallpaper = (WallpaperDto)((Button)sender).DataContext;
            rootFrame.Navigate(typeof(WallpaperMainPage), wallpaper.WallpaperInfo);
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RecommendLVM.PageNum = 1;
            await RecommendLVM.GetRecommendWallpapersAsync(UserId);
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            RecommendLVM.PageNum = 1;
            await RecommendLVM.GetRecommendWallpapersAsync(UserId);
        }
    }
}
