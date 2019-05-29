using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Models
{
    public class FootballBet
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }

        public Guid matchdayId { get; set; }
        public MatchDay MatchDay { get; set; }

        public Guid groupId { get; set; }
        public Group Group { get; set; }

        [Required]
        public int minBet { get; set;}

        [Required]
        public int maxBet { get; set; }

        [Required]
        public double ganancia { get; set; }

        [Required]
        public DateTime dateReleased { get; set; } = DateTime.Now;

        [Required]
        public DateTime dateLastBet { get; set; }

        [Required]
        public DateTime dateChanges { get; set; }
    }
}
