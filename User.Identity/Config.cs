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
                //new Client()
                //{
                //    ClientId="MVC",// 客户端的唯一ID
                //    ClientName="Mvc Client",
                //    ClientUri="http://localhost:5001",
                //    LogoUri="https://tse3-mm.cn.bing.net/th?id=OIP.xq1C2fmnSw5DEoRMC86vJwD6D6&w=198&h=189&c=7&o=5&pid=1.7",
                //    AllowedGrantTypes=GrantTypes.HybridAndClientCredentials,// 指定允许客户端使用的授权类型
                //    ClientSecrets=new List<Secret>{new Secret("secret".Sha256())},// 访问令牌端点的凭据
                //    RedirectUris={"http://localhost:5001/signin-oidc" },// 指定允许的URI以返回令牌或授权码
                //    PostLogoutRedirectUris={ "http://localhost:5001/signout-callback-oidc" },// 指定在注销后重定向到的允许URI
                //    AlwaysIncludeUserClaimsInIdToken=true,// 将用户相关信息夹在idtoken中去
                //    AllowOfflineAccess=true,// 指定此客户端是否可以请求刷新令牌（请求offline_access范围）
                //    AllowAccessTokensViaBrowser=true,// 指定是否允许此客户端通过浏览器接收访问令牌
                   
                //    AllowedScopes={
                //        IdentityServerConstants.StandardScopes.Profile,
                //        IdentityServerConstants.StandardScopes.OpenId,
                //        IdentityServerConstants.StandardScopes.Email,
                //        //"api"
                //    }// 默认情况下，客户端无权访问任何资源 - 通过添加相应的范围名称来指定允许的资源
                //},

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
                    AllowedGrantTypes=new List<string>{"sms_aut_code"},
                    AlwaysIncludeUserClaimsInIdToken=true,
                    AllowedScopes=new List<string>
                    {
                        "user_api",
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
               new ApiResource("user_api","user service")
            };
        }
        //public static List<TestUser> GetTestUsers()
        //{
        //}
    }
}
