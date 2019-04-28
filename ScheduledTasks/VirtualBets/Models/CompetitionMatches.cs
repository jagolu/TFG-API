using System.Collections.Generic;

namespace API.ScheduledTasks.VirtualBets.Models
{
    public class CompetitionMatches
    {
        public Competition competition { get; set; }
        public List<Match> matches { get; set; }

        public class Competition
        {
            public string name { get; set; }
        }
    }
}
