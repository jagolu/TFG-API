using System;
using System.Collections.Generic;
using System.Linq;
using API.Areas.UserInfo.Models;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
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
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);
            string userDeletePass = (userDelete.password == null || userDelete.password.Length==0)
                ? null : PasswordHasher.hashPassword(userDelete.password);
            
            
            if(user.password != userDeletePass) {
                return BadRequest(new { error = "CantDeleteAccount" });
            }

            if(!deleteAccountBeingNormal(user)) {
                return BadRequest(new { error = "CantDeleteAccount" });
            }

            try {
                _context.User.Remove(user);
                _context.SaveChanges();

            } catch (Exception){
                return StatusCode(500);
            }

            return Ok();
        }

        private bool deleteAccountBeingNormal(User u)
        {
            _context.Entry(u).Reference("role").Load();
            if(u.role != _context.Role.Where(r => r.name == "NORMAL_USER").First()) {
                return false;
            }

            if (!removeGroups(u)){
                return false;
            }

            return true;
        }

        private bool removeGroups(User user)
        {
            _context.Entry(user).Collection("groups").Load();
            bool canRemove = true;

            user.groups.ToList().ForEach(userGroup=>
            {
                List<UserGroup> members = _context.UserGroup.Where(ug => ug.groupId == userGroup.groupId && !ug.blocked).ToList();

                try
                {
                    if (members.Count() == 1) // The user in the group is the only member in
                    {
                        _context.Remove(userGroup);
                        _context.Remove(_context.Group.Where(g => g.id == userGroup.groupId).First());
                        _context.SaveChanges();
                    }
                    else
                    {
                        Role role_groupMaker = _context.Role.Where(r => r.name == "GROUP_MAKER").First();
                        Role role_groupAdmin = _context.Role.Where(r => r.name == "GROUP_ADMIN").First();
                        Role role_groupNormal = _context.Role.Where(r => r.name == "GROUP_NORMAL").First();
                       
                        //The user is a normal user or an admin in the group, the UserGroup entry is just deleted
                        if(userGroup.role != role_groupMaker)
                        {
                            _context.Remove(userGroup);
                            _context.SaveChanges();
                        } //The user is the group maker
                        else
                        {
                            List<UserGroup> adminMembers = members.Where(m => m.role == role_groupAdmin).OrderBy(d => d.dateRole).ToList();
                            List<UserGroup> normalMembers = members.Where(m => m.role == role_groupNormal).OrderBy(d => d.dateJoin).ToList();
                            UserGroup newMaster;

                            if(adminMembers.Count() != 0) //The older admin in the group will become in the group maker
                            {
                                newMaster = adminMembers.First();
                            }
                            else //If there isn't any admin, the older member in the group will become in the group make
                            {
                                newMaster = normalMembers.First();
                            }

                            newMaster.role = role_groupMaker;
                            newMaster.dateRole = DateTime.Today;

                            _context.Remove(userGroup);
                            _context.SaveChanges();
                        }
                    }
                }
                catch (Exception)
                {
                    canRemove = false;
                }
            });

            return canRemove;
        }
    }
}
