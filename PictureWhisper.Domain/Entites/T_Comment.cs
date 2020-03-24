using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PictureWhisper.Domain.Entites
{
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
