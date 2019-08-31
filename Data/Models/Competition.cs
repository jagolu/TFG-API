using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Models
{
    /// <summary>
    /// A football competition
    /// </summary>
    public class Competition
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        /// <value>The id of the competition</value>
        public Guid id { get; set; }

        [Required]
        [MaxLength(64)]
        /// <value>The name of the competition</value>
        public string name { get; set; }

        /// <value>The actual matchday of the competition</value>
        public int actualMatchDay { get; set; } = 0;


        /// <value>The matchdays of the competition</value>
        public ICollection<MatchDay> matchDays { get; set; } = new HashSet<MatchDay>();
    }
}
