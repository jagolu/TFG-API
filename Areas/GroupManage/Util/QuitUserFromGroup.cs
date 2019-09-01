using API.Areas.Alive.Util;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Areas.GroupManage.Util
{
    public static class QuitUserFromGroup
    {
        //
        // ──────────────────────────────────────────────────────────────────────────────────
        //   :::::: P U B L I C   F U N C T I O N S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Removes a member of a group from it
        /// </summary>
        /// <param name="userGroup">The user of the group</param>
        /// <param name="_context">The database context</param>
        /// <param name="hub">The notification hub</param>
        /// <returns>True if the process was good, false otherwise</returns>
        public static async Task<bool> quitUser(UserGroup userGroup, ApplicationDBContext _context, IHubContext<NotificationHub> hub)
        {
            List<UserGroup> members = getValidUsersInGroup(userGroup, _context);

            try
            {
                removeBets(userGroup, _context);

                if (members.Count() == 1) // The user in the group is the only member in
                {
                    _context.Entry(userGroup).Reference("Group").Load();
                    RemoveGroup.remove(userGroup.Group, _context, hub);
                }
                else
                {
                    Role role_groupMaker = RoleManager.getGroupMaker(_context);
                    Role role_groupAdmin = RoleManager.getGroupAdmin(_context);
                    Role role_groupNormal = RoleManager.getGroupNormal(_context);

                    //The user is a normal user or an admin in the group, the UserGroup entry is just deleted
                    if (userGroup.role == role_groupMaker)
                    {
                        await manageQuitMaker(members, role_groupMaker, role_groupAdmin, role_groupNormal, true, _context, hub);
                    }

                    _context.Remove(userGroup);
                    _context.SaveChanges();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Get the valid users in a group
        /// </summary>
        /// <param name="caller">The caller of the function (must be a member of the group)</param>
        /// <param name="_context">The database context</param>
        /// <returns>The list of members of the group</returns>
        public static List<UserGroup> getValidUsersInGroup(UserGroup caller, ApplicationDBContext _context)
        {
            return _context.UserGroup.Where(ug => 
                                ug.groupid == caller.groupid && 
                                !ug.blocked &&
                                ug.User.open
                    ).ToList();
        }

        /// <summary>
        /// Manages the action when the maker of a group leaves it
        /// </summary>
        /// <param name="members">All the members of the group</param>
        /// <param name="maker">The group-maker role</param>
        /// <param name="admin">The group-admin role</param>
        /// <param name="normal">The group-normal role</param>
        /// <param name="leave">True if the maker has leaved the group, false otherwise</param>
        /// <param name="_context">The database context</param>
        /// <param name="hub">The notification hub</param>
        public static async Task manageQuitMaker(List<UserGroup> members, Role maker, Role admin, Role normal, bool leave, ApplicationDBContext _context, IHubContext<NotificationHub> hub)
        {
            List<UserGroup> adminMembers = members.Where(m => m.role == admin).OrderBy(d => d.dateRole).ToList();
            List<UserGroup> normalMembers = members.Where(m => m.role == normal).OrderBy(d => d.dateJoin).ToList();
            UserGroup newMaster;

            if (adminMembers.Count() != 0) //The older admin in the group will become in the group maker
            {
                newMaster = adminMembers.First();
            }
            else //If there isn't any admin, the older member in the group will become in the group make
            {
                newMaster = normalMembers.First();
            }

            newMaster.role = maker;
            newMaster.dateRole = DateTime.Today;

            _context.Update(newMaster);
            _context.SaveChanges();

            _context.Entry(newMaster).Reference("User").Load();
            _context.Entry(newMaster).Reference("Group").Load();
            Home.Util.GroupNew.launch(newMaster.User, newMaster.Group, null, Home.Models.TypeGroupNew.MAKE_MAKER_GROUP, leave, _context);
            Home.Util.GroupNew.launch(newMaster.User, newMaster.Group, null, Home.Models.TypeGroupNew.MAKE_MAKER_USER, leave, _context);
            await SendNotification.send(hub, newMaster.Group.name, newMaster.User, Alive.Models.NotificationType.MAKE_MAKER, _context);
        }


        //
        // ────────────────────────────────────────────────────────────────────────────────────
        //   :::::: P R I V A T E   F U N C T I O N S : :  :   :    :     :        :          :
        // ────────────────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Removes the u fb of a user in a group
        /// </summary>
        /// <param name="ug">The member of the group</param>
        /// <param name="context">The database context</param>
        private static void removeBets(UserGroup ug, ApplicationDBContext context)
        {
            context.Entry(ug).Reference("User").Load();
            context.Entry(ug.User).Collection("footballBets").Load();
            ug.User.footballBets.ToList().ForEach(ufb =>
            {
                context.Entry(ufb).Reference("FootballBet").Load();
                if(ufb.FootballBet.groupid == ug.groupid)
                {
                    context.Remove(ufb);
                }
            });
            context.SaveChanges();
        }
    }
}
