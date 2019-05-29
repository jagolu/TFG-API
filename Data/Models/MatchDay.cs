using System;
using System.ComponentModel.DataAnnotations;

namespace API.Data.Models
{
    public class MatchDay
    {
        public Competition Competition { get; set; }
        public Guid CompetitionId { get; set; }

        [Required]
        public DateTime date { get; set; }

        [Required]
        public string status { get; set; }

        [Required]
        public int number { get; set; }

        public String group { get; set; }

        public Team HomeTeam { get; set; }
        public Guid HomeTeamId { get; set; }

        public Team AwayTeam { get; set; }
        public Guid AwayTeamId { get; set; }

        public int? homeGoals { get; set; }
        public int? awayGoals { get; set; }

        public int? homeEndPenalties { get; set; }
        public int? awayEndPenalties { get; set; }
    }
}
