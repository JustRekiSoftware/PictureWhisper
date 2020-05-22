using IdentityModel.Client;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace PictureWhisper.Client.Helper
{
    /// <summary>
    /// Http客户端帮助类
    /// </summary>
    public class HttpClientHelper
    {
        private static DateTime lastAuthorize = DateTime.Now;
        private static string accessToken = string.Empty;
        private static readonly int expireTime = 600;// token过期时间(s)
        public static readonly string baseUrl = "https://localhost:5001/api/";

        /// <summary>
        /// 获取含有验证信息的Http客户端
        /// </summary>
        /// <returns></returns>
        public async static Task<HttpClient> GetAuthorizedHttpClientAsync()
        {
            var client = new HttpClient();
            if (accessToken == string.Empty || 
                (DateTime.Now - lastAuthorize).TotalSeconds > expireTime)//验证信息为空或已过期
            {
                accessToken = await GetAccessTokenAsync();//获取AccessToken
                if (accessToken == string.Empty)
                {
                    throw new Exception("请求错误，无法获取AccessToken！");
                }
            }
            client.DefaultRequestHeaders.Add("Authorization", accessToken);//添加验证信息

            return client;
        }

        /// <summary>
        /// 获取AccessToken
        /// </summary>
        /// <returns>获取成功返回AccessToken，否则返回空字符串</returns>
        private async static Task<string> GetAccessTokenAsync()
        {
            using (var client = new System.Net.Http.HttpClient())//IdentityServer只实现了System.Net.Http.HttpClient的扩展函数
            {
                var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5000/");// 请求发现文档
                if (disco.IsError)
                {
                    return string.Empty;
                }
                var tokenResponse = await client
                    .RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = disco.TokenEndpoint,//Token请求地址
                    ClientId = "PictureWhisperClient",
                    ClientSecret = "SnVzdFJla2kgU29mdHdhcmU=",
                    Scope = "PictureWhisperWebAPI"
                });// 请求Token
                if (tokenResponse.IsError)
                {
                    return string.Empty;
                }
                lastAuthorize = DateTime.Now;

                return "Bearer " + tokenResponse.AccessToken;
            }
        }

        /// <summary>
        /// 启动更新AccessToken的Task
        /// </summary>
        public static void StartUpdateAccessTokenTask()
        {
            Task.Run(async () =>
            {
                while (true)//在Token要过期时进行更新
                {
                    accessToken = await GetAccessTokenAsync();
                    Thread.Sleep((expireTime - 60) * 1000);
                }
            });
        }
    }
}
