using PictureWhisper.Domain.Abstract;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace PictureWhisper.Client.ViewModels
{
    /// <summary>
    /// 回复的显示类
    /// </summary>
    public class ReplyDto : BindableBase
    {
        private T_Reply replyInfo;
        public T_Reply ReplyInfo
        {
            get { return replyInfo; }
            set { SetProperty(ref replyInfo, value); }
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

        private Visibility deleteButtonVisibility;
        public Visibility DeleteButtonVisibility
        {
            get { return deleteButtonVisibility; }
            set { SetProperty(ref deleteButtonVisibility, value); }
        }
    }
}
