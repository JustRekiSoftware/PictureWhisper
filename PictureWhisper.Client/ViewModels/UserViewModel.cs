using Newtonsoft.Json.Linq;
using PictureWhisper.Client.Helper;
using PictureWhisper.Domain.Abstract;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureWhisper.Client.ViewModels
{
    public class UserViewModel : BindableBase
    {
        private UserDto user;
        public UserDto User
        {
            get { return user; }
            set { SetProperty(ref user, value); }
        }

        public UserViewModel()
        {
            User = new UserDto();
        }

        public async Task GetUserAsync(int id)
        {
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var url = HttpClientHelper.baseUrl + "user/" + id;
                var response = await client.GetAsync(new Uri(url));
                if (!response.IsSuccessStatusCode)
                {
                    return;
                }
                var wallpaper = JObject.Parse(
                    await response.Content.ReadAsStringAsync());
                var result = wallpaper.ToObject<UserInfoDto>();
                if (result == null)
                {
                    return;
                }
                User.UserInfo = result;
                url = HttpClientHelper.baseUrl +
                    "download/picture/origin/" + User.UserInfo.U_Avatar;
                User.UserAvatar = await ImageHelper.GetImageAsync(client, url);
                User.FollowButtonText = "关注（" + User.UserInfo.U_FollowerNum + "）";
                User.FollowedTextBlockText = "ta的关注" 
                    + Environment.NewLine + User.UserInfo.U_FollowedNum;
            }
        }

        public async Task GetAvatarAsync(string imagePath)
        {
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var url = HttpClientHelper.baseUrl +
                    "download/picture/origin/" + imagePath;
                User.UserAvatar = await ImageHelper.GetImageAsync(client, url);
            }
        }

        public void FillInfo()
        {
            User.FollowButtonText = "+  关注 " + FormatFollowNum(User.UserInfo.U_FollowerNum);
            User.FollowedTextBlockText = FormatFollowNum(User.UserInfo.U_FollowedNum);
        }

        public string FormatFollowNum(int num)
        {
            var numStr = num.ToString();
            if (numStr.Length <= 3)
            {
                return numStr;
            }
            else if (numStr.Length == 4)
            {
                return numStr[0] + "," + numStr.Substring(1);
            }
            else
            {
                return numStr.Substring(0, numStr.Length - 4) + "." + numStr[numStr.Length - 4] + "万";
            }
        }
    }
}
