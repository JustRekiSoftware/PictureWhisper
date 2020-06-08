using PictureWhisper.Domain.Abstract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PictureWhisper.Domain.Entites
{
    /// <summary>
    /// 壁纸分区
    /// </summary>
    [Table("T_WallpaperType")]
    public class T_WallpaperType : BindableBase
    {
        private short id;
        [Key]
        public short WT_ID
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }

        private string name;
        [Required]
        [MaxLength(8)]
        public string WT_Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }
    }
}
