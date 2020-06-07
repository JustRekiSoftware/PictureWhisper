using PictureWhisper.Domain.Abstract;
using PictureWhisper.Domain.Entites;

namespace PictureWhisper.Client.ViewModels
{
    /// <summary>
    /// 审核结果的显示类
    /// </summary>
    public class ReviewDto : BindableBase
    {
        private T_Review reviewInfo;
        public T_Review ReviewInfo
        {
            get { return reviewInfo; }
            set { SetProperty(ref reviewInfo, value); }
        }

        public string ReviewTitleText
        {
            get
            {
                var toReturn = string.Empty;
                toReturn += reviewInfo.RV_Type == (short)ReviewType.壁纸审核 ?
                    "壁纸审核" : "举报审核";
                toReturn += reviewInfo.RV_Result ? "处理已通过" : "处理未通过";

                return toReturn;
            }
        }

        private string reviewDisplayText;
        public string ReviewDisplayText
        {
            get { return reviewDisplayText; }
            set { SetProperty(ref reviewDisplayText, value); }
        }
    }
}
