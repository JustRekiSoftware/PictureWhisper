﻿using PictureWhisper.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PictureWhisper.Domain.Entites
{
    public class UserInfoDto : BindableBase
    {
        private int id;
        public int U_ID 
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }

        private string name;
        [MaxLength(16)]
        public string U_Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        private string info;
        [MaxLength(256)]
        public string U_Info
        {
            get { return info; }
            set { SetProperty(ref info, value); }
        }

        private string avatar;
        [MaxLength(128)]
        public string U_Avatar
        {
            get { return avatar; }
            set { SetProperty(ref avatar, value); }
        }

        private int followedNum;
        [Required]
        public int U_FollowedNum
        {
            get { return followedNum; }
            set { SetProperty(ref followedNum, value); }
        }

        private int followerNum;
        [Required]
        public int U_FollowerNum
        {
            get { return followerNum; }
            set { SetProperty(ref followerNum, value); }
        }
    }
}
