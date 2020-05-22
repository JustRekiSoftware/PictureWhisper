using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PictureWhisper.Client.Domain.Entities
{
    /// <summary>
    /// 历史浏览记录
    /// </summary>
    [Table("T_HistoryInfo")]
    public class T_HistoryInfo
    {
        [Key]
        public int HI_ID { get; set; }

        [Required]
        public int HI_WallpaperID { get; set; }
    }
}
