using System;

namespace API.Areas.Bet.Models
{
    public class HistoryUserFootballBet
    {
        public HistoryUserFootballBet(API.Data.Models.UserFootballBet ub, API.Data.ApplicationDBContext _context, bool ended)
        {
            _context.Entry(ub).Reference("User").Load();
            this.username = ub.User.nickname;
            this.valid = ub.valid;
            this.dateInvalid = ub.dateInvalid;
            if (ended)
            {
                this.bet = ub.bet;
                this.homeGoals = ub.homeGoals;
                this.awayGoals = ub.awayGoals;
                this.winner = ub.winner;
                this.dateDone = ub.dateDone;
                this.earnings = ub.earnings;
            }
        }
        public string username { get; set; }
        public int? bet { get; set; }
        public int? homeGoals { get; set; }
        public int? awayGoals { get; set; }
        public int? winner { get; set; }
        public DateTime dateDone { get; set; }
        public bool valid { get; set; }
        public DateTime dateInvalid { get; set; }
        public int? earnings { get; set; }
    }
}
