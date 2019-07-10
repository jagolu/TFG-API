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
            Role maker = _context.Role.Where(r => r.name == "GROUP_MAKER").First();
            group.bets.ToList().ForEach(bet =>
            {
                _context.Entry(bet).Collection("userBets").Load();
                _context.UserFootballBet.RemoveRange(_context.UserFootballBet.Where(fb => fb.FootballBetId == bet.id));
                //_context.Remove(bet);
            });

            _context.UserGroup.Where(ug => ug.groupId == group.id).ToList().ForEach(us =>
            {
                _context.Entry(us).Reference("User").Load();
                _context.Entry(us).Reference("role").Load();
                if (!us.blocked) Home.Util.GroupNew.launch(us.User, group, Home.Models.TypeGroupNew.REMOVE_GROUP, us.role == maker, _context);
            });

            _context.RemoveRange(group.bets);
            _context.RemoveRange(group.users);
            _context.Remove(group);
            _context.SaveChanges();
        }
    }
}
