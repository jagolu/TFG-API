﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using API.Areas.Identity.Models;
using API.Data;
using API.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

            try {
                UserSession session = UserSessionGenerator.getUserJson(_context, user, provider);

                if(session != null)
                {
                    List<UserGroups> groups = GroupsFromUser.getUserGroups(user, _context);

                    session.groups = groups;

                    _context.SaveChanges();

                    return Ok( session );
                }

                return StatusCode(500);

            } catch (DbUpdateException) {
                return StatusCode(500);

            } catch (Exception) {
                return BadRequest();

            }
        }
    }
}
