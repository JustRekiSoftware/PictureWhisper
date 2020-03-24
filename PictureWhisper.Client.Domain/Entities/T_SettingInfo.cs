using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PictureWhisper.Client.Domain.Entities
{
    [Table("T_SettingInfo")]
    public class T_SettingInfo : BindableBase
    {
        private int id;
        [Key]
        public int STI_ID
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }

        private bool autoSetWallpaper;
        [Required]
        public bool STI_AutoSetWallpaper
        {
            get { return autoSetWallpaper; }
            set { SetProperty(ref autoSetWallpaper, value); }
        }
    }
}
