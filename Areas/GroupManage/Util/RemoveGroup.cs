using API.Data;
using API.Data.Models;
using System.Linq;

namespace API.Areas.GroupManage.Util
{
    public static class RemoveGroup
    {
        public static void Remove(Group group, ApplicationDBContext _context)
        {
            _context.Entry(group).Collection("users").Load();
            _context.Entry(group).Collection("bets").Load();
            group.bets.ToList().ForEach(bet =>
            {
                _context.Entry(bet).Collection("userBets").Load();
                _context.UserFootballBet.RemoveRange(
                    _context.UserFootballBet.Where(fb => fb.FootballBetId == bet.id)
                );
                _context.Remove(bet);
            });
            _context.RemoveRange(
                _context.UserGroup.Where(ug => ug.groupId == group.id)
            );

            _context.Remove(group);
            _context.SaveChanges();
        }
    }
}
