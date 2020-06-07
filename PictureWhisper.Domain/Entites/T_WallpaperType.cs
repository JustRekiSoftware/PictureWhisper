using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PictureWhisper.Domain.Entites
{
    /// <summary>
    /// 壁纸分区
    /// </summary>
    [Table("T_WallpaperType")]
    public class T_WallpaperType
    {
        [Key]
        public short WT_ID { get; set; }

        [Required]
        [MaxLength(8)]
        public string WT_Name { get; set; }
    }
}
