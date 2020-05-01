﻿using PictureWhisper.Domain.Entites;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PictureWhisper.Client.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WallpaperStroyPage : Page
    {
        private T_Wallpaper WallpaperInfo { get; set; }

        public WallpaperStroyPage()
        {
            WallpaperInfo = new T_Wallpaper();
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                var wallpaper = (T_Wallpaper)e.Parameter;
                if (WallpaperInfo.W_ID != wallpaper.W_ID)
                {
                    WallpaperInfo = wallpaper;
                }
            }
            base.OnNavigatedTo(e);
        }
    }
}
