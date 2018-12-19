using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;


namespace MyAbpDemo.Infrastructure.Api
{
    public class JwtToken
    {
        /// <summary>
        /// 获取登录TOKEN
        /// </summary>
        /// <param name="jwtSetting">The JWT setting.</param>
        /// <param name="userName">The user identifier.</param>
        /// <param name="email">email</param>
        /// <returns></returns>
        public static string GetToken(JwtSetting jwtSetting, string userName,string email)
        {
            var symmetricKeyAsBase64 = jwtSetting.ServerSecret;
            var keyByteArray = Encoding.UTF8.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray);
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var now = DateTime.UtcNow;
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,userName),
                new Claim(ClaimTypes.Email,email),
            };
        
         
            var jwt = new JwtSecurityToken(
                issuer: jwtSetting.Issuer,
                audience: jwtSetting.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(TimeSpan.FromDays(jwtSetting.ExpireDays)),
                signingCredentials: signingCredentials
            );
            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return token;
        }

    }
}
