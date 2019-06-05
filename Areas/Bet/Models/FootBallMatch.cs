using System;
using System.Collections.Generic;

namespace API.Areas.Bet.Models
{
    public class FootBallMatch
    {
        public string competition { get; set; }
        public string match_name { get; set; }
        public DateTime date { get; set; }
        public string matchday { get; set; }
        public List<NameWinRate> allowedTypeBets { get; set; }
    }
}
