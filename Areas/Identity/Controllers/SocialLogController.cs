using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using API.Areas.Identity.Models;
using API.Areas.Identity.Util;
using API.Data;
using API.Data.Models;
using API.Util;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace API.Areas.Identity.Controllers
{
    [Route("Authorization/[action]")]
    [ApiController]
    public class SocialLogController : ControllerBase
    {
        private ApplicationDBContext _context;
        private IConfiguration _configuration;
        private readonly IHttpClientFactory _http;

        public SocialLogController(ApplicationDBContext context, IConfiguration configuration,
                                IHttpClientFactory clientFactory)
        {
            _context = context;
            _configuration = configuration;
            _http = clientFactory;
        }

        [HttpPost]
        [AllowAnonymous]
        [ActionName("SocialLog")]
        public async Task<IActionResult> socialLog([FromBody] UserMediaLog socialUser)
        {
            if (socialUser.socialProvider == "FACEBOOK") return await socialLog(socialUser, false);

            else if (socialUser.socialProvider == "GOOGLE") return await socialLog(socialUser, true);

            return BadRequest(new { error = "InvalidSocialToken" });
        }

        private async Task<IActionResult> socialLog(UserMediaLog socialUser, Boolean isGoogleType)
        {
            try {
                if(isGoogleType && !await verifyGoogleToken(socialUser.authToken, socialUser.id)) {
                    return BadRequest(new { error = "InvalidSocialToken" });
                }
                if (!isGoogleType && !await verifyFacebookToken(socialUser.authToken, socialUser.id)) {
                    return BadRequest(new { error = "InvalidSocialToken" });
                }

                User user = new User();

                if(!existsUser(socialUser.email, ref user)) //The new user doesn't exists
                {
                    //The new user doesn't exist but his password isn't correct or is null
                    if (!PasswordHasher.validPassword(socialUser.password))
                    {
                        //The user is trying to log without signUp first
                        return BadRequest(new { error = "NotSocialSignYet" });//No registrado
                    }
                    //The new user doesn't exist and his password is correct and != null
                    user = addSocialUser(socialUser);
                }
                else //The new user already exists
                {
                    //The new user already exists but he has sent a new password (wtf?)
                    if (PasswordHasher.validPassword(socialUser.password) || socialUser.password != null)
                    {
                        //The user is trying to reSignUp again
                        return BadRequest(new { error = "EmailAlreadyExistsError" });
                    }

                    //Here the user already exists and doesn't send a password, so is
                    // trying to do a normal logIn
                }

                if (AdminPolicy.isAdmin(user, _context)) return BadRequest("notAllowed");
                UserSession session = MakeUserSession.getUserSession(_context, user, socialUser.provider);
                if (session == null) return StatusCode(500);

                return Ok(session);
            } catch (Exception) {
                return StatusCode(500);
            }
        }

        private bool existsUser(string email, ref User user)
        {
            var userExists = _context.User.Where(u => u.email == email);
            if (userExists.Count() != 1)
            {
                return false;
            }
            user = userExists.First();
            return true;
        }

        private User addSocialUser(UserMediaLog socialUser)
        {
            User newUser = new User {
                email = socialUser.email,
                nickname = socialUser.firstName,
                password = PasswordHasher.hashPassword(socialUser.password),
                tokenValidation = null,
                role = _context.Role.Where(r => r.name == "NORMAL_USER").First(),
                profileImg = getImage(socialUser.urlImage)
            };

            _context.User.Add(newUser);
            _context.SaveChanges();

            return newUser;
        }

        private async Task<Boolean> verifyGoogleToken(string token, string userId)
        {
            //Install-Package Google.Apis.Auth -Version 1.38.0
            var validPayLoad = await GoogleJsonWebSignature.ValidateAsync(token);

            if (validPayLoad == null) return false;

            if (validPayLoad.Audience.ToString() != _configuration["Social:googleId"]) return false;

            if (validPayLoad.Subject != userId) return false;

            return true;
        }

        private async Task<Boolean> verifyFacebookToken(string token, string userId)
        {
            string facebookId = _configuration["Social:facebookSecret"];

            var request = new HttpRequestMessage(HttpMethod.Get,
                "https://graph.facebook.com/debug_token?" +
                "input_token=" + token + "&access_token=" + facebookId);

            var client = _http.CreateClient();

            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode) return false;

            string result = await response.Content.ReadAsStringAsync();
            FacebookResponse resultJSON = JsonConvert.DeserializeObject<FacebookResponse>(result);

            return isValidFacebookToken(resultJSON, userId);
        }

        private Boolean isValidFacebookToken(FacebookResponse res, string userId)
        {
            if (res.data.app_id != _configuration["Social:facebookId"]) return false;
            if (res.data.error != null) return res.data.error.is_valid;
            if (userId != res.data.user_id) return false;
            return res.data.is_valid;
        }

        private Byte[] getImage(string url)
        {
            WebClient client = new WebClient();
            Byte[] bytes = client.DownloadData(new Uri(url));

            return bytes;
        }
    }
}
