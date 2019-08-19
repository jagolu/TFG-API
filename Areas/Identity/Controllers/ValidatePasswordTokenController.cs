using System.ComponentModel.DataAnnotations;
using API.Areas.Identity.Util;
using API.Data;
using API.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Areas.Identity.Controllers
{
    [Route("Authorization/[action]")]
    [ApiController]
    public class ValidatePasswordTokenController : ControllerBase
    {
        private ApplicationDBContext _context;

        public ValidatePasswordTokenController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        [ActionName("checkPasswordToken")]
        public IActionResult checkPassword([Required] string passwordToken)
        {
            User u = new User();
            if (!ValidTokenPassword.isValid(passwordToken, ref u, _context))
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}
