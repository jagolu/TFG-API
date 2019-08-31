using System;
using System.Collections.Generic;

namespace API.Areas.Bet.Models
{
    /// <summary>
    /// The info of a football match
    /// </summary>
    public class FootballMatch
    {
        /// <value>The name of the competition</value>
        public string competition { get; set; }
        
        /// <value>The name of the match</value>
        public string match_name { get; set; }
        
        /// <value>The time when the match starts</value>
        public DateTime date { get; set; }
        
        /// <value>The id of the matchday</value>
        public string matchday { get; set; }
        
        /// <value>A list of allowed bets on this match</value>
        public List<string> allowedTypeBets { get; set; }
    }
}
