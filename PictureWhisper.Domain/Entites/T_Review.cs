using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PictureWhisper.Domain.Entites
{
    /// <summary>
    /// 审核
    /// </summary>
    [Table("T_Review")]
    public class T_Review
    {
        [Key]
        public int RV_ID { get; set; }

        [Required]
        public int RV_ReviewerID { get; set; }

        [Required]
        public short RV_Type { get; set; }

        [Required]
        public int RV_ReviewedID { get; set; }

        [Required]
        public bool RV_Result { get; set; }

        [Required]
        public int RV_MsgToReporterID { get; set; }

        [Required]
        public int RV_MsgToReportedID { get; set; }

        [Required]
        [Column(TypeName = "datetime")]
        public DateTime RV_Date { get; set; }
    }
}
