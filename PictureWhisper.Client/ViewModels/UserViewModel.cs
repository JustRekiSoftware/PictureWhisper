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
    /// <summary>
    /// 用户信息的ViewModel
    /// </summary>
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

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <returns></returns>
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
                await GetIsFollowAsync(User.UserInfo.U_ID);
                User.FollowButtonText = "+ 关注（" + FormatFollowNum(User.UserInfo.U_FollowerNum) + "）";
                User.FollowedTextBlockText = FormatFollowNum(User.UserInfo.U_FollowedNum);
            }
        }

        /// <summary>
        /// 获取头像
        /// </summary>
        /// <param name="imagePath">头像下载路径</param>
        /// <returns></returns>
        public async Task GetAvatarAsync(string imagePath)
        {
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var url = HttpClientHelper.baseUrl +
                    "download/picture/origin/" + imagePath;
                User.UserAvatar = await ImageHelper.GetImageAsync(client, url);
            }
        }

        /// <summary>
        /// 获取是否已关注
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <returns></returns>
        public async Task GetIsFollowAsync(int id)
        {
            var userId = SQLiteHelper.GetSigninInfo().SI_UserID;
            if (userId == id)
            {
                User.IsFollow = false;
            }
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var url = HttpClientHelper.baseUrl +
                    "follow/" + userId + "/" + id;
                var isFollow = bool.Parse(await client.GetStringAsync(new Uri(url)));

                User.IsFollow = isFollow;
            }
        }

        /// <summary>
        /// 补充用户关注信息
        /// </summary>
        public void FillInfo()
        {
            User.FollowButtonText = "+  关注 " + FormatFollowNum(User.UserInfo.U_FollowerNum);
            User.FollowedTextBlockText = FormatFollowNum(User.UserInfo.U_FollowedNum);
        }

        /// <summary>
        /// 格式化关注数
        /// </summary>
        /// <param name="num">关注数</param>
        /// <returns>返回格式化后的关注数</returns>
        public string FormatFollowNum(int num)
        {
            var numStr = num.ToString();
            if (numStr.Length <= 3)//1000以内
            {
                return numStr;
            }
            else if (numStr.Length == 4)//10000以内
            {
                return numStr[0] + "," + numStr.Substring(1);
            }
            else//10000以上
            {
                return numStr.Substring(0, numStr.Length - 4) + "." + numStr[numStr.Length - 4] + "万";
            }
        }
    }
}
