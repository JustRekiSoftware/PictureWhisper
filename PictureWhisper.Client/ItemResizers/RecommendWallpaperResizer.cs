using Marduk.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace PictureWhisper.Client.ItemResizers
{
    public class RecommendWallpaperResizer : IItemResizer
    {
        public Size Resize(object item, Size oldSize, Size availableSize)
        {
            return new Size(availableSize.Width, oldSize.Height);
        }
    }
}
