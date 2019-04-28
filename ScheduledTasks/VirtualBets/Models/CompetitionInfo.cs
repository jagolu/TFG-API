namespace API.ScheduledTasks.VirtualBets.Models
{
    public class CompetitionInfo
    {
        public CurrentSeason currentSeason { get; set; }

        public class CurrentSeason
        {
            public int currentMatchday { get; set; }
        }
    }
}
