using API.Data;
using API.Data.Models;
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
        //
        // ──────────────────────────────────────────────────────────────────────
        //   :::::: C L A S S   V A R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// The configuration of the app
        /// </summary>
        private static IConfiguration _configuration;


        //
        // ──────────────────────────────────────────────────────────────────────────
        //   :::::: C O N S T R U C T O R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Initializes the configruation
        /// </summary>
        /// <param name="configuration">The configuration of the app</param>
        public static void Initialize(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        //
        // ──────────────────────────────────────────────────────────────────────────────────
        //   :::::: P U B L I C   F U N C T I O N S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Generate a new session token and a refresh token
        /// </summary>
        /// <param name="context">The database context</param>
        /// <param name="email">The email of the user who wants the new token</param>
        /// <param name="provider">The provider of the caller</param>
        /// <returns>The new session token</returns>
        public static string generateTokenAndRefreshToken(ApplicationDBContext context, string email, bool provider)
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

        /// <summary>
        /// Check if the claim of the token is valid
        /// </summary>
        /// <param name="token">The session token</param>
        /// <returns>True if the claim is valid, false otherwise</returns>
        public static bool isValidClaim(string token)
        {
            return getPrincipalFromExpiredToken(token) == null;
        }

        /// <summary>
        /// Get the email claim of the token
        /// </summary>
        /// <param name="token">The session token</param>
        /// <returns>The email claim of the token</returns>
        public static string getEmailClaim(string token)
        {
            var principal = getPrincipalFromExpiredToken(token);
            
            return principal == null ? null :
                principal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;
        }

        /// <summary>
        /// Get the refresh token claim of the token
        /// </summary>
        /// <param name="token">The session token</param>
        /// <returns>The refresh token claim of the token</returns>
        public static string getRefreshTokenClaim(string token)
        {
            var principal = getPrincipalFromExpiredToken(token);

            return principal == null ? null : principal.FindFirst("refreshToken").Value;
        }

        /// <summary>
        /// Get the bearer of the http request
        /// </summary>
        /// <param name="token">The value of the authorization header</param>
        /// <returns>The bearer token</returns>
        public static string getBearerToken(string token)
        {
            string bearer = "Bearer ";
            int start = token.IndexOf(bearer)+bearer.Length;
            return token.Substring(start);
        }


        //
        // ────────────────────────────────────────────────────────────────────────────────────
        //   :::::: P R I V A T E   F U N C T I O N S : :  :   :    :     :        :          :
        // ────────────────────────────────────────────────────────────────────────────────────
        //
        
        /// <summary>
        /// Generates a new session token
        /// </summary>
        /// <param name="email">The email of the user who wants the new token</param>
        /// <param name="refreshToken">The refresh token of the user</param>
        /// <returns>The new token</returns>
        private static string generateToken(string email, string refreshToken)
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
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Generate a new refresh token
        /// </summary>
        /// <param name="context">The context of the database</param>
        /// <param name="email">The email of the user who wants the new token</param>
        /// <param name="provider">The provider of the caller</param>
        /// <returns>The new refresh token</returns>
        private static string generateRefreshToken(ApplicationDBContext context,string email, bool provider)
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
                    token.expirationTime = DateTime.Now.AddMinutes(60);

                    context.Update(token);
                }

                context.SaveChanges();
            } catch (Exception) {
                return null;
            }

            return refreshToken;
        }

        /// <summary>
        /// Get the "principal" of the expired token
        /// </summary>
        /// <param name="token">The token that has expired</param>
        /// <returns>The token that has expired</returns>
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
