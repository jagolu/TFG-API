using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using API.Data;
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
        public string socialLog([FromBody] UserMediaLog socialToken)
        {
            this.verifyFacebookTokenAsync(socialToken.authToken);
            return socialToken.authToken;
        }

        private async Task<bool> verifyFacebookTokenAsync(string token)
        {
            string facebookId = _configuration["Social:facebook"];

            var request = new HttpRequestMessage(HttpMethod.Get,
                "https://graph.facebook.com/debug_token?" +
                "input_token=" + token + "&access_token=" + facebookId);

            var client = _http.CreateClient();
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode) {
                string result = await response.Content.ReadAsStringAsync();
                FacebookResponse resultJSON = JsonConvert.DeserializeObject<FacebookResponse>(result);
                string a = "asdfasd";
                
            }
            else {
                return false;
            }

            //TODO get body of the response	
            return false;
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
