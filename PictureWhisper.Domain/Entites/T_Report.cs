using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PictureWhisper.Domain.Entites
{
    [Table("T_Report")]
    public class T_Report
    {
        [Key]
        public int RPT_ID { get; set; }

        [Required]
        public int RPT_ReporterID { get; set; }

        [Required]
        public short RPT_Type { get; set; }

        [Required]
        public int RPT_ReportedID { get; set; }

        [Required]
        public short RPT_Reason { get; set; }

        [Required]
        [MaxLength(256)]
        public string RPT_Additional { get; set; }

        [Required]
        [Column(TypeName = "datetime")]
        public DateTime RPT_Date { get; set; }

        [Required]
        public short RPT_Status { get; set; }
    }
}
