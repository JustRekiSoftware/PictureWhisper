using PictureWhisper.Domain.Abstract;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace PictureWhisper.Client.ViewModels
{
    public class WallpaperDto : BindableBase
    {
        private T_Wallpaper wallpaperInfo;
        public T_Wallpaper WallpaperInfo
        {
            get { return wallpaperInfo; }
            set { SetProperty(ref wallpaperInfo, value); }
        }

        private UserInfoDto publisherInfo;
        public UserInfoDto PublisherInfo
        {
            get { return publisherInfo; }
            set { SetProperty(ref publisherInfo, value); }
        }

        private BitmapImage publisherAvatar;
        public BitmapImage PublisherAvatar
        {
            get { return publisherAvatar; }
            set { SetProperty(ref publisherAvatar, value); }
        }

        private BitmapImage image;
        public BitmapImage Image
        {
            get { return image; }
            set { SetProperty(ref image, value); }
        }
    }
}
