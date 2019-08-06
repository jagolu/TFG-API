using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Models
{
    public class MatchDay
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }


        public Competition Competition { get; set; }
        public Guid competitionid { get; set; }

        [Required]
        public DateTime date { get; set; }

        [Required]
        [MaxLength(32)]
        public string status { get; set; }

        [Required]
        public int number { get; set; }

        [MaxLength(64)]
        public String group { get; set; }

        public Team HomeTeam { get; set; }
        public Guid homeTeamId { get; set; }

        public Team AwayTeam { get; set; }
        public Guid awayTeamid { get; set; }

        public int? firstHalfHomeGoals { get; set; }
        public int? firstHalfAwayGoals { get; set; }
        public int? secondHalfHomeGoals { get; set; }
        public int? secondHalfAwayGoals { get; set; }
        public int? fullTimeHomeGoals { get; set; }
        public int? fullTimeAwayGoals { get; set; }

        public ICollection<FootballBet> bets { get; set; } = new HashSet<FootballBet>();
    }
}
