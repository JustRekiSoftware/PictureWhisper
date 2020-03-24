using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.ViewManagement;

namespace PictureWhisper.Client.Helpers
{
    public class ColorHelper
    {
        public static Color GetAccentColor()
        {
            var uiSetting = new UISettings();

            return uiSetting.GetColorValue(UIColorType.Accent);
        }

        public static Color GetBackgroudColor()
        {
            var uiSetting = new UISettings();

            return uiSetting.GetColorValue(UIColorType.Background);
        }

        public static Color GetForegroudColor()
        {
            var uiSetting = new UISettings();

            return uiSetting.GetColorValue(UIColorType.Foreground);
        }

        public static Color GetGray()
        {
            var value = System.Drawing.Color.Gray.ToArgb();
            var bytes = BitConverter.GetBytes(value);

            return Color.FromArgb(bytes[0], bytes[1], bytes[2], bytes[3]);
        }
    }
}
