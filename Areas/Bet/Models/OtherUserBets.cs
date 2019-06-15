using System.Collections.Generic;

namespace API.Areas.Bet.Models
{
    public class OtherUserBets
    {
        public string username { get; set; }
        public bool winner { get; set; }
        public List<HistoryUserFootballBet> bets { get; set; }
    }
}
