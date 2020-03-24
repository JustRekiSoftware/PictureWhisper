using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PictureWhisper.Domain.Entites
{
    public class UserSigninDto
    {
        public int U_ID { get; set; }

        [MaxLength(128)]
        public string U_Avatar { get; set; }

        public short U_Type { get; set; }

        public short U_Status { get; set; }
    }
}
