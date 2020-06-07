using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PictureWhisper.Client.Domain.Entities
{
    /// <summary>
    /// 用户登录信息
    /// </summary>
    [Table("T_SigninInfo")]
    public class T_SigninInfo
    {
        [Key]
        public int SI_ID { get; set; }

        public int SI_UserID { get; set; }

        [MaxLength(32)]
        public string SI_Email { get; set; }

        [MaxLength(64)]
        public string SI_Password { get; set; }

        [MaxLength(128)]
        public string SI_Avatar { get; set; }

        public short SI_Type { get; set; }

        public short SI_Status { get; set; }
    }
}
