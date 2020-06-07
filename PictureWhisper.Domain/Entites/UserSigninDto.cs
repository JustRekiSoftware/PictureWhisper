using System.ComponentModel.DataAnnotations;

namespace PictureWhisper.Domain.Entites
{
    /// <summary>
    /// 用户登录信息
    /// </summary>
    public class UserSigninDto
    {
        public int U_ID { get; set; }

        [MaxLength(128)]
        public string U_Avatar { get; set; }

        public short U_Type { get; set; }

        public short U_Status { get; set; }
    }
}
