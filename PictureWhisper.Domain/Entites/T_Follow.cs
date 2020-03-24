using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PictureWhisper.Domain.Entites
{
    [Table("T_Follow")]
    public class T_Follow
    {
        [Key]
        public int FLW_ID { get; set; }

        [Required]
        public int FLW_FollowerID { get; set; }

        [Required]
        public int FLW_FollowedID { get; set; }

        [Required]
        [Column(TypeName = "datetime")]
        public DateTime FLW_Date { get; set; }
    }
}
