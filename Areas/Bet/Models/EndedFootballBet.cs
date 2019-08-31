using API.Data;
using API.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace API.Areas.Bet.Models
{
    /// <summary>
    /// The group football bets that has ended
    /// </summary>
    public class EndedFootballBet
    {
        //
        // ──────────────────────────────────────────────────────────────────────────
        //   :::::: C O N S T R U C T O R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="caller">The caller of the function</param>
        /// <param name="bet">The fb that has ended</param>
        /// <param name="_context">The database context</param>
        /// <param name="includeResults">True to include the results of the match, false otherwise</param>
        public EndedFootballBet(User caller, FootballBet bet, ApplicationDBContext _context, bool includeResults)
        {
            _context.Entry(bet).Reference("typePay").Load();
            _context.Entry(bet).Collection("userBets").Load();
            var userBet = bet.userBets.Where(b => b.userid == caller.id);

            this.bet = new GroupBet(bet, _context, includeResults);
            if (userBet.Count() != 0)
            {
                bool theUserHasWin = false;
                this.ownBet = new List<HistoryUserFootballBet>();
                bet.userBets.Where(b => b.userid == caller.id).OrderByDescending(bb => bb.dateDone).ToList().ForEach(bb =>
                {
                    this.ownBet.Add(new HistoryUserFootballBet(bb, _context, true));
                    theUserHasWin = !theUserHasWin && bb.earnings>0 && bb.valid ? true : theUserHasWin;
                });
                if (bet.ended) this.userWins = theUserHasWin;
            }
            else this.ownBet = null;
            if (bet.ended)
            {
                this.users = new List<OtherUserBets>();
                _context.Entry(bet).Reference("Group").Load();
                _context.Entry(bet.Group).Collection("users").Load();
                //See all the users of the group
                bet.Group.users.Where(g => g.userid!= caller.id).ToList().ForEach(userGroup =>
                {
                    List<UserFootballBet> anotherUserBets = bet.userBets.Where(b => b.userid == userGroup.userid)
                        .OrderByDescending(bb => bb.dateDone).ToList();

                    if (anotherUserBets.Count() != 0)
                    {
                        _context.Entry(userGroup).Reference("User").Load();
                        List<HistoryUserFootballBet> otherUserBetsHistory = new List<HistoryUserFootballBet>();
                        bool winner = false;
                        anotherUserBets.ForEach(ub =>
                        {
                            winner = !winner && ub.earnings > 0 && ub.valid ? true : winner;
                            otherUserBetsHistory.Add(new HistoryUserFootballBet(ub, _context, bet.ended));
                        });
                        this.users.Add(new OtherUserBets { username = userGroup.User.nickname, winner = winner, bets = otherUserBetsHistory });
                    }
                });
            }
            else this.users = null;
        }


        //
        // ──────────────────────────────────────────────────────────────────────
        //   :::::: C L A S S   V A R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────
        //

        /// <value>The info of the bet</value>
        public GroupBet bet { get; set; }
        
        /// <value>The user fb done by other user that the caller</value>
        public List<OtherUserBets> users { get; set; }
        
        /// <value>The user fb done by the caller on that fb</value>
        public List<HistoryUserFootballBet> ownBet { get; set; }
        
        /// <value>True if the caller won that fb</value>
        public bool? userWins { get; set; }
    }
}