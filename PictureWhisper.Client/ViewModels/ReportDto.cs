using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace PictureWhisper.Client.ViewModels
{
    public class ReportDto
    {
        public T_Report ReportInfo { get; set; }

        public BitmapImage Image { get; set; }

        public string DisplayText { get; set; }

        public int MessageToId { get; set; }

        public string ReportReason { get; set; }
    }
}
