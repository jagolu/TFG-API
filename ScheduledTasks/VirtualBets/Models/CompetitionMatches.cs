using System.Collections.Generic;

namespace API.ScheduledTasks.VirtualBets.Models
{
    /// <summary>
    /// The matches of the competition
    /// </summary>
    public class CompetitionMatches
    {
        /// <value>The info of the competition</value>
        public Competition competition { get; set; }
        
        /// <value>The matches of the competition</value>
        public List<Match> matches { get; set; }

        /// <summary>
        /// The info of a competition
        /// </summary>
        public class Competition
        {
            /// <value>The name of the competition</value>
            public string name { get; set; }
        }
    }
}
