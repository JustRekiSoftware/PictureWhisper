using PictureWhisper.Domain.Abstract;
using PictureWhisper.Domain.Entites;
using Windows.UI.Xaml.Media.Imaging;

namespace PictureWhisper.Client.ViewModels
{
    /// <summary>
    /// 审核信息的ViewModel
    /// </summary>
    public class ReviewViewModel : BindableBase
    {
        private BitmapImage image;
        public BitmapImage Image
        {
            get { return image; }
            set { SetProperty(ref image, value); }
        }

        private T_Wallpaper wallpaper;
        public T_Wallpaper Wallpaper
        {
            get { return wallpaper; }
            set { SetProperty(ref wallpaper, value); }
        }

        private ReportDto reportDto;
        public ReportDto ReportDto
        {
            get { return reportDto; }
            set { SetProperty(ref reportDto, value); }
        }

        public ReviewViewModel()
        {
            image = new BitmapImage();
            wallpaper = new T_Wallpaper();
            reportDto = new ReportDto();
        }
    }
}
