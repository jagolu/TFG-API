﻿using System;
using System.Collections.Generic;
using System.Linq;
using API.Areas.DirectMessages.Models;
using API.Areas.DirectMessages.Util;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace API.Areas.DirectMessages.Controllers
{
    [Route("DirectMessages/[action]")]
    [ApiController]
    public class CreateDMTitleController : ControllerBase
    {
        private ApplicationDBContext _context;
        private readonly IServiceScopeFactory scopeFactory;

        public CreateDMTitleController(ApplicationDBContext context, IServiceScopeFactory sf)
        {
            _context = context;
            scopeFactory = sf;
        }

        [HttpPost]
        [Authorize]
        [ActionName("CreateDMTitle")]
        public IActionResult createTitle([FromBody] CreateDMTitle order)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);
            User receiver = new User();
            if (!user.open) return BadRequest(new { error = "YoureBanned" });

            if(!checkSenderAndReceiver(user, order.emailReceiver))
            {
                return BadRequest();
            }
            if(!getReceiver(ref receiver, order.emailReceiver))
            {
                return BadRequest();
            }
            try
            {
                DirectMessageTitle dm = new DirectMessageTitle
                {
                    Sender = user,
                    Receiver = receiver,
                    title = order.title
                };

                _context.Add(dm);
                _context.SaveChanges();
                using (var scope = scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                    User dbUser = dbContext.User.Where(u => u.id == user.id).First();
                    return Ok(LoadTitles.load(dbUser, dbContext));
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        private bool checkSenderAndReceiver(User sender, String recv)
        {
            bool sendToAdmin = recv == null;
            bool sentFromAdmin = AdminPolicy.isAdmin(sender, _context);

            if (sentFromAdmin && sendToAdmin)
            {
                return false;
            }
            if(!sentFromAdmin && !sendToAdmin)
            {
                return false;
            }

            return true;
        }

        private bool getReceiver(ref User receiver, String mail)
        {
            User recv = new User();

            if(mail == null)
            {
                List<User> admins = _context.User.Where(u => u.role == RoleManager.getAdmin(_context)).ToList();

                Random rand = new Random();
                int index = rand.Next(admins.Count());
                recv = admins[index];
            }
            else
            {
                List<User> posibleUsers = _context.User.Where(u => u.email == mail).ToList();
                
                if(posibleUsers.Count() != 1)
                {
                    return false;
                }
                recv = posibleUsers.First();
            }
            receiver = recv;

            return true;
        }
    }
}
