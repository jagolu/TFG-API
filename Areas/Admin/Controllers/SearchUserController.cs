﻿using System.Collections.Generic;
using System.Linq;
using API.Areas.Admin.Models;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static API.Areas.Admin.Models.UserSearchInfo;

namespace API.Areas.Admin.Controllers
{
    [Route("Admin/[action]")]
    [ApiController]
    public class SearchUserController : ControllerBase
    {
        private ApplicationDBContext _context;

        public SearchUserController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        [ActionName("GetAllUsers")]
        public List<UserSearchInfo> getAllGroups()
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);
            if (!AdminPolicy.isAdmin(user, _context)) return new List<UserSearchInfo>();
            List<UserSearchInfo> groupRet = new List<UserSearchInfo>();

            return addUsersToList(_context.User.ToList());
        }


        [HttpGet]
        [Authorize]
        [ActionName("SearchUser")]
        public List<UserSearchInfo> searchUser(string toFind)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);
            if (!AdminPolicy.isAdmin(user, _context)) return new List<UserSearchInfo>();

            //The request name is empty
            if (toFind == null || toFind.Length == 0)
            {
                return new List<UserSearchInfo>();
            }

            List<User> userWithSameMail = _context.User.Where(u =>
                u.email.ToLower().Contains(toFind.ToLower().Trim()) ||
                u.nickname.ToLower().Contains(toFind.ToLower().Trim())
            ).ToList();

            return addUsersToList(userWithSameMail);
        }



        private List<UserSearchInfo> addUsersToList(List<User> users)
        {
            List<UserSearchInfo> usersRet = new List<UserSearchInfo>();

            users.ForEach(user =>
            {
                _context.Entry(user).Collection("groups").Load();
                List<UserInGroup> uGroups = new List<UserInGroup>();

                user.groups.ToList().ForEach(g =>
                {
                    _context.Entry(g).Reference("Group").Load();
                    _context.Entry(g).Reference("role").Load();
                    uGroups.Add(new UserInGroup
                    {
                        groupName = g.Group.name,
                        role = g.role.name,
                        blocked = g.blocked,
                        joinTime = g.dateJoin,
                        roleTime = g.dateRole
                    });
                });

                usersRet.Add(new UserSearchInfo
                {
                    publicUserId = user.publicId,
                    email = user.email,
                    username = user.nickname,
                    open = user.open,
                    dateSignUp = user.dateSignUp,
                    groups = uGroups
                });
            });

            return usersRet;
        }
    }
}