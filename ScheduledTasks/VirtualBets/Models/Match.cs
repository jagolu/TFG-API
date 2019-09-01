namespace API.ScheduledTasks.VirtualBets.Models
{
    /// <summary>
    /// A match from the football api
    /// </summary>
    public class Match
    {
        /// <value>The season of the match</value>
        public Season season { get; set; }
        
        /// <value>The date when the match is played</value>
        public string utcDate { get; set; }
        
        /// <value>The status of the match</value>
        public string status { get; set; }
        
        /// <value>The number of matchday of the match</value>
        public int? matchday { get; set; }
        
        /// <value>The stage of the match</value>
        public string stage { get; set; }
        
        /// <value>The group of the match</value>
        public string group { get; set; }
        
        /// <value>The score of the match</value>
        public Score score { get; set; }
        
        /// <value>The info of the home team</value>
        public HomeTeam homeTeam { get; set; }
        
        /// <value>The info of the away team</value>
        public AwayTeam awayTeam { get; set; }
        
        /// <summary>
        /// The score of a match
        /// </summary>
        public class Score
        {
            /// <value>The duration of the match</value>
            public string duration { get; set; }
            
            /// <value>The goals on the full time of the match</value>
            public TimeGoals fullTime { get; set; }
            
            /// <value>The goals on the first half of the match</value>
            public TimeGoals halfTime { get; set; }
            
            /// <value>The result of the penalties time</value>
            public Penalties penalties { get; set; }

            /// <summary>
            /// The goals on a time of the match
            /// </summary>
            public class TimeGoals
            {
                /// <value>The number of goals of the home team</value>
                public int? homeTeam { get; set; }
                
                /// <value>The number of goals of the away team</value>
                public int? awayTeam { get; set; }
            }

            /// <summary>
            /// The result of the penalties time
            /// </summary>
            public class Penalties
            {
                /// <value>The penalties scored by the home team</value>
                public int? homeTeam { get; set; }
                
                /// <value>The penalties scored by the away team</value>
                public int? awayTeam { get; set; }
            }
        }

        /// <summary>
        /// The info of the home team
        /// </summary>
        public class HomeTeam
        {
            /// <value>The name of the team</value>
            public string name { get; set; }
        }

        /// <summary>
        /// The name of the away team
        /// </summary>
        public class AwayTeam
        {
            /// <value>The name of the team</value>
            public string name { get; set; }
        }

        /// <summary>
        /// The info of the season of a match
        /// </summary>
        public class Season
        {
            /// <value>The id of the season</value>
            public int id { get; set; }
        }
    }
}
