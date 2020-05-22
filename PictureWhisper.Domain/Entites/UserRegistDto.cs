using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PictureWhisper.Domain.Entites
{
    /// <summary>
    /// 用户注册信息
    /// </summary>
    public class UserRegistDto
    {
        [MaxLength(32)]
        public string U_Email { get; set; }

        [MaxLength(16)]
        public string U_Name { get; set; }

        [MaxLength(64)]
        public string U_Password { get; set; }

        [MaxLength(128)]
        public string U_Tag { get; set; }
    }
}
