using System.ComponentModel.DataAnnotations;

namespace API.Areas.Bet.Models
{
    /// <summary>
    /// Necesary info to do a user fb
    /// </summary>
    public class DoFootballBet
    {
        [Required]
        /// <value>The group where the fb belongs to</value>
        public string groupName { get; set; }

        [Required]
        /// <value>The id of the fb</value>
        public string footballbet { get; set; }

        [Required]
        /// <value>The coins bet by the user</value>
        public int bet { get; set; }

        /// <value>The home goals guessed by the user</value>
        public int? homeGoals { get; set; }
        
        /// <value>The away goals guessed by the user</value>
        public int? awayGoals { get; set; }
        
        /// <value>The winner guessed by the user</value>
        public int? winner { get; set; }
    }
}
