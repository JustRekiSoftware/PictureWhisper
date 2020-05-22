using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PictureWhisper.IdentityServer
{
    /// <summary>
    /// IdentityServer配置
    /// </summary>
    public class Config
    {
        //配置客户端
        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                new Client
                {
                    ClientId = "PictureWhisperClient",//客户端识别名
                    AccessTokenLifetime = 600,//Token有效时间
                    AllowedGrantTypes = GrantTypes.ClientCredentials,//使用客户端认证进行身份验证
                    ClientSecrets = new List<Secret>//加密验证
                    {
                        new Secret("SnVzdFJla2kgU29mdHdhcmU=".Sha256())
                    },
                    AllowedScopes = new List<string>//客户端可以访问的范围
                    {
                        "PictureWhisperWebAPI"
                    }
                }
            };
        //配置Web Api
        public static IEnumerable<ApiResource> Apis =>
            new List<ApiResource>
            {
                new ApiResource("PictureWhisperWebAPI", "PictureWhisper ASP.NET Core Web API")//添加WebAPI识别名
            };
    }
}
