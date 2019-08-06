using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Models
{
    public class UserFootballBet
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }

        public Guid footballBetid { get; set; }
        public FootballBet FootballBet { get; set; }

        public Guid userid { get; set; }
        public User User { get; set; }

        [Required]
        public int bet { get; set; }

        public int? winner { get; set; }

        public int? homeGoals { get; set; }
        public int? awayGoals { get; set; }

        [Required]
        public DateTime dateDone { get; set; } = DateTime.Now;

        public DateTime dateInvalid { get; set; }

        [Required]
        public bool valid { get; set; } = true;

        [Required]
        public int earnings { get; set; } = 0;
    }
}
