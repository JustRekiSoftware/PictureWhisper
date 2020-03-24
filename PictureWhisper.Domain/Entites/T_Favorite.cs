using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PictureWhisper.Domain.Entites
{
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
