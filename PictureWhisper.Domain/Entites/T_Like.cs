using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PictureWhisper.Domain.Entites
{
    /// <summary>
    /// 点赞
    /// </summary>
    [Table("T_Like")]
    public class T_Like
    {
        [Key]
        public int L_ID { get; set; }

        [Required]
        public int L_WallpaperID { get; set; }

        [Required]
        public int L_LikerID { get; set; }

        [Required]
        [Column(TypeName = "datetime")]
        public DateTime L_Date { get; set; }
    }
}
