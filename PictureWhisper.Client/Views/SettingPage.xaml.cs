using PictureWhisper.Client.BackgroundTask;
using PictureWhisper.Client.Helper;
using PictureWhisper.Client.Domain.Entities;
using System;
using System.IO;
using System.Text;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
        private bool StatusChange { get; set; }
        private string AboutText { get; set; }

        public SettingPage()
        {
            this.InitializeComponent();
        }

        private async void AutoSetWallpaperToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (!StatusChange)
            {
                return;
            }
            SettingInfo.STI_AutoSetWallpaper = !SettingInfo.STI_AutoSetWallpaper;
            await SQLiteHelper.UpdateSettingInfoAsync(SettingInfo);
            if (SettingInfo.STI_AutoSetWallpaper)
            {
                await BackgroundTaskHelper.RegisterBackgroundTaskAsync(
                    typeof(AutoSetWallpaperTask),
                    typeof(AutoSetWallpaperTask).Name,
                    new TimeTrigger(15, false),
                    null,
                    true);
                AutoSetWallpaperTextBlock.Text = "后台任务已启动";
                AutoSetWallpaperTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                BackgroundTaskHelper.UnRegisterBackgroundTask(typeof(AutoSetWallpaperTask).Name);
                AutoSetWallpaperTextBlock.Text = "后台任务已停止";
                AutoSetWallpaperTextBlock.Visibility = Visibility.Visible;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            AutoSetWallpaperTextBlock.Visibility = Visibility.Collapsed;
            var settingInfo = SQLiteHelper.GetSettingInfo();
            if (settingInfo != null)
            {
                SettingInfo = settingInfo;
            }
            if (SettingInfo.STI_AutoSetWallpaper)
            {
                StatusChange = false;
                AutoSetWallpaperToggleSwitch.IsOn = true;
                StatusChange = true;
            }
            else
            {
                StatusChange = false;
                AutoSetWallpaperToggleSwitch.IsOn = false;
                StatusChange = true;
            }
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("图语");
            builder.AppendLine("JustReki Software");
            AboutText = builder.ToString();
            base.OnNavigatedTo(e);
        }
    }
}
