using System.Collections.Generic;

namespace API.ScheduledTasks.InitializeVirtualDB.Models
{
    public class CompetitionMatches
    {
        public Competition competition { get; set; }
        public List<Match> matches { get; set; }
        public int actualNumber { get; set; }

        public class Competition
        {
            public string name { get; set; }
        }
    }
}
