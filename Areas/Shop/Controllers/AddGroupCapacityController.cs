using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Areas.GroupManage.Models;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Areas.Shop.Controllers
{
    [Route("Shop/[action]")]
    [ApiController]
    public class AddGroupCapacityController : ControllerBase
    {
        private ApplicationDBContext _context;

        public AddGroupCapacityController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        [ActionName("AddGroupCapacity")]
        public IActionResult addCapacity(string groupName, int morePlaces)
        {
            //MEtodo basico para el desarrollo y pruebas de el entorno relacionado con las compras
            //TODO do it well
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);
            var dbGroup = _context.Group.Where(g => g.name == groupName);

            if(dbGroup.Count() != 1)
            {
                return BadRequest(new { error = "" });
            }

            if(morePlaces != 5 && morePlaces != 25 && morePlaces != 50)
            {
                return BadRequest(new { error = "" });
            }

            dbGroup.First().capacity = dbGroup.First().capacity + morePlaces;

            try
            {
                _context.Group.Update(dbGroup.First());
            }
            catch (Exception)
            {
                return StatusCode(500);
            }

            return Ok( new { success = "AddGroupCapacity"});
        }
    }
}
