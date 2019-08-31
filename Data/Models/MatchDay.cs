using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Models
{
    /// <summary>
    /// A football match
    /// </summary>
    public class MatchDay
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        /// <value>The id of the matchday</value>
        public Guid id { get; set; }

        /// <value>The competition where the match belongs to</value>
        public Competition Competition { get; set; }
        
        /// <value>The id of the competition where the match belongs to</value>
        public Guid competitionid { get; set; }

        [Required]
        /// <value>The date when the match is played</value>
        public DateTime date { get; set; }

        [Required]
        [MaxLength(32)]
        /// <value>The status of the match</value>
        public string status { get; set; }

        [Required]
        /// <value>The number of the matchday</value>
        public int number { get; set; }

        [Required]
        /// <value>The season when the match is played</value>
        public int season { get; set; }

        [MaxLength(64)]
        /// <value>The group of the matchday (if is a cup competition)</value>
        public String group { get; set; }

        /// <value>The home team of the match</value>
        public Team HomeTeam { get; set; }
        
        /// <value>The id of the home team of the match</value>
        public Guid homeTeamId { get; set; }

        /// <value>The away team of the match</value>
        public Team AwayTeam { get; set; }
        
        /// <value>The id of the away team of the match</value>
        public Guid awayTeamid { get; set; }

        /// <value>The count of goals scored by the home team in the first part</value>
        public int? firstHalfHomeGoals { get; set; }

        /// <value>The count of goals scored by the away team in the first part</value>
        public int? firstHalfAwayGoals { get; set; }

        /// <value>The count of goals scored by the home team in the secont part</value>
        public int? secondHalfHomeGoals { get; set; }

        /// <value>The count of goals scored by the away team in the second match</value>
        public int? secondHalfAwayGoals { get; set; }

        /// <value>The count of goals scored by the home team in the full match</value>
        public int? fullTimeHomeGoals { get; set; }

        /// <value>The count of goals scored by the away team in the full match</value>
        public int? fullTimeAwayGoals { get; set; }


        /// <value>The bets done on that match</value>
        public ICollection<FootballBet> bets { get; set; } = new HashSet<FootballBet>();
    }
}
