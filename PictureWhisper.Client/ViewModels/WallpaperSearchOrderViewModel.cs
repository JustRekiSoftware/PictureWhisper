using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace PictureWhisper.Client.ViewModels
{
    public class WallpaperSearchOrderViewModel
    {
        public ObservableCollection<OrderbyInfo> OrderbyInfos { get; set; }

        public WallpaperSearchOrderViewModel()
        {
            OrderbyInfos = new ObservableCollection<OrderbyInfo>();

            OrderbyInfos.Add(new OrderbyInfo
            {
                Text = "时间",
                Value = "date"
            });
            OrderbyInfos.Add(new OrderbyInfo
            {
                Text = "点赞",
                Value = "like"
            });
            OrderbyInfos.Add(new OrderbyInfo
            {
                Text = "收藏",
                Value = "favorite"
            });
        }

        public class OrderbyInfo
        {
            public string Text { get; set; }

            public string Value { get; set; }
        }
    }
}
