using System;
using System.ComponentModel.DataAnnotations;

namespace API.Areas.Bet.Models
{
    public class LaunchFootballBet
    {
        [Required]
        public string groupName { get; set; }

        [Required]
        public string matchday { get; set; }

        [Required]
        public string typeBet { get; set; }

        [Required]
        public string typePay { get; set; }

        [Required]
        public int minBet { get; set; }

        [Required]
        public int maxBet { get; set; }

        [Required]
        public DateTime lastBetTime { get; set; }
    }
}
