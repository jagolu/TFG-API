using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace API.Controllers.Identity
{
    [Route("Authorization/[action]")]
    [ApiController]
    public class LogInController : ControllerBase
    {
        private ApplicationDBContext _context;
        private IConfiguration _configuration;

        public LogInController(ApplicationDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

        }

        [HttpPost]
        [AllowAnonymous]
        [ActionName("LogIn")]
        public IActionResult logIn([FromBody] UserLogIn user)
        {
            var userExist = this._context.User.Where(u => u.email == user.email);

            if ((userExist.Count() != 1) || (PasswordHasher.hashPassword(user.password) != userExist.First().password)) {
                return BadRequest(new { error = "WrongEmailOrPassword" });
            }

            if ((userExist.First().tokenValidation != null)) {
                return BadRequest(new { error = "NotValidatedYet" });
            }

            try {
                string newRefreshToken = TokenGenerator.generateRefreshToken(_context, user.email, user.provider);
                string newToken = TokenGenerator.generateToken(user.email, newRefreshToken);

                return Ok(new { token = newToken });

            } catch (Exception) {
                return BadRequest(new { error = "InvalidToken" });
            }
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
    }
}
