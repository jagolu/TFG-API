using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Models
{
    /// <summary>
    /// A bet done by a user
    /// </summary>
    public class UserFootballBet
    {        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        /// <value>The id of the user fb</value>
        public Guid id { get; set; }

        /// <value>The id of the fb where the user did the bet</value>
        public Guid footballBetid { get; set; }
        
        /// <value>The fb which the user bet</value>
        public FootballBet FootballBet { get; set; }

        /// <value>The id of the user who did the bet</value>
        public Guid userid { get; set; }
        
        /// <value>The user who did the bet</value>
        public User User { get; set; }

        [Required]
        /// <value>The coins which the user bet</value>
        public int bet { get; set; }

        /// <value>The winner which the user supossed</value>
        public int? winner { get; set; }

        /// <value>The home goals which the user supossed</value>
        public int? homeGoals { get; set; }
        
        /// <value>The away goals which the user supossed</value>
        public int? awayGoals { get; set; }

        [Required]
        /// <value>The time when the user did the bet</value>
        public DateTime dateDone { get; set; } = DateTime.Now;

        /// <value>The time when the user cancelled the bet</value>
        public DateTime dateInvalid { get; set; }

        [Required]
        /// <value>True if the bet is valid, false if the bet has been cancelled</value>
        public bool valid { get; set; } = true;

        [Required]
        /// <value>The earnings of the user</value>
        public int earnings { get; set; } = 0;
    }
}
