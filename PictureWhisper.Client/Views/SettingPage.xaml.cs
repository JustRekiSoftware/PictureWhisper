using PictureWhisper.Client.BackgroundTask.Tasks;
using PictureWhisper.Client.Domain.Entities;
using PictureWhisper.Client.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
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
    public sealed partial class SettingPage : Page
    {
        private T_SettingInfo SettingInfo { get; set; }
        private string AboutText { get; set; }

        public SettingPage()
        {
            SettingInfo = new T_SettingInfo();
            this.InitializeComponent();
        }

        private async void AutoSetWallpaperToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            await SQLiteHelper.UpdateSettingInfoAsync(SettingInfo);
            if (SettingInfo.STI_AutoSetWallpaper)
            {
                await BackgroundTask.Helpers.BackgroundTaskHelper.RegisterBackgroundTaskAsync(
                    typeof(AutoSetWallpaperTask),
                    typeof(AutoSetWallpaperTask).Name,
                    new TimeTrigger(60, false),
                    null,
                    true);
                AutoSetWallpaperTextBlock.Text = "后台任务已启动";
                AutoSetWallpaperTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                BackgroundTask.Helpers.BackgroundTaskHelper.UnRegisterBackgroundTask(typeof(AutoSetWallpaperTask).Name);
                AutoSetWallpaperTextBlock.Text = "后台任务已停止";
                AutoSetWallpaperTextBlock.Visibility = Visibility.Visible;
            }
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            AutoSetWallpaperTextBlock.Visibility = Visibility.Collapsed;
            SettingInfo = SQLiteHelper.GetSettingInfo();
            var path = Path.Combine(KnownFolders.PicturesLibrary.Path, "PictureWhisper");
            if (SettingInfo == null)
            {
                SettingInfo.STI_AutoSetWallpaper = false;
            }
            await SQLiteHelper.AddSettingInfoAsync(SettingInfo);
            SettingInfo = SQLiteHelper.GetSettingInfo();
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("图语");
            builder.AppendLine("JustReki Software");
            AboutText = builder.ToString();
            base.OnNavigatedTo(e);
        }
    }
}
