using API.Areas.GroupManage.Models;
using API.Data;
using API.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace API.Areas.Bet.Models
{
    public class EndedFootballBet
    {
        public EndedFootballBet(User caller, FootballBet bet, ApplicationDBContext _context)
        {
            _context.Entry(bet).Reference("typePay").Load();
            _context.Entry(bet).Collection("userBets").Load();
            var userBet = bet.userBets.Where(b => b.userId == caller.id);
            bool groupbet = bet.typePay.name.Contains("GROUP");

            this.bet = new GroupBet(bet, _context);
            this.usersJoined = groupbet ? bet.userBets.Count() : 0;
            this.ownBet = userBet.Count() == 1 ? new HistoryUserFootballBet(userBet.First(), _context) : null;
            if (!groupbet)
            {
                this.users = new List<HistoryUserFootballBet>();
                bet.userBets.Where(b => b.userId != caller.id).ToList().ForEach(bb =>
                {
                    this.users.Add(new HistoryUserFootballBet(bb, _context));
                });
            }
            else this.users = null;
        }

        public GroupBet bet { get; set; }
        public int usersJoined { get; set; }
        public List<HistoryUserFootballBet> users { get; set; }
        public HistoryUserFootballBet ownBet { get; set; }
    }
}
