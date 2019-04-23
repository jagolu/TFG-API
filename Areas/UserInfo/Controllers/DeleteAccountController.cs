using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using API.Areas.UserInfo.Models;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Areas.UserInfo.Controllers
{
    [Route("User/[action]")]
    [ApiController]
    public class DeleteAccountController : ControllerBase
    {
        private ApplicationDBContext _context;

        public DeleteAccountController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        [ActionName("DeleteAccount")]
        public IActionResult deleteAccount([FromBody] DeleteUser userDelete)
        {
            string userDeletePass = (userDelete.password == null || userDelete.password.Length==0)
                ? null : PasswordHasher.hashPassword(userDelete.password);
            
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);

            _context.Entry(user).Reference("role").Load();

            if(user.password != userDeletePass) {
                return BadRequest(new { error = "CantDeleteAccount" });
            }

            bool beingAdmin = deleteAccountBeingAdmin(userDelete.email, user);
            bool beingNormal = deleteAccountBeingNormal(user.email, user);

            if(!beingAdmin && !beingNormal) {
                return BadRequest(new { error = "CantDeleteAccount" });
            }

            try {

                _context.SaveChanges();

            } catch (Exception){
                return StatusCode(500);
            }

            return Ok();
        }

        private Boolean deleteAccountBeingAdmin(string email, User u)
        {
            if(u.role.name != "ADMIN") {
                return false;
            }

            if(!isValidEmail(email)){
                return false;
            }

            _context.User.Remove(
                _context.User.Where(
                    uu => uu.email == email
                ).First()
            );


            return true;
        }

        private Boolean deleteAccountBeingNormal(string email, User u)
        {
            if(u.role.name != "NORMAL_USER") {
                return false;
            }

            User uToDelete = _context.User.Where(uu => uu.email == email).First();

            if (!canRemoveGroups(uToDelete)){
                return false;
            }

            _context.User.Remove(uToDelete);

            return true;
        }

        private bool isValidEmail(string email)
        {
            try {
                MailAddress m = new MailAddress(email);

                string e = _context.User.Where(u => u.email == email).First().email;

                return true;

            } catch (Exception) {
                return false;
            }
        }

        private bool canRemoveGroups(User u)
        {
            List<UserGroup> groups =_context.UserGroup.Where(ug => ug.userId == u.id).ToList();
            bool canRemove = true;

            groups.ForEach(
                g=>
                {
                    int n_members = _context.UserGroup.Where(ug => ug.groupId == g.groupId).Count();

                    if (n_members == 1) // The user in the group is the only member in
                    {
                        try
                        {
                            _context.Remove(g);

                            _context.SaveChanges();

                            _context.Remove(_context.Group.Where(group => group.id == g.groupId).First());

                            _context.SaveChanges();
                        }
                        catch (Exception)
                        {
                            canRemove = false;
                        }

                    }
                    else canRemove = false;

                }
            );
            return canRemove;
        }
    }
}
