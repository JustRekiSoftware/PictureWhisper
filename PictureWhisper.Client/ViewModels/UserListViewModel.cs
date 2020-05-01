using Newtonsoft.Json.Linq;
using PictureWhisper.Client.Helper;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureWhisper.Client.ViewModels
{
    public class UserListViewModel
    {
        public ObservableCollection<UserDto> SearchResultUsers { get; set; }

        public ObservableCollection<UserDto> FollowUsers { get; set; }

        public UserListViewModel()
        {
            SearchResultUsers = new ObservableCollection<UserDto>();
            FollowUsers = new ObservableCollection<UserDto>();
        }

        public async Task GetSearchResultUsersAsync(string queryData, int page, int pageSize)
        {
            if (page == 1)
            {
                SearchResultUsers.Clear();
            }
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var url = string.Format("{0}user/query/{1}/{2}/{3}",
                    HttpClientHelper.baseUrl, queryData, page, pageSize);
                var response = await client.GetAsync(new Uri(url));
                if (!response.IsSuccessStatusCode)
                {
                    return;
                }
                var users = JArray.Parse(
                    await response.Content.ReadAsStringAsync());
                var result = users.ToObject<List<UserInfoDto>>();
                if (result == null)
                {
                    return;
                }
                foreach (var user in result)
                {
                    url = HttpClientHelper.baseUrl
                        + "download/picture/small/" + user.U_Avatar;
                    var image = await ImageHelper.GetImageAsync(client, url);
                    var userDto = new UserDto
                    {
                        UserInfo = user,
                        UserAvatar = image
                    };
                    await GetIsFollowAsync(userDto);
                    this.SearchResultUsers.Add(userDto);
                }
            }
        }

        public async Task GetFollowUsersAsync(int id, int page, int pageSize)
        {
            if (page == 1)
            {
                FollowUsers.Clear();
            }
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var url = string.Format("{0}follow/{1}/{2}/{3}",
                    HttpClientHelper.baseUrl, id, page, pageSize);
                var response = await client.GetAsync(new Uri(url));
                if (!response.IsSuccessStatusCode)
                {
                    return;
                }
                var users = JArray.Parse(
                    await response.Content.ReadAsStringAsync());
                var result = users.ToObject<List<UserInfoDto>>();
                if (result == null)
                {
                    return;
                }
                foreach (var user in result)
                {
                    url = HttpClientHelper.baseUrl
                        + "download/picture/small/" + user.U_Avatar;
                    var image = await ImageHelper.GetImageAsync(client, url);
                    var userDto = new UserDto
                    {
                        UserInfo = user,
                        UserAvatar = image
                    };
                    await GetIsFollowAsync(userDto);
                    this.FollowUsers.Add(userDto);
                }
            }
        }

        public async Task GetIsFollowAsync(UserDto user)
        {
            var userId = SQLiteHelper.GetSigninInfo().SI_UserID;
            if (userId == user.UserInfo.U_ID)
            {
                user.IsFollow = false;
            }
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                var url = HttpClientHelper.baseUrl +
                    "follow/" + userId + "/" + user.UserInfo.U_ID;
                var isFollow = bool.Parse(await client.GetStringAsync(new Uri(url)));

                user.IsFollow = isFollow;
            }
        }

        public void FillInfo()
        {
            foreach (var user in SearchResultUsers)
            {
                if (user.IsFollow)
                {
                    user.FollowButtonText = "已关注 ";
                }
                else
                {
                    user.FollowButtonText = "+  关注 " + FormatFollowNum(user.UserInfo.U_FollowerNum);
                }
                user.FollowedTextBlockText = FormatFollowNum(user.UserInfo.U_FollowedNum);
            }
            foreach (var user in FollowUsers)
            {
                if (user.IsFollow)
                {
                    user.FollowButtonText = "已关注 ";
                }
                else
                {
                    user.FollowButtonText = "+  关注 " + FormatFollowNum(user.UserInfo.U_FollowerNum);
                }
                user.FollowedTextBlockText = FormatFollowNum(user.UserInfo.U_FollowedNum);
            }
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
