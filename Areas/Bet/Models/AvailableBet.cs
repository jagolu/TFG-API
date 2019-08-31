using System.Collections.Generic;

namespace API.Areas.Bet.Models
{
    /// <summary>
    /// The available bets in a competition
    /// </summary>
    public class AvailableBet
    {
        /// <value>The name of the comepetition</value>
        public string competition { get; set; }
        
        /// <value>The available matchs in that competition</value>
        public List<FootballMatch> matches { get; set; }
    }
}
