using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace User.Identity
{
    public class Config
    {
        /// <summary>
        /// 预置允许验证的Client
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client()
                {
                    ClientId="android",
                    ClientSecrets=new List<Secret>
                    {
                        new Secret("secret".Sha256())
                    },
                    RefreshTokenExpiration=TokenExpiration.Sliding,
                    AllowOfflineAccess=true,
                    RequireClientSecret=false,
                    AllowedGrantTypes=new List<string>{"sms_auth_code"},
                    AlwaysIncludeUserClaimsInIdToken=true,
                     //AllowAccessTokensViaBrowser=true,
                     //AllowRememberConsent=true,
                    AllowedScopes=new List<string>
                    {
                        "gateway_api",
                        "contact_api",
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                         IdentityServerConstants.StandardScopes.OpenId,
                          IdentityServerConstants.StandardScopes.Profile,
                    }
                }
            };
        }
        /// <summary>
        /// 配置身份资源
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Email(),
                new IdentityResources.Profile()
            };
        }
        /// <summary>
        /// 配置API资源
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
               new ApiResource("gateway_api","user service"),
               new ApiResource("contact_api","contact service")
            };
        }
        //public static List<TestUser> GetTestUsers()
        //{
        //}
    }
}
