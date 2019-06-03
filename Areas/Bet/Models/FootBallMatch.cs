using System;
using System.Collections.Generic;

namespace API.Areas.Bet.Models
{
    public class FootBallMatch
    {
        public string competition { get; set; }
        public string homeTeam { get; set; }
        public string awayTeam { get; set; }
        public DateTime date { get; set; }
        public List<String> allowedTypeBets { get; set; }
    }
}
