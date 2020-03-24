using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PictureWhisper.Domain.Entites
{
    public class UserInfoDto
    {
        public int U_ID { get; set; }

        [MaxLength(16)]
        public string U_Name { get; set; }

        [MaxLength(256)]
        public string U_Info { get; set; }

        [MaxLength(128)]
        public string U_Avatar { get; set; }

        [Required]
        public int U_FollowedNum { get; set; }

        [Required]
        public int U_FollowerNum { get; set; }
    }
}
