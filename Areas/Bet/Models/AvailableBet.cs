using System.Collections.Generic;

namespace API.Areas.Bet.Models
{
    public class AvailableBet
    {
        public string competition { get; set; }
        public List<FootBallMatch> matches { get; set; }
        public List<NameWinRate> allowedTypePays { get; set; }
    }
}
