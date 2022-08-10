using Microsoft.IdentityModel.Tokens;
using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Data.SqlClient;
using System.Data;
using System.Net.Http;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace WebApp.Service
{
    public class TokenService : ITokenService
    {
        private static string _securityKey = ConfigurationManager.AppSettings["SecurityKey"];
        private static string _accessTokenDurationInMinutes = ConfigurationManager.AppSettings["AccessTokenDurationInMinutes"];
        public HttpRequestMessage Request { get; set; }

        public string GenerateAccessToken(string userName)
        {
            byte[] key = Convert.FromBase64String(_securityKey);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, value: userName) }),
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(_accessTokenDurationInMinutes)),
                SigningCredentials = new SigningCredentials(securityKey, algorithm: SecurityAlgorithms.HmacSha256Signature)
            };
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);
            //Insert to database
            //InsertToken(handler.WriteToken(token));
            return handler.WriteToken(token);
        }
        //Gọi khi đã xác định được chính xác người dùng
        public string ValidateToken(string token)
        {
            try
            {
                ClaimsPrincipal principal = GetPrincipal(token);
                if (principal == null)
                    return "";
                ClaimsIdentity identity;
                try
                {
                    identity = (ClaimsIdentity)principal.Identity;
                }
                catch (NullReferenceException) { return ""; }
                Claim usernameClaim = identity.FindFirst(type: ClaimTypes.Name);
                string username = usernameClaim.Value;
                return username;
            }
            catch (NullReferenceException)
            {
                return null;
            }
        }
       
        public ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
                if (jwtToken == null || jwtToken.ValidTo == DateTime.MinValue || jwtToken.ValidTo < DateTime.UtcNow)
                    return null;
                byte[] key = Convert.FromBase64String(_securityKey);
                TokenValidationParameters parameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
                SecurityToken securityToken; 
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, parameters, out securityToken);
                return principal;
                 
            }
            catch{    return null;   }
        }
        public string GenerateRefreshToken()
        {
            byte[] randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        
    }
}