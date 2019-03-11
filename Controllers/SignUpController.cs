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
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
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
            Object returnValue;
            var userExists = _context.User.Where(u => u.email == user.email).Count() != 0;
            
            if (userExists) {
                return Ok(new { adfa = "EmailAlreadyExists"});
            }

            try {

                API.Models.User u = new Models.User {
                    email = user.email,
                    nickname = user.username,
                    password = user.getHashPassword()
                };
                _context.User.Add(u);
                _context.SaveChanges();
                returnValue = new { adfa="done"};

            }catch(Exception e) {
                returnValue = new { adfa= "ServerError" };
            }
            return Ok(new JsonResult (returnValue));
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
            [EmailAddress (ErrorMessage = "This is not a valid email")]
            public string email { get; set; }

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
