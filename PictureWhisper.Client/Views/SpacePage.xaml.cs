using PictureWhisper.Client.Helpers;
using PictureWhisper.Client.ViewModels;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PictureWhisper.Client.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SpacePage : Page
    {
        private WallpaperListViewModel WallpaperLVM { get; set; }
        private int CurrentIndex { get; set; }
        private ImageViewModel ImageVM { get; set; }
        private readonly int PageSize = 20;
        private int PageNum { get; set; }
        private int UserId { get; set; }

        public SpacePage()
        {
            WallpaperLVM = new WallpaperListViewModel();
            ImageVM = new ImageViewModel();
            this.InitializeComponent();
        }

        private async void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentIndex == 0)
            {
                PageNum = 1;
                await LoadSpaceWallpapersAsync(PageNum++);
            }
            if (CurrentIndex > 0)
            {
                PrevFontIcon.Glyph = "&#xE70E;";
                CurrentIndex--;
                ImageVM.Image = WallpaperLVM.SpaceWallpapers[CurrentIndex].Image;
            }
            if (CurrentIndex == 0)
            {
                PrevFontIcon.Glyph = "&#xE72C;";
            }
        }

        private async void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentIndex < WallpaperLVM.SpaceWallpapers.Count)
            {
                CurrentIndex++;
                ImageVM.Image = WallpaperLVM.SpaceWallpapers[CurrentIndex].Image;
            }
            if (CurrentIndex > WallpaperLVM.SpaceWallpapers.Count - 5)
            {
                await LoadSpaceWallpapersAsync(PageNum++);
            }
        }

        private void WallpaperImageButton_Click(object sender, RoutedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(WallpaperMainPage), 
                WallpaperLVM.SpaceWallpapers[CurrentIndex].WallpaperInfo);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            UserId = SQLiteHelper.GetSigninInfo().SI_UserID;
            PageNum = 1;
            CurrentIndex = 0;
            await LoadSpaceWallpapersAsync(PageNum++);
            base.OnNavigatedTo(e);
        }

        private async Task LoadSpaceWallpapersAsync(int page)
        {
            await WallpaperLVM.GetSpaceWallpapersAsync(UserId, page, PageSize);
        }
    }
}
