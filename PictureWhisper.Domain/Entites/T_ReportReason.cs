using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PictureWhisper.Domain.Entites
{
    /// <summary>
    /// 举报理由
    /// </summary>
    [Table("T_ReportReason")]
    public class T_ReportReason
    {
        [Key]
        public short RR_ID { get; set; }

        [Required]
        [MaxLength(16)]
        public string RR_Info { get; set; }
    }
}
