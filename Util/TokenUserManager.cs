using API.Data;
using API.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace API.Util
{
    public static class TokenUserManager
    {
        public static User getUserFromToken(HttpContext httpContext, ApplicationDBContext context)
        {
            var authToken = httpContext.Request?.Headers["Authorization"];

            string email = TokenGenerator.getEmailClaim(TokenGenerator.getBearerToken(authToken.Value));

            User user = context.User.Where(u => u.email == email).First();

            return user;
        }
    }
}
