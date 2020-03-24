﻿using PictureWhisper.Domain.Abstract;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace PictureWhisper.Client.ViewModels
{
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