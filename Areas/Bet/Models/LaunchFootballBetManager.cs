using System.Collections.Generic;

namespace API.Areas.Bet.Models
{
    public class LaunchFootballBetManager
    {
        public List<NameWinRate> typeBets { get; set; }
        public List<NameWinRate> typePays { get; set; }
        public List<AvailableBet> competitionMatches { get; set; }
    }
}
