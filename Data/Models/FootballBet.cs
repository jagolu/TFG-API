using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Models
{
    /// <summary>
    /// A football bet in a group
    /// </summary>
    public class FootballBet
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        /// <value>The id of the fb</value>
        public Guid id { get; set; }

        /// <value>The id of the matchday of the bet</value>
        public Guid matchdayid { get; set; }
        
        /// <value>The matchday of the bet</value>
        public MatchDay MatchDay { get; set; }

        /// <value>The id of the group where the fb belongs to</value>
        public Guid groupid { get; set; }
        
        /// <value>The group where the fb belongs to</value>
        public Group Group { get; set; }

        [Required]
        /// <value>The type of the fb</value>
        public TypeFootballBet type { get; set; }

        [Required]
        /// <value>The pay type of the fb</value>
        public TypePay typePay { get; set; }

        [Required]
        /// <value>The min coins of the bet</value>
        public int minBet { get; set;}

        [Required]
        /// <value>The max coins of the bet</value>
        public int maxBet { get; set; }

        [Required]
        /// <value>The win rate of the bet</value>
        public double winRate { get; set; }

        [Required]
        /// <value>The date when the fb was released</value>
        public DateTime dateReleased { get; set; } = DateTime.Now;

        [Required]
        /// <value>The date until the users can bet</value>
        public DateTime dateLastBet { get; set; }

        [Required]
        /// <value>The date when the match ends</value>
        public DateTime dateEnded { get; set; }

        [Required]
        /// <value>True if the bet has ended, false otherwise</value>
        public bool ended { get; set; } = false;

        /// <value>The date when the fb was cancelled</value>
        public DateTime dateCancelled { get; set; }

        [Required]
        /// <value>True if the fb is cancelled, false otherwise</value>
        public bool cancelled { get; set; } = false;

        [Required]
        /// <value>The bets done on the fb</value>
        public int usersJoined { get; set; } = 0;


        /// <value>The user bets on this fb</value>
        public ICollection<UserFootballBet> userBets { get; set; } = new HashSet<UserFootballBet>();
    }
}
