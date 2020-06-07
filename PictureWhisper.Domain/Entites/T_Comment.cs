using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PictureWhisper.Domain.Entites
{
    /// <summary>
    /// 评论
    /// </summary>
    [Table("T_Comment")]
    public class T_Comment
    {
        [Key]
        public int C_ID { get; set; }

        [Required]
        public int C_PublisherID { get; set; }

        [Required]
        public int C_ReceiverID { get; set; }

        [Required]
        public int C_WallpaperID { get; set; }

        [Required]
        [MaxLength(256)]
        public string C_Content { get; set; }

        [Required]
        public int C_ReplyNum { get; set; }

        [Required]
        [Column(TypeName = "datetime")]
        public DateTime C_Date { get; set; }

        [Required]
        public short C_Status { get; set; }
    }
}
