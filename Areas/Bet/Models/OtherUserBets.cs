using System.Collections.Generic;

namespace API.Areas.Bet.Models
{
    /// <summary>
    /// The info of other user fb
    /// </summary>
    public class OtherUserBets
    {
        /// <value>The username of the user who did the other user fb</value>
        public string username { get; set; }
        
        /// <value>true if that user fb was a winner-user-fb, false otherwise</value>
        public bool winner { get; set; }
        
        /// <value>A list with all the user bets done on the same fb</value>
        public List<HistoryUserFootballBet> bets { get; set; }
    }
}
