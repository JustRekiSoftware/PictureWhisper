using PictureWhisper.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PictureWhisper.Domain.Entites
{
    /// <summary>
    /// 举报
    /// </summary>
    [Table("T_Report")]
    public class T_Report : BindableBase
    {
        private int id;
        [Key]
        public int RPT_ID
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }

        private int reporterId;
        [Required]
        public int RPT_ReporterID
        {
            get { return reporterId; }
            set { SetProperty(ref reporterId, value); }
        }

        private short type;
        [Required]
        public short RPT_Type
        {
            get { return type; }
            set { SetProperty(ref type, value); }
        }

        private int reportedId;
        [Required]
        public int RPT_ReportedID
        {
            get { return reportedId; }
            set { SetProperty(ref reportedId, value); }
        }

        private short reason;
        [Required]
        public short RPT_Reason
        {
            get { return reason; }
            set { SetProperty(ref reason, value); }
        }

        private string additional;
        [Required]
        [MaxLength(256)]
        public string RPT_Additional
        {
            get { return additional; }
            set { SetProperty(ref additional, value); }
        }

        private DateTime date;
        [Required]
        [Column(TypeName = "datetime")]
        public DateTime RPT_Date
        {
            get { return date; }
            set { SetProperty(ref date, value); }
        }

        private short status;
        [Required]
        public short RPT_Status
        {
            get { return status; }
            set { SetProperty(ref status, value); }
        }
    }
}
