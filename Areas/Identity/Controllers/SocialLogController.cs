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
        //
        // ──────────────────────────────────────────────────────────────────────
        //   :::::: C L A S S   V A R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────
        //

        /// <value>The database context of the application</value>
        private ApplicationDBContext _context;
        
        /// <value>The configuration of the application</value>
        private IConfiguration _configuration;
        
        /// <value>Http client factory to do http request</value>
        private readonly IHttpClientFactory _http;


        //
        // ──────────────────────────────────────────────────────────────────────────
        //   :::::: C O N S T R U C T O R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The database context</param>
        /// <param name="configuration">The configuration of the application</param>
        /// <param name="clientFactory">Http client factory</param>
        public SocialLogController(
            ApplicationDBContext context, 
            IConfiguration configuration,
            IHttpClientFactory clientFactory
        ){
            _context = context;
            _configuration = configuration;
            _http = clientFactory;
        }


        //
        // ──────────────────────────────────────────────────────────────────────────────────
        //   :::::: P U B L I C   F U N C T I O N S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────────────
        //

        [HttpPost]
        [AllowAnonymous]
        [ActionName("SocialLog")]
        /// <summary>
        /// Log in or sign up an user by google or facebook
        /// </summary>
        /// <param name="socialUser">The info to log a user</param>
        /// See <see cref="Areas.Identity.Models.UserMediaLog"/> to know the param structure
        /// <returns>The IActionResult of the socialLog action</returns>
        /// See <see cref="Areas.Identity.Models.UserSession"/> to know the return structure
        public async Task<IActionResult> socialLog([FromBody] UserMediaLog socialUser)
        {
            if (socialUser.socialProvider == "FACEBOOK") return await doSocialLog(socialUser, false);

            else if (socialUser.socialProvider == "GOOGLE") return await doSocialLog(socialUser, true);

            return BadRequest(new { error = "InvalidSocialToken" });
        }


        //
        // ────────────────────────────────────────────────────────────────────────────────────
        //   :::::: P R I V A T E   F U N C T I O N S : :  :   :    :     :        :          :
        // ────────────────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Do the social log on google and facebook
        /// </summary>
        /// <param name="socialUser">The info to log/sign the user</param>
        /// See <see cref="Areas.Identity.Models.UserMediaLog"/> to know param structure
        /// <param name="isGoogleType">True if the log/sign is to Google, false if is a Facebook log/sign</param>
        /// <returns>The IActionResult of the social log</returns>
        /// See <see cref="Areas.Identity.Models.UserSession"/> to know the return structure
        private async Task<IActionResult> doSocialLog(UserMediaLog socialUser, Boolean isGoogleType)
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
                    Home.Util.GroupNew.launch(user, null, null, Home.Models.TypeGroupNew.WELCOME, false, _context);
                }
                else //The new user already exists
                {
                    //The new user already exists but he has sent a new password (wtf?)
                    if (PasswordHasher.validPassword(socialUser.password) || socialUser.password != null)
                    {
                        if (user.dateDeleted != null)
                        {
                            return BadRequest(new { error = "DeleteRequested" });
                        }
                        //The user is trying to reSignUp again
                        return BadRequest(new { error = "EmailAlreadyExistsError" });
                    }
                    if (!user.open)
                    {
                        return BadRequest(new { error = "YoureBanned" });
                    }
                    if (user.dateDeleted != null)
                    {
                        //The user asked for delete the account, but he has log in to reset the delete request
                        ResetDelete.reset(user, _context);
                        Home.Util.GroupNew.launch(user, null, null, Home.Models.TypeGroupNew.WELCOMEBACK, false, _context);
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

        /// <summary>
        /// Check if the email is saved in the database
        /// </summary>
        /// <param name="email">The email to log/sign</param>
        /// <param name="user">A new user object, to save the user on it (only if its exists)</param>
        /// <returns>True if the user exists, false otherwise</returns>
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

        /// <summary>
        /// Add a user to the database
        /// </summary>
        /// <param name="socialUser">The info of the user to add</param>
        /// See <see cref="Areas.Identity.Models.UserMediaLog"/> to know the param structure
        /// <returns>The user who has been added</returns>
        private User addSocialUser(UserMediaLog socialUser)
        {
            User newUser = new User {
                email = socialUser.email,
                nickname = socialUser.firstName,
                password = PasswordHasher.hashPassword(socialUser.password),
                tokenValidation = null,
                role = RoleManager.getNormalUser(_context),
                profileImg = getImage(socialUser.urlImage)
            };

            _context.User.Add(newUser);
            _context.SaveChanges();

            return newUser;
        }

        /// <summary>
        /// Verify the Google token
        /// </summary>
        /// <param name="token">The token to verify</param>
        /// <param name="userId">The id of ther user in Google</param>
        /// <returns>True if the token is valid, false otherwise</returns>
        private async Task<Boolean> verifyGoogleToken(string token, string userId)
        {
            //Install-Package Google.Apis.Auth -Version 1.38.0
            var validPayLoad = await GoogleJsonWebSignature.ValidateAsync(token);

            if (validPayLoad == null) return false;

            if (validPayLoad.Audience.ToString() != _configuration["Social:googleId"]) return false;

            if (validPayLoad.Subject != userId) return false;

            return true;
        }

        /// <summary>
        /// Do the request to verify the Facebook token
        /// </summary>
        /// <param name="token">The token to verify</param>
        /// <param name="userId">The id of the user in Facebook</param>
        /// <returns>True if the token is valid, false otherwise</returns>
        private async Task<Boolean> verifyFacebookToken(string token, string userId)
        {
            string facebookId = _configuration["Social:facebookSecret"];
            var request = new HttpRequestMessage(HttpMethod.Get, 
                "https://graph.facebook.com/debug_token?" + "input_token=" + token + "&access_token=" + facebookId);

            var client = _http.CreateClient();
            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode) return false;

            string result = await response.Content.ReadAsStringAsync();
            FacebookResponse resultJSON = JsonConvert.DeserializeObject<FacebookResponse>(result);

            return isValidFacebookToken(resultJSON, userId);
        }

        /// <summary>
        /// Verify the Facebook token
        /// </summary>
        /// <param name="res">The response of the verify Facebook request</param>
        /// <param name="userId">The id of the user in facebook</param>
        /// <returns>True if the token is valid, false otherwise</returns>
        private Boolean isValidFacebookToken(FacebookResponse res, string userId)
        {
            if (res.data.app_id != _configuration["Social:facebookId"]) return false;
            if (res.data.error != null) return res.data.error.is_valid;
            if (userId != res.data.user_id) return false;
            return res.data.is_valid;
        }

        /// <summary>
        /// Get the image from a url
        /// </summary>
        /// <param name="url">The url to get the image</param>
        /// <returns>The image in bytes</returns>
        private Byte[] getImage(string url)
        {
            WebClient client = new WebClient();
            Byte[] bytes = client.DownloadData(new Uri(url));

            return bytes;
        }
    }
}
