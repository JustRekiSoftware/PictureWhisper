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
    public class CommentDto : BindableBase
    {
        private T_Comment commentInfo;
        public T_Comment CommentInfo
        {
            get { return commentInfo; }
            set { SetProperty(ref commentInfo, value); }
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

        private string allReplyHyperlinkButtonDisplayText;
        public string AllReplyHyperlinkButtonDisplayText
        {
            get { return allReplyHyperlinkButtonDisplayText; }
            set { SetProperty(ref allReplyHyperlinkButtonDisplayText, value); }
        }
    }
}
