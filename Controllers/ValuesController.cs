using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public ValuesController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<List<string>> Get()
        {
            var users = _context.User.ToList();
            var roles = _context.Role.ToList();

            int userSize = users.Count();
            int rolesSize = roles.Count();

            List<string> allUsers = new List<string>();
            List<string> allRoles = new List<string>();


            for (int i = 0; i < userSize; i++) {
                allUsers.Add(users.ElementAt(i).nickname);
            }
            /*for(int i = 0; i < rolesSize; i++) {
                allUsers.Add(roles.ElementAt(i).name);
            }*/

            return allUsers;
        }

        // GET api/values/5
        //[Authorize(Roles = Role.)]
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
