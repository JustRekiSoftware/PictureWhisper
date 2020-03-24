using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PictureWhisper.Client.Domain.Entities
{
    [Table("T_RecommendInfo")]
    public class T_RecommendInfo
    {
        [Key]
        public int RI_ID { get; set; }

        [Required]
        public int RI_Num { get; set; }
    }
}
