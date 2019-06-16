using System;
using System.Collections.Generic;
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
        public TypeFootballBet type { get; set; }

        [Required]
        public TypePay typePay { get; set; }

        [Required]
        public int minBet { get; set;}

        [Required]
        public int maxBet { get; set; }

        [Required]
        public double winRate { get; set; }

        [Required]
        public DateTime dateReleased { get; set; } = DateTime.Now;

        [Required]
        public DateTime dateLastBet { get; set; }

        [Required]
        public bool ended { get; set; } = false;

        public DateTime dateCancelled { get; set; }

        [Required]
        public bool cancelled { get; set; } = false;



        public ICollection<UserFootballBet> userBets { get; set; } = new HashSet<UserFootballBet>();
    }
}
