using API.Data;
using API.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace API.Util
{
    public static class TokenGenerator
    {
        private static IConfiguration _configuration;

        public static void Initialize(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static string generateTokenAndRefreshToken(ApplicationDBContext context, string email, Boolean provider)
        {
            try
            {
                string newRefreshToken = generateRefreshToken(context, email, provider);
                string newToken = generateToken(email, newRefreshToken);

                return newToken;

            }
            catch (Exception)
            {
                return null;
            }
        }

        public static Boolean isValidClaim(string token)
        {
            return getPrincipalFromExpiredToken(token) == null;
        }

        public static string getEmailClaim(string token)
        {
            var principal = getPrincipalFromExpiredToken(token);
            
            return principal == null ? null :
                principal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;
        }

        public static string getRefreshTokenClaim(string token)
        {
            var principal = getPrincipalFromExpiredToken(token);

            return principal == null ? null : 
                principal.FindFirst("refreshToken").Value;
        }

        public static string getBearerToken(string token)
        {
            string bearer = "Bearer ";
            int start = token.IndexOf(bearer)+bearer.Length;
            return token.Substring(start);
        }

        private static String generateToken(string email, string refreshToken)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                claims: new Claim[] {
                    new Claim(ClaimTypes.Email, email),
                    new Claim("refreshToken", refreshToken)
                },
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static String generateRefreshToken(ApplicationDBContext context,string email, Boolean provider)
        {
            var user = (from u in context.User where u.email == email select u).First();
            var ut = from t in context.UserToken where t.userId == user.id && t.loginProvider == provider select t;
            string refreshToken = Guid.NewGuid().ToString();

            try {
                if (ut.Count() != 1) {
                    context.UserToken.Add(new UserToken {
                        User = user,
                        refreshToken = refreshToken,
                        loginProvider = provider
                    });
                }
                else {
                    UserToken token = ut.First();
                    token.refreshToken = refreshToken;
                    token.expirationTime = DateTime.Now.AddHours(1);

                    context.Update(token);
                }

                context.SaveChanges();
            } catch (Exception) {
                return null;
            }

            return refreshToken;
        }

        private static ClaimsPrincipal getPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters {
                ValidateAudience = false,
                ValidateIssuer = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;

            try {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

                var jwtSecurityToken = securityToken as JwtSecurityToken;

                if (jwtSecurityToken == null ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase)) {
                    return null;
                }

                return principal;

            } catch (Exception) {
                return null;
            }
        }
    }
}
