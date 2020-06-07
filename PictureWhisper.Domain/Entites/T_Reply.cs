using PictureWhisper.Domain.Abstract;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PictureWhisper.Domain.Entites
{
    /// <summary>
    /// 回复
    /// </summary>
    [Table("T_Relpy")]
    public class T_Reply : BindableBase
    {
        private int id;
        [Key]
        public int RPL_ID
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }

        private int publisherId;
        [Required]
        public int RPL_PublisherID
        {
            get { return publisherId; }
            set { SetProperty(ref publisherId, value); }
        }

        private int reciverId;
        [Required]
        public int RPL_ReceiverID
        {
            get { return reciverId; }
            set { SetProperty(ref reciverId, value); }
        }

        private int commentId;
        [Required]
        public int RPL_CommentID
        {
            get { return commentId; }
            set { SetProperty(ref commentId, value); }
        }

        private string content;
        [Required]
        [MaxLength(256)]
        public string RPL_Content
        {
            get { return content; }
            set { SetProperty(ref content, value); }
        }

        private DateTime date;
        [Required]
        [Column(TypeName = "datetime")]
        public DateTime RPL_Date
        {
            get { return date; }
            set { SetProperty(ref date, value); }
        }

        private short status;
        [Required]
        public short RPL_Status
        {
            get { return status; }
            set { SetProperty(ref status, value); }
        }
    }
}
