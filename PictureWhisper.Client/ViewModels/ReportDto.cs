using PictureWhisper.Domain.Abstract;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace PictureWhisper.Client.ViewModels
{
    public class ReportDto : BindableBase
    {
        private T_Report reportInfo;
        public T_Report ReportInfo
        {
            get { return reportInfo; }
            set { SetProperty(ref reportInfo, value); }
        }

        private BitmapImage image;
        public BitmapImage Image
        {
            get { return image; }
            set { SetProperty(ref image, value); }
        }

        private string displayText;
        public string DisplayText
        {
            get { return displayText; }
            set { SetProperty(ref displayText, value); }
        }

        private int messageToId;
        public int MessageToId
        {
            get { return messageToId; }
            set { SetProperty(ref messageToId, value); }
        }

        private string reportReason;
        public string ReportReason
        {
            get { return reportReason; }
            set { SetProperty(ref reportReason, value); }
        }
    }
}
