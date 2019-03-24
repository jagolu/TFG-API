using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using API.Data;
using API.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
            if(socialUser.socialProvider == SocialProvider.FACEBOOK)
            {
                try
                {
                    if (!await verifyFacebookToken(socialUser.authToken))
                    {
                        return BadRequest(new { error = "InvalidSocialToken" });
                    }

                    addSocialUser(socialUser);
                }
                catch (Exception)
                {
                    return StatusCode(500);
                }
            }
            return Ok();
        }

        private async Task<IActionResult> facebookLog(UserMediaLog socialUser)
        {
            if (socialUser.socialProvider == SocialProvider.FACEBOOK)
            {
                try
                {
                    if (!await verifyFacebookToken(socialUser.authToken))
                    {
                        return BadRequest(new { error = "InvalidSocialToken" });
                    }

                    addSocialUser(socialUser);
                }
                catch (Exception)
                {
                    return StatusCode(500);
                }
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

        private async Task<Boolean> verifyFacebookToken(string token)
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

            return resultJSON.data.error == null ? resultJSON.data.is_valid : resultJSON.data.error.is_valid;
        }

        private Boolean isValidFacebookToken(FacebookResponse res)
        {

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
        public SocialProvider socialProvider { get; set; }

        [Required]
        public Boolean provider { get; set; } = false;
    }

    public enum SocialProvider
    {
        FACEBOOK = 0,
        GOOGLE = 1
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
