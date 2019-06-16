using API.Areas.GroupManage.Util;
using API.Data;
using API.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace API.Areas.Bet.Util
{
    public static class UserInFootballBet
    {
        public static bool check(User caller, ref Group group, string groupName, ref UserGroup ugCaller, ref FootballBet footballBet, string footballBetId, ApplicationDBContext _context, bool checkOnlyBet = true)
        {
            if (!UserInGroup.checkUserInGroup(caller.id, ref group, groupName, ref ugCaller, _context))
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

        private static bool getBet(ref FootballBet fb, string betId, Group group, ApplicationDBContext _context)
        {
            List<FootballBet> fbs = _context.FootballBets
                .Where(md => md.id.ToString() == betId && md.groupId == group.id).ToList();
            if (fbs.Count() != 1)
            {
                return false;
            }
            if (group.type)
            {
                return false;
            }

            fb = fbs.First();
            return true;
        }

        private static bool checkUserInBet(FootballBet fb, User caller, ApplicationDBContext _context)
        {
            var existBet = _context.UserFootballBet.Where(ufb => ufb.userId == caller.id && ufb.FootballBetId == fb.id && ufb.valid);
            if (existBet.Count() != 0)
            {
                return false;
            }
            return true;
        }
    }
}
