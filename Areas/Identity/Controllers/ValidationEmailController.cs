using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using API.Areas.Identity.Models;
using API.Areas.Identity.Util;
using API.Data;
using API.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Areas.Identity.Controllers
{
    [Route("Authorization/[action]")]
    [ApiController]
    public class ValidationEmailController : ControllerBase
    {
        private ApplicationDBContext _context;

        public ValidationEmailController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        [ActionName("Validate")]
        public IActionResult validateEmail([Required] string emailToken, Boolean provider = false)
        {
            var tokenExists = _context.User.Where(u => u.tokenValidation == emailToken);

            if (tokenExists.Count() != 1) {
                return BadRequest();
            }

            User user = tokenExists.First();

            user.tokenValidation = null;

            _context.Update(user);

            UserSession session = MakeUserSession.getUserSession(_context, user, provider);

            if (session == null) return StatusCode(500);

            return Ok(session);
        }
    }
}
