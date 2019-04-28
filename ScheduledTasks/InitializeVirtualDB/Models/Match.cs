﻿namespace API.ScheduledTasks.InitializeVirtualDB.Models
{
    public class Match
    {
        public string utcDate { get; set; }
        public string status { get; set; }
        public int? matchday { get; set; }
        public string stage { get; set; }
        public string group { get; set; }
        public Score score { get; set; }
        public HomeTeam homeTeam { get; set; }
        public AwayTeam awayTeam { get; set; }
        
        public class Score
        {
            public string duration { get; set; }
            public FullTime fullTime { get; set; }
            public Penalties penalties { get; set; }


            public class FullTime
            {
                public int? homeTeam { get; set; }
                public int? awayTeam { get; set; }
            }
            public class Penalties
            {
                public int? homeTeam { get; set; }
                public int? awayTeam { get; set; }
            }
        }

        public class HomeTeam
        {
            public string name { get; set; }
        }

        public class AwayTeam
        {
            public string name { get; set; }
        }
    }
}
