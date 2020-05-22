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
        /// 获取更亮的主题色
        /// </summary>
        /// <returns></returns>
        public static Color GetLighterAccentColor()
        {
            var uiSetting = new UISettings();

            return uiSetting.GetColorValue(UIColorType.AccentLight2);
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
        public static Color GetForegroudColor()
        {
            var uiSetting = new UISettings();

            return uiSetting.GetColorValue(UIColorType.Foreground);
        }

        /// <summary>
        /// 根据System.Drawing.Color的颜色获取Windows.UI.Color
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color GetColor(System.Drawing.Color color)
        {
            var value = color.ToArgb();
            var bytes = BitConverter.GetBytes(value);

            return Color.FromArgb(bytes[0], bytes[1], bytes[2], bytes[3]);
        }
    }
}
