using API.Data;
using API.Data.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace API.Util
{
    public static class TokenUserManager
    {
        /// <summary>
        /// Get the user asociated to a session token
        /// </summary>
        /// <param name="httpContext">The http context of the request</param>
        /// <param name="context">The database context</param>
        /// <returns>The user asociated to the token</returns>
        public static User getUserFromToken(HttpContext httpContext, ApplicationDBContext context)
        {
            var authToken = httpContext.Request?.Headers["Authorization"];

            string email = TokenGenerator.getEmailClaim(TokenGenerator.getBearerToken(authToken.Value));

            User user = context.User.Where(u => u.email == email).First();

            return user;
        }
    }
}
