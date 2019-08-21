using System;
using System.Collections.Generic;

namespace API.Areas.Bet.Models
{
    public class FootballMatch
    {
        public string competition { get; set; }
        public string match_name { get; set; }
        public DateTime date { get; set; }
        public string matchday { get; set; }
        public List<string> allowedTypeBets { get; set; }
    }
}
