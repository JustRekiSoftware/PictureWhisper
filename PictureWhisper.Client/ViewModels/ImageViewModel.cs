using PictureWhisper.Domain.Abstract;
using Windows.UI.Xaml.Media.Imaging;

namespace PictureWhisper.Client.ViewModels
{
    /// <summary>
    /// 图片的ViewModel
    /// </summary>
    public class ImageViewModel : BindableBase
    {
        private BitmapImage image;
        public BitmapImage Image
        {
            get { return image; }
            set { SetProperty(ref image, value); }
        }

        public ImageViewModel()
        {
            image = new BitmapImage();
        }
    }
}
