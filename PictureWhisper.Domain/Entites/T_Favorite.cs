using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PictureWhisper.Domain.Entites
{
    /// <summary>
    /// 收藏
    /// </summary>
    [Table("T_Favorite")]
    public class T_Favorite
    {
        [Key]
        public int FVRT_ID { get; set; }

        [Required]
        public int FVRT_WallpaperID { get; set; }

        [Required]
        public int FVRT_FavoritorID { get; set; }

        [Required]
        [Column(TypeName = "datetime")]
        public DateTime FVRT_Date { get; set; }
    }
}
