using System;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignUpController : ControllerBase
    {
        private Data.ApplicationDBContext _context;
        

        public SignUpController(Data.ApplicationDBContext context){
            _context = context;
        }

        // GET: api/SignUp
        [HttpGet]
        public string Get()
        {
            string returnValue = "Users: ";
            foreach(Models.User u in _context.User)
            {
                returnValue += "\n\t" + u.email;
            }
            returnValue += "\n\nRoles";
            foreach(Models.Role r in _context.Role)
            {
                returnValue += "\n\t" + r.name;
            }
            return returnValue;
        }

        // GET: api/SignUp/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/SignUp
        [HttpPost]
        public IActionResult Post([FromBody] User user)
        {
            /*if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }*/
            var userExists = _context.User.Where(u => u.email == user.email).Count() != 0;
            
            if (userExists) {
                return BadRequest(new { error = "EmailAlreadyExistsError"});
            }

            try {
                API.Models.User u = new Models.User {
                    email = user.email,
                    nickname = user.username,
                    password = user.password ?? user.getHashPassword(),
                    tokenValidation = (user.password == null) ? null : Guid.NewGuid().ToString("N")
                };
                _context.User.Add(u);
                _context.SaveChanges();

                Email email = new Email();
                email.sendVerificationToken(u.email, u.nickname, u.tokenValidation);

                return Ok(new { Ok="Success" });

            }catch(Exception e) {
                return StatusCode(500);
            }
        }

        // PUT: api/SignUp/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        public class User
        {
            [Required]
            [EmailAddress (ErrorMessage = "This is not a valid email")]
            public string email { get; set; }

            [Required]
            [MinLength(4, ErrorMessage = "Username must have at least 3 characters")]
            [MaxLength(20, ErrorMessage = "Username must have less than 20 characters")]
            public string username { get; set; }

            [MinLength(8, ErrorMessage = "Password must have at least 8 characters")]
            [MaxLength(20, ErrorMessage = "Password must have less than 20 characters")]
            [RegularExpression (@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{1,}$",
                ErrorMessage="Password must have at least a lowercase, a uppercase and a number")]
            public string password { get; set; }

            public string getHashPassword()
            {
                if (password == null) return null;
                return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: password,
                    salt: new byte [int.Parse(Environment.GetEnvironmentVariable("saltSize"))],
                    prf: KeyDerivationPrf.HMACSHA512,
                    iterationCount: int.Parse(Environment.GetEnvironmentVariable("hashCount")),
                    numBytesRequested: int.Parse(Environment.GetEnvironmentVariable("subkeyLength"))
                )); ;
            }
        }
    }
}
