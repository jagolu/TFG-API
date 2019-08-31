using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Models
{
    /// <summary>
    /// A football team
    /// </summary>
    public class Team
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        /// <value>The id of a team</value>
        public Guid id { get; set; }

        [Required]
        [MaxLength(64)]
        /// <value>The name of the team</value>
        public string name { get; set; }


        /// <value>The matchs which the team plays as away team</value>
        public ICollection<MatchDay> awayMatchDays { get; set; } = new HashSet<MatchDay>();
        
        /// <value>The matchs which the team plays as home team</value>
        public ICollection<MatchDay> homeMatchDays { get; set; } = new HashSet<MatchDay>();
    }
}
