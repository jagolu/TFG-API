using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers.Identity
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private Data.ApplicationDBContext _context;
        private IConfiguration _config;

        public AuthenticationController(Data.ApplicationDBContext c, 
                                        IConfiguration conf)
        {
            this._context = c;
            this._config = conf;
        }
        
        [HttpPost]
        [AllowAnonymous]
        [ActionName("SignUp")]
        public IActionResult signUp([FromBody] UserSignUp user)
        {
            /*if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }*/
            var userExists = this._context.User.Where(u => u.email == user.email).Count() != 0;

            if (userExists) {
                return BadRequest(new { error = "EmailAlreadyExistsError" });
            }

            try {
                User u = new User {
                    email = user.email,
                    nickname = user.username,
                    password = user.getHashPassword(this._config),
                    tokenValidation = (user.password == null) ? null : Guid.NewGuid().ToString("N")
                };
                this._context.User.Add(u);

                Email email = new Email(this._config);
                email.sendVerificationToken(u.email, u.nickname, u.tokenValidation);

                this._context.SaveChanges();

                return Ok();

            } catch (Exception e) {
                return StatusCode(500);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ActionName("LogIn")]
        public IActionResult logIn([FromBody] UserLogIn user)
        {
            var userExist = this._context.User.Where(u => u.email == user.email);
            
            if( (userExist.Count() != 1) || (user.getHashPassword(this._config) != userExist.First().password) ) {
                return BadRequest(new { error = "WrongEmailOrPassword" });
            }

            if((userExist.First().tokenValidation != null)) {
                return BadRequest(new { error = "NotValidatedYet" });
            }

            string refreshToken = this.generateRefreshToken(user.email, user.provider);
            string token = this.generateToken(user.email, refreshToken);

            return Ok(new { token = token });
        }

        [HttpGet]
        [AllowAnonymous]
        [ActionName("Validate")]
        public IActionResult validateEmail(string emailToken, Boolean provider = false)
        {
            var tokenExists = this._context.User.Where(u => u.tokenValidation == emailToken);

            if(tokenExists.Count() != 1) {
                return BadRequest(new { error = "NonExistingToken" });
            }

            User user = tokenExists.First();

            try {
                user.tokenValidation = null;

                this._context.Update(user);
                this._context.SaveChanges();

            }catch(Exception e) {
                return StatusCode(500);
            }

            string refreshToken = this.generateRefreshToken(user.email, provider);
            string token = this.generateToken(user.email, refreshToken);

            return Ok(new { token = token });
        }

        [HttpGet]
        [AllowAnonymous]
        [ActionName("SocialLog")]
        public IActionResult socialLog(string tok) {
            string b = tok;
            Boolean isOk = this.verifyFacebookTokenAsync(tok).;
            if (isOk) {
                return BadRequest(new {error="TokenDoesntExists");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ActionName("Refresh")]
        public IActionResult refresh([FromBody] refreshRequest req)
        {
            var principal = this.getPrincipalFromExpiredToken(req.token);

            string email = principal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;
            string refreshToken = principal.FindFirst("refreshToken").Value;
            var savedRefreshToken = this._context.UserToken.Where(ut => ut.refreshToken == refreshToken);

            if (savedRefreshToken.Count() != 1) {
                throw new SecurityTokenException("Invalid refresh token");
            }

            string newRefreshToken = this.generateRefreshToken(email, req.provider);
            string newToken = this.generateToken(email, newRefreshToken);

            return Ok(new { token = newToken });
        }

        private String generateToken(string email, string refreshToken)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: this._config["Jwt:Issuer"],
                claims: new Claim[] {
                    new Claim(ClaimTypes.Email, email),
                    new Claim("refreshToken", refreshToken)
                },
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private String generateRefreshToken(string email, Boolean provider)
        {
            User user = (from u in this._context.User where u.email == email select u).First();
            var ut= from t in this._context.UserToken where t.userId == user.id && t.loginProvider==provider select t;
            string refreshToken = Guid.NewGuid().ToString();

            try { 
                if(ut.Count() != 1) {
                    this._context.UserToken.Add(new UserToken {
                        User = user,
                        refreshToken = refreshToken,
                        loginProvider = provider
                    });
                }
                else {
                    UserToken token = ut.First();
                    token.refreshToken = refreshToken;
                    token.expirationTime = DateTime.Now.AddHours(1);

                    this._context.Update(token);
                }

                this._context.SaveChanges();
            }catch(Exception e) {
                return null;
            }

            return refreshToken;
        }

        private ClaimsPrincipal getPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters 
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if(jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase)) {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

        private async Task<bool> verifyFacebookTokenAsync(string token)
        {
            string facebookId = this._config["Social:facebook"];
            /*var request = new HttpRequestMessage(HttpMethod.Get,
                "https://graph.facebook.com/debug_token?" +
                "input_token=" + token +
                "&access_token=" + facebookId);*/

            var request = "https://graph.facebook.com/debug_token?" +
                "input_token=" + token +
                "&access_token=" + facebookId;

            var client = new HttpClient();

            var response = await client.GetStringAsync(request);

            //TODO get body of the response
            return false;
        }
    }

    public class UserSignUp
    {
        [Required]
        [EmailAddress(ErrorMessage = "This is not a valid email")]
        public string email { get; set; }

        [Required]
        [MinLength(4, ErrorMessage = "Username must have at least 3 characters")]
        [MaxLength(20, ErrorMessage = "Username must have less than 20 characters")]
        public string username { get; set; }

        [MinLength(8, ErrorMessage = "Password must have at least 8 characters")]
        [MaxLength(20, ErrorMessage = "Password must have less than 20 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{1,}$",
            ErrorMessage = "Password must have at least a lowercase, a uppercase and a number")]
        public string password { get; set; }

        public Boolean provider { get; set; } = false;

        public string getHashPassword(IConfiguration configuration)
        {
            if (password == null) return null;
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: new byte[int.Parse(configuration["Crypt:saltSize"])],
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: int.Parse(configuration["Crypt:hashCount"]),
                numBytesRequested: int.Parse(configuration["Crypt:subkeyLength"])
            )); ;
        }
    }

    public class UserLogIn
    {
        [Required]
        [EmailAddress(ErrorMessage = "This is not a valid email")]
        public string email { get; set; }

        [MinLength(8, ErrorMessage = "Password must have at least 8 characters")]
        [MaxLength(20, ErrorMessage = "Password must have less than 20 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{1,}$",
            ErrorMessage = "Password must have at least a lowercase, a uppercase and a number")]
        public string password { get; set; }

        public Boolean provider { get; set; } = false;

        public string getHashPassword(IConfiguration configuration)
        {
            if (password == null) return null;
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: new byte[int.Parse(configuration["Crypt:saltSize"])],
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: int.Parse(configuration["Crypt:hashCount"]),
                numBytesRequested: int.Parse(configuration["Crypt:subkeyLength"])
            )); ;
        }
    }

    public class refreshRequest
    {
        [Required]
        public string token { get; set; }

        public Boolean provider { get; set; } = false;
    }
}
