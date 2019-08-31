using System.Collections.Generic;

namespace API.Areas.Bet.Models
{
    /// <summary>
    /// The info of the football bet page with the allowed matchdays 
    /// </summary>
    public class LaunchFootballBetManager
    {
        /// <value>All the bet types in the database</value>
        public List<NameWinRate> typeBets { get; set; }
        
        /// <value>All the pay types in the database</value>
        public List<NameWinRate> typePays { get; set; }
        
        /// <value>All the matchdays availables</value>
        public List<AvailableBet> competitionMatches { get; set; }
    }
}
