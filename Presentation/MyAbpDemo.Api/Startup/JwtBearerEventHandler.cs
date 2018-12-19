using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Abp.Runtime.Caching;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MyAbpDemo.Application;

namespace MyAbpDemo.Api
{
    public static class JwtBearerEventHandler
    {
        public static Task OnTokenValidated(TokenValidatedContext context)
        {
            if (context.Principal.Identity.IsAuthenticated)
            {
                var userId = context.Principal.Identity.Name;
                var token = context.SecurityToken as JwtSecurityToken;

                //set or refresh cache 
                var cacheManager = (ICacheManager)context.HttpContext.RequestServices.GetService(typeof(ICacheManager));
                var userTokenCache = cacheManager.GetUserTokenCache(userId);

                if (string.IsNullOrEmpty(userTokenCache) || token == null || userTokenCache != token.RawData)
                {
                    context.NoResult();
                }
                else
                {
                    //验证成功缓存2小时
                    cacheManager.SetUserTokenCache(userId, token.RawData);
                }
            }

            return Task.CompletedTask;
        }
    }
}
