﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers.Identity
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailVerificationController : ControllerBase
    {
        private Data.ApplicationDBContext _context;

        public EmailVerificationController(Data.ApplicationDBContext context)
        {
            _context = context;
        }

        // POST: api/EmailVerification
        [HttpPost]
        [AllowAnonymous]
        public IActionResult Post([FromBody] object tokenRequest)
        {
            //Convierte el object que se pasa en tipo JSON a tipo string
            String token = ((dynamic)JObject.Parse(tokenRequest.ToString())).token;

            var user = _context.User.Where(u => u.tokenValidation == token);

            if (user.Count() != 1) {
                return BadRequest(new { error = "TokenNotExists" });
            }

            try {
                Models.User userUpdate = user.First();
                userUpdate.tokenValidation = null; 

                _context.User.Update(userUpdate);

                _context.SaveChanges();

                return Ok();

            }catch(Exception e) {
                return StatusCode(500);
            }
        }
    }
}
