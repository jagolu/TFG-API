using API.Areas.GroupManage.Util;
using API.Data;
using API.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace API.Areas.Bet.Util
{
    public static class UserInFootballBet
    {
        //
        // ──────────────────────────────────────────────────────────────────────────────────
        //   :::::: P U B L I C   F U N C T I O N S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Check if a user has bet in a fb
        /// </summary>
        /// <param name="caller">The user who wants do/cancel a user fb</param>
        /// <param name="group">A new group object, to save on it</param>
        /// <param name="groupName">The name of the group</param>
        /// <param name="ugCaller">A new UserGroup object, to save on it</param>
        /// <param name="footballBet">A new FootballBet object, to save on it</param>
        /// <param name="footballBetId">The id of the football bet</param>
        /// <param name="_context">The datbase context</param>
        /// <param name="checkOnlyBet">True to check if the user has bet on the fb, false otherwise</param>
        /// <returns></returns>
        public static bool check(User caller, ref Group group, string groupName, ref UserGroup ugCaller, ref FootballBet footballBet, string footballBetId, ApplicationDBContext _context, bool checkOnlyBet = true)
        {
            if (!UserFromGroup.isOnIt(caller.id, ref group, groupName, ref ugCaller, _context))
            {
                return false;
            }
            if (!getBet(ref footballBet, footballBetId, group, _context))
            {
                return false;
            }
            if (checkOnlyBet && !checkUserInBet(footballBet, caller, _context))
            {
                return false;
            }

            return true;
        }
        

        //
        // ────────────────────────────────────────────────────────────────────────────────────
        //   :::::: P R I V A T E   F U N C T I O N S : :  :   :    :     :        :          :
        // ────────────────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Check if the fb exists or not
        /// </summary>
        /// <param name="fb">A new football bet object, to save on it</param>
        /// <param name="betId">The fb id</param>
        /// <param name="group">The group where the fb belongs to</param>
        /// <param name="_context">The database context</param>
        /// <returns>True if the fb exists, false otherwise</returns>
        private static bool getBet(ref FootballBet fb, string betId, Group group, ApplicationDBContext _context)
        {
            List<FootballBet> fbs = _context.FootballBets
                .Where(md => md.id.ToString() == betId && md.groupid == group.id).ToList();
            if (fbs.Count() != 1)
            {
                return false;
            }

            fb = fbs.First();
            return true;
        }
        
        /// <summary>
        /// Checks if the user has bet on the fb
        /// </summary>
        /// <param name="fb">The fb to check</param>
        /// <param name="caller">The user who can be or not in the fb</param>
        /// <param name="_context">The database context</param>
        /// <returns>True if the user has bet on the fb, false otherwise</returns>
        private static bool checkUserInBet(FootballBet fb, User caller, ApplicationDBContext _context)
        {
            var existBet = _context.UserFootballBet.Where(ufb => ufb.userid == caller.id && ufb.footballBetid == fb.id && ufb.valid);
            if (existBet.Count() != 0)
            {
                return false;
            }
            return true;
        }
    }
}
