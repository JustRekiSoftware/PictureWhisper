using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace PictureWhisper.Client.Helper
{
    public class HttpClientHelper
    {
        private static DateTime lastAuthorize = DateTime.Now;
        private static string accessToken = string.Empty;
        // token过期时间(s)
        private static readonly int expireTime = 600;

        public static readonly string baseUrl = "https://localhost:5001/api/";

        public async static Task<HttpClient> GetAuthorizedHttpClientAsync()
        {
            var client = new HttpClient();
            if (accessToken == string.Empty || (DateTime.Now - lastAuthorize).TotalSeconds > expireTime)
            {
                accessToken = await GetAccessTokenAsync();
                if (accessToken == string.Empty)
                {
                    throw new Exception("请求错误，无法获取AccessToken！");
                }
            }
            client.DefaultRequestHeaders.Add("Authorization", accessToken);

            return client;
        }

        private async static Task<string> GetAccessTokenAsync()
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                // 请求发现文档
                var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5000/");
                if (disco.IsError)
                {
                    return string.Empty;
                }
                // 请求token
                var tokenResponse = await client
                    .RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = disco.TokenEndpoint,
                    ClientId = "PictureWhisperClient",
                    ClientSecret = "SnVzdFJla2kgU29mdHdhcmU=",
                    Scope = "PictureWhisperWebAPI"
                });
                if (tokenResponse.IsError)
                {
                    return string.Empty;
                }
                lastAuthorize = DateTime.Now;

                return "Bearer " + tokenResponse.AccessToken;
            }
        }

        public static void StartUpdateAccessTokenTask()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    accessToken = await GetAccessTokenAsync();

                    Thread.Sleep((expireTime - 60) * 1000);
                }
            });
        }
    }
}
