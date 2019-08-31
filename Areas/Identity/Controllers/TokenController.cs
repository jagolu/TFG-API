using System;
using System.Collections.Generic;
using System.Linq;
using API.Areas.Identity.Models;
using API.Areas.Identity.Util;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Areas.Identity.Controllers
{
    [Route("Authorization/[action]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        //
        // ──────────────────────────────────────────────────────────────────────
        //   :::::: C L A S S   V A R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────
        //

        /// <value>The database context of the application</value>
        private ApplicationDBContext _context;


        //
        // ──────────────────────────────────────────────────────────────────────────
        //   :::::: C O N S T R U C T O R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The context of the database</param>        
        public TokenController(ApplicationDBContext context)
        {
            _context = context;
        }


        //
        // ──────────────────────────────────────────────────────────────────────────────────
        //   :::::: P U B L I C   F U N C T I O N S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────────────
        //
        
        [HttpPost]
        [AllowAnonymous]
        [ActionName("Refresh")]
        /// <summary>
        /// Get a new session token
        /// </summary>
        /// <param name="req">The info of the refresh</param>
        /// See <see cref="Areas.Identity.Models.RefreshRequest"/> to know the param structure
        /// <returns>The IActionResult of the refresh request</returns>
        /// See <see cref="Areas.Identity.Models.UserSession"/> the response structure
        public IActionResult refresh([FromBody] RefreshRequest req)
        {
            if (TokenGenerator.isValidClaim(req.token)) return StatusCode(401);

            string email = TokenGenerator.getEmailClaim(req.token);
            string refreshToken = TokenGenerator.getRefreshTokenClaim(req.token);

            if(refreshToken == null) return StatusCode(401); 

            List<UserToken> savedRefreshToken = _context.UserToken.Where(ut => ut.refreshToken == refreshToken).ToList();

            if (savedRefreshToken.Count() != 1) {
                return StatusCode(401);
            }

            if(savedRefreshToken.First().expirationTime < DateTime.Now)
            {
                try
                {
                    _context.Remove(savedRefreshToken.First());
                    _context.SaveChanges();
                }
                catch (Exception) { }
                return StatusCode(401);
            }

            User user = _context.User.Where(u => u.email == email).First();
            if (!user.open) return BadRequest(new { error = "YoureBanned" });

            UserSession session = MakeUserSession.getUserSession(_context, user, req.provider);

            if (session == null) return StatusCode(500);

            return Ok(session);
        }
    }
}
