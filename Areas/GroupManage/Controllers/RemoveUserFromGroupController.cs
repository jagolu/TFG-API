﻿using System;
using API.Areas.GroupManage.Models;
using API.Areas.GroupManage.Util;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Areas.GroupManage.Controllers
{
    [Route("Group/[action]")]
    [ApiController]
    public class RemoveUserFromGroupController : ControllerBase
    {
        private ApplicationDBContext _context;

        public RemoveUserFromGroupController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        [ActionName("RemoveUser")]
        public IActionResult removeUser([FromBody] KickUser order)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context); //The user who tries to kick the user from the group
            UserGroup targetUser = new UserGroup();
            Group group = new Group();

            if (!GroupUserManager.CheckUserGroup(user, ref group, order.groupName, ref targetUser, order.publicId, _context, TypeCheckGroupUser.REMOVE_USER, false))
            {
                return BadRequest(new { error = "" });
            }

            try
            {
                _context.Remove(targetUser);
                _context.SaveChanges();
                group.users.Remove(targetUser);

                //TODO el contexto no se actualiza, y el usuario que se ha eliminado sigue
                //apareciendo en el array de UserGroup de este grupo WTF
                //group = _context.Group.Where(g => g.name == order.groupName).First();

                return Ok(GroupPageManager.GetPage(user, group, _context));
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}