﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PictureWhisper.Domain.Entites
{
    /// <summary>
    /// 用户
    /// </summary>
    [Table("T_User")]
    public class T_User
    {
        [Key]
        public int U_ID { get; set; }

        [Required]
        [MaxLength(32)]
        public string U_Email { get; set; }

        [Required]
        [MaxLength(16)]
        public string U_Name { get; set; }

        [Required]
        [MaxLength(64)]
        public string U_Password { get; set; }

        [Required]
        [MaxLength(256)]
        public string U_Info { get; set; }

        [Required]
        [MaxLength(128)]
        public string U_Tag { get; set; }

        [Required]
        [MaxLength(128)]
        public string U_Avatar { get; set; }

        [Required]
        public int U_FollowedNum { get; set; }

        [Required]
        public int U_FollowerNum { get; set; }

        [Required]
        public short U_Type { get; set; }

        [Required]
        public short U_Status { get; set; }
    }
}
