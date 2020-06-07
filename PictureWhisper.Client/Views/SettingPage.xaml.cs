using PictureWhisper.Client.BackgroundTask;
using PictureWhisper.Client.Domain.Entities;
using PictureWhisper.Client.Helper;
using System.Text;
using Windows.ApplicationModel.Background;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PictureWhisper.Client.Views
{
    /// <summary>
    /// 设置页面
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

        /// <summary>
        /// 点击自动设置壁纸切换按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AutoSetWallpaperToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (!StatusChange)//不响应自动切换
            {
                return;
            }
            SettingInfo.STI_AutoSetWallpaper = !SettingInfo.STI_AutoSetWallpaper;
            await SQLiteHelper.UpdateSettingInfoAsync(SettingInfo);//更新设置
            if (SettingInfo.STI_AutoSetWallpaper)
            {
                await BackgroundTaskHelper.RegisterBackgroundTaskAsync(
                    typeof(AutoSetWallpaperTask),
                    typeof(AutoSetWallpaperTask).Name,
                    new TimeTrigger(15, false),
                    null,
                    true);//启动后台任务
                AutoSetWallpaperTextBlock.Text = "后台任务已启动";
                AutoSetWallpaperTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                BackgroundTaskHelper.UnRegisterBackgroundTask(typeof(AutoSetWallpaperTask).Name);//停止后台任务
                AutoSetWallpaperTextBlock.Text = "后台任务已停止";
                AutoSetWallpaperTextBlock.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// 导航到该页面时的事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            AutoSetWallpaperTextBlock.Visibility = Visibility.Collapsed;
            var settingInfo = SQLiteHelper.GetSettingInfo();
            if (settingInfo != null)
            {
                SettingInfo = settingInfo;
            }
            StatusChange = false;
            if (SettingInfo.STI_AutoSetWallpaper)//切换初始值
            {
                AutoSetWallpaperToggleSwitch.IsOn = true;
            }
            else
            {
                AutoSetWallpaperToggleSwitch.IsOn = false;
            }
            StatusChange = true;
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("图语");
            builder.AppendLine("JustReki Software");
            AboutText = builder.ToString();
            base.OnNavigatedTo(e);
        }
    }
}
