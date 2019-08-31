using System;
using System.ComponentModel.DataAnnotations;

namespace API.Areas.Bet.Models
{
    /// <summary>
    /// Necessary info to launch a new fb
    /// </summary>
    public class LaunchFootballBet
    {
        [Required]
        /// <value>The name of the group where the new fb is going to be launched</value>
        public string groupName { get; set; }

        [Required]
        /// <value>The id of the match of the fb</value>
        public string matchday { get; set; }

        [Required]
        /// <value>The id of the type of the bet</value>
        public string typeBet { get; set; }

        [Required]
        /// <value>The id of the pay type</value>
        public string typePay { get; set; }

        [Required]
        /// <value>The min of the bet</value>
        public int minBet { get; set; }

        [Required]
        /// <value>The max of the bet</value>
        public int maxBet { get; set; }

        [Required]
        /// <value>The time until the users can do a user fb</value>
        public DateTime lastBetTime { get; set; }
    }
}
