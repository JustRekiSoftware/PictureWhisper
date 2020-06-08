using System;
using Windows.UI;
using Windows.UI.ViewManagement;

namespace PictureWhisper.Client.Helper
{
    /// <summary>
    /// 颜色帮助类
    /// </summary>
    public class ColorHelper
    {
        /// <summary>
        /// 获取主题色
        /// </summary>
        /// <returns></returns>
        public static Color GetAccentColor()
        {
            var uiSetting = new UISettings();

            return uiSetting.GetColorValue(UIColorType.Accent);
        }

        /// <summary>
        /// 获取背景色
        /// </summary>
        /// <returns></returns>
        public static Color GetBackgroudColor()
        {
            var uiSetting = new UISettings();

            return uiSetting.GetColorValue(UIColorType.Background);
        }

        /// <summary>
        /// 获取前景色
        /// </summary>
        /// <returns></returns>
        public static Color GetForegroudColor1()
        {
            var uiSetting = new UISettings();

            return uiSetting.GetColorValue(UIColorType.Foreground);
        }

        /// <summary>
        /// 获取前景色
        /// </summary>
        /// <returns></returns>
        public static Color GetHyperLinkButtonForegroundColor()
        {
            return Colors.Gray;
        }

        /// <summary>
        /// 获取消息提示色
        /// </summary>
        /// <returns></returns>
        public static Color GetMessageNotifyColor()
        {
            return Colors.Aqua;
        }
    }
}
