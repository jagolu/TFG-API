namespace API.ScheduledTasks.VirtualBets.Models
{
    /// <summary>
    /// The info of a competition
    /// </summary>
    public class CompetitionInfo
    {
        /// <value>The current season of the competition</value>
        public CurrentSeason currentSeason { get; set; }

        /// <summary>
        /// The class to the current season of the competition
        /// </summary>
        public class CurrentSeason
        {
            /// <value>The current matchday of the competition</value>
            public int currentMatchday { get; set; }
        }
    }
}
