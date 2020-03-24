using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureWhisper.Client.ViewModels
{
    public class ReviewDto
    {
        public T_Review ReviewInfo { get; set; }

        public string ReviewDisplayText { get; set; }
    }
}
