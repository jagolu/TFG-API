using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace API.Controllers.Identity
{
    [Route("Authorization/[action]")]
    [ApiController]
    public class ValidationEmailController : ControllerBase
    {
        private ApplicationDBContext _context;
        private IConfiguration _configuration;

        public ValidationEmailController(ApplicationDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        [AllowAnonymous]
        [ActionName("Validate")]
        public IActionResult validateEmail([Required] string emailToken, Boolean provider = false)
        {
            var tokenExists = _context.User.Where(u => u.tokenValidation == emailToken);

            if (tokenExists.Count() != 1) {
                return BadRequest(new { error = "NonExistingToken" });
            }

            User user = tokenExists.First();
            
            user.tokenValidation = null;

            _context.Update(user);

            try {
                string newRefreshToken = TokenGenerator.generateRefreshToken(_context, user.email, provider);
                string newToken = TokenGenerator.generateToken(user.email, newRefreshToken);

                _context.SaveChanges();

                return Ok(new { token = newToken });

            } catch (DbUpdateException) {
                return StatusCode(500);

            } catch (Exception) {
                return BadRequest(new { error = "InvalidToken" });

            }
        }
    }
}
