namespace API.Areas.Bet.Models
{
    public class HistoryUserFootballBet
    {
        public HistoryUserFootballBet(API.Data.Models.UserFootballBet ub, API.Data.ApplicationDBContext _context)
        {
            _context.Entry(ub).Reference("User").Load();
            this.username = ub.User.nickname;
            this.bet = ub.bet;
            this.homeGoals = ub.homeGoals;
            this.awayGoals = ub.awayGoals;
            this.winner = ub.winner;
        }
        public string username { get; set; }
        public int? bet { get; set; }
        public int? homeGoals { get; set; }
        public int? awayGoals { get; set; }
        public int? winner { get; set; }
    }
}
