using PictureWhisper.Domain.Abstract;
using PictureWhisper.Domain.Entites;
using Windows.UI.Xaml.Media.Imaging;

namespace PictureWhisper.Client.ViewModels
{
    /// <summary>
    /// 用户信息的显示类
    /// </summary>
    public class UserDto : BindableBase
    {
        private UserInfoDto userInfo;
        public UserInfoDto UserInfo
        {
            get { return userInfo; }
            set { SetProperty(ref userInfo, value); }
        }

        private BitmapImage userAvatar;
        public BitmapImage UserAvatar
        {
            get { return userAvatar; }
            set { SetProperty(ref userAvatar, value); }
        }

        private bool isFollow;
        public bool IsFollow
        {
            get { return isFollow; }
            set { SetProperty(ref isFollow, value); }
        }

        private string followButtonText;
        public string FollowButtonText
        {
            get { return followButtonText; }
            set { SetProperty(ref followButtonText, value); }
        }

        private string followedTextBlockText;
        public string FollowedTextBlockText
        {
            get { return followedTextBlockText; }
            set { SetProperty(ref followedTextBlockText, value); }
        }
    }
}
