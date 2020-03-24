using PictureWhisper.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PictureWhisper.Domain.Entites
{
    [Table("T_Wallpaper")]
    public class T_Wallpaper : BindableBase
    {
        private int id;
        [Key]
        public int W_ID
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }

        private int publisherId;
        [Required]
        public int W_PublisherID
        {
            get { return publisherId; }
            set { SetProperty(ref publisherId, value); }
        }

        private string location;
        [Required]
        [MaxLength(128)]
        public string W_Location
        {
            get { return location; }
            set { SetProperty(ref location, value); }
        }

        private string title;
        [Required]
        [MaxLength(128)]
        public string W_Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        private string story;
        [Required]
        [MaxLength(1024)]
        public string W_Story
        {
            get { return story; }
            set { SetProperty(ref story, value); }
        }

        private short type;
        [Required]
        public short W_Type
        {
            get { return type; }
            set { SetProperty(ref type, value); }
        }

        private string tag;
        [Required]
        [MaxLength(128)]
        public string W_Tag
        {
            get { return tag; }
            set { SetProperty(ref tag, value); }
        }

        private int likeNum;
        [Required]
        public int W_LikeNum
        {
            get { return likeNum; }
            set { SetProperty(ref likeNum, value); }
        }

        private int favoriteNum;
        [Required]
        public int W_FavoriteNum
        {
            get { return favoriteNum; }
            set { SetProperty(ref favoriteNum, value); }
        }

        private DateTime date;
        [Required]
        [Column(TypeName = "datetime")]
        public DateTime W_Date
        {
            get { return date; }
            set { SetProperty(ref date, value); }
        }

        private short status;
        [Required]
        public short W_Status
        {
            get { return status; }
            set { SetProperty(ref status, value); }
        }
    }
}
