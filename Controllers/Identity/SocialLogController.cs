using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using API.Data;
using API.Models;
using API.Util;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;



namespace API.Controllers.Identity
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
            try
            {
                if (!await verifyFacebookToken(socialUser.authToken, socialUser.id))
                {
                    return BadRequest(new { error = "InvalidSocialToken" });
                }

                addSocialUser(socialUser);

                return generateTokenAndRefreshToken(socialUser.email, socialUser.provider);

            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
        
        private void addSocialUser(UserMediaLog socialUser)
        {
            var userExist = _context.User.Where(u => u.email == socialUser.email);

            if (userExist.Count() == 1) return;

            _context.User.Add(new User
            {
                email = socialUser.email,
                nickname = socialUser.firstName,
                password = null,
                tokenValidation = null
            });

            _context.SaveChanges();
        }

        private async Task<Boolean> verifyGoogleToken(string token, string userId)
        {
            //Install-Package Google.Apis.Auth -Version 1.38.0
            var validPayLoad = await GoogleJsonWebSignature.ValidateAsync(token);

            if (validPayLoad == null) return false;

            if (validPayLoad.Audience.ToString() != _configuration["Social:googleId"])  return false;

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

        private IActionResult generateTokenAndRefreshToken(string email, Boolean provider) {
            string nToken = TokenGenerator.generateTokenAndRefreshToken(_context, email, provider);

            if (nToken != null) return Ok(new { token = nToken });
            else return StatusCode(500);
        }
    }

    public class UserMediaLog
    {
        [Required]
        public string authToken { get; set; }

        [Required]
        public string email { get; set; }

        [Required]
        public string firstName { get; set; }

        [Required]
        public string id { get; set; }

        [Required]
        public string socialProvider { get; set; }

        [Required]
        public Boolean provider { get; set; } = false;
    }

    public class FacebookResponse
    {
        public dataType data { get; set; }

        public class dataType
        {
            public string app_id { get; set; }
            public string type { get; set; }
            public string application { get; set; }
            public string data_access_expires_at { get; set; }
            public Boolean is_valid { get; set; }
            public string user_id { get; set; }
            public errorType error { get; set; }


            public class errorType
            {
                public Boolean is_valid { get; set; }
            }
        }

    }
}
