using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PictureWhisper.IdentityServer
{
    public class Config
    {
        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                new Client
                {
                    ClientId = "PictureWhisperClient",

                    AccessTokenLifetime = 600,

                    // 使用clientid / secret进行身份验证
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    // 加密验证
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("SnVzdFJla2kgU29mdHdhcmU=".Sha256())
                    },

                    // client可以访问的范围
                    AllowedScopes = new List<string>
                    {
                        "PictureWhisperWebAPI"
                    }
                }
            };

        public static IEnumerable<ApiResource> Apis =>
            new List<ApiResource>
            {
                new ApiResource("PictureWhisperWebAPI", "PictureWhisper ASP.NET Core Web API")
            };
    }
}
