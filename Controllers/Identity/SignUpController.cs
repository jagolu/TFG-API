using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using API.Data;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace API.Controllers.Identity
{
    [Route("Authorization/[action]")]
    [ApiController]
    public class SignUpController : ControllerBase
    {
        private ApplicationDBContext _context;
        private IConfiguration _configuration;

        public SignUpController(ApplicationDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost]
        [AllowAnonymous]
        [ActionName("SignUp")]
        public IActionResult signUp([FromBody] UserSignUp user)
        {
            //if (!ModelState.IsValid) {
            //    return BadRequest(ModelState);
            //}
            var userExists = _context.User.Where(u => u.email == user.email).Count() != 0;

            if (userExists) {
                return BadRequest(new { error = "EmailAlreadyExistsError" });
            }
            
            User newUser = new User {
                email = user.email,
                nickname = user.username,
                password = PasswordHasher.hashPassword(user.password),
                tokenValidation = (user.password == null) ? null : Guid.NewGuid().ToString("N")
            };

            _context.User.Add(newUser);
            
            try {

                EmailSender email = new EmailSender(_configuration);
                email.sendVerificationToken(newUser.email, newUser.nickname, newUser.tokenValidation);

                _context.SaveChanges();

            } catch (Exception) {

                return StatusCode(500);
            }

            return Ok();
        }

        [HttpGet]
        [AllowAnonymous]
        [ActionName("aa")]
        public string aa()
        {
            return "hola";
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
    }
}
