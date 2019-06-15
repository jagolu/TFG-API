using API.Data;
using API.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace API.Areas.Bet.Models
{
    public class EndedFootballBet
    {
        public EndedFootballBet(User caller, FootballBet bet, ApplicationDBContext _context, bool includeResults)
        {
            _context.Entry(bet).Reference("typePay").Load();
            _context.Entry(bet).Collection("userBets").Load();
            var userBet = bet.userBets.Where(b => b.userId == caller.id);

            this.bet = new GroupBet(bet, _context, includeResults);
            if (userBet.Count() != 0)
            {
                this.ownBet = new List<HistoryUserFootballBet>();
                bet.userBets.Where(b => b.userId == caller.id).OrderByDescending(bb => bb.dateDone).ToList().ForEach(bb =>
                {
                    this.ownBet.Add(new HistoryUserFootballBet(bb, _context, true));
                });
            }
            else this.ownBet = null;
            if (bet.ended)
            {
                this.users = new List<OtherUserBets>();
                _context.Entry(bet).Reference("Group").Load();
                _context.Entry(bet.Group).Collection("users").Load();
                //See all the users of the group
                bet.Group.users.Where(g => g.userId!= caller.id).ToList().ForEach(userGroup =>
                {
                    List<UserFootballBet> anotherUserBets = bet.userBets.Where(b => b.userId == userGroup.userId)
                        .OrderByDescending(bb => bb.dateDone).ToList();

                    if (anotherUserBets.Count() != 0)
                    {
                        _context.Entry(userGroup).Reference("User").Load();
                        List<HistoryUserFootballBet> otherUserBetsHistory = new List<HistoryUserFootballBet>();
                        bool winner = false;
                        anotherUserBets.ForEach(ub =>
                        {
                            winner = !winner && ub.earnings > 0 ? true : false;
                            otherUserBetsHistory.Add(new HistoryUserFootballBet(ub, _context, bet.ended));
                        });
                        this.users.Add(new OtherUserBets { username = userGroup.User.nickname, winner = winner, bets = otherUserBetsHistory });
                    }
                });
            }
            else this.users = null;
        }

        public GroupBet bet { get; set; }
        public List<OtherUserBets> users { get; set; }
        public List<HistoryUserFootballBet> ownBet { get; set; }
    }
}