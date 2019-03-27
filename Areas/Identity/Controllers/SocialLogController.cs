using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using API.Areas.Identity.Models;
using API.Data;
using API.Models;
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

        public SocialLogController(ApplicationDBContext context,
                                IConfiguration configuration,
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
            if (socialUser.socialProvider == "FACEBOOK") return await facebookLog(socialUser);

            else if (socialUser.socialProvider == "GOOGLE") return await googleLog(socialUser);

            return BadRequest(new { error = "InvalidSocialToken" });
        }

        private async Task<IActionResult> googleLog(UserMediaLog socialUser)
        {
            try {
                if (!await verifyGoogleToken(socialUser.authToken, socialUser.id)) {
                    return BadRequest(new { error = "InvalidSocialToken" });
                }

                addSocialUser(socialUser);

                return generateTokenAndRefreshToken(socialUser.email, socialUser.provider);

            } catch (Exception) {
                return StatusCode(500);
            }

        }

        private async Task<IActionResult> facebookLog(UserMediaLog socialUser)
        {
            try {
                if (!await verifyFacebookToken(socialUser.authToken, socialUser.id)) {
                    return BadRequest(new { error = "InvalidSocialToken" });
                }

                addSocialUser(socialUser);

                return generateTokenAndRefreshToken(socialUser.email, socialUser.provider);

            } catch (Exception) {
                return StatusCode(500);
            }
        }

        private void addSocialUser(UserMediaLog socialUser)
        {
            var userExist = _context.User.Where(u => u.email == socialUser.email);

            if (userExist.Count() == 1) return;

            User newUser = new User {
                email = socialUser.email,
                nickname = socialUser.firstName,
                password = null,
                tokenValidation = null
            };

            UserRoles newUserRoles = new UserRoles {
                User = newUser,
                Role = _context.Role.Where(r => r.name == "NORMAL_USER").First()
            };

            _context.User.Add(newUser);
            _context.UserRoles.Add(newUserRoles);

            _context.SaveChanges();
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
            string facebookId = _configuration["Social:facebook"];

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

        private IActionResult generateTokenAndRefreshToken(string email, Boolean provider)
        {
            string nToken = TokenGenerator.generateTokenAndRefreshToken(_context, email, provider);

            if (nToken != null) return Ok(new { token = nToken });
            else return StatusCode(500);
        }
    }
}
