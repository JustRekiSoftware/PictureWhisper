using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PictureWhisper.Domain.Entites
{
    [Table("T_Relpy")]
    public class T_Reply
    {
        [Key]
        public int RPL_ID { get; set; }

        [Required]
        public int RPL_PublisherID { get; set; }

        [Required]
        public int RPL_ReceiverID { get; set; }

        [Required]
        public int RPL_CommentID { get; set; }

        [Required]
        [MaxLength(256)]
        public string RPL_Content { get; set; }

        [Required]
        [Column(TypeName = "datetime")]
        public DateTime RPL_Date { get; set; }

        [Required]
        public short RPL_Status { get; set; }
    }
}
