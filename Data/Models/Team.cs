using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Models
{
    public class Team
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }

        [Required]
        [MaxLength(64)]
        public string name { get; set; }

        public ICollection<MatchDay> awayMatchDays { get; set; } = new HashSet<MatchDay>();
        public ICollection<MatchDay> homeMatchDays { get; set; } = new HashSet<MatchDay>();
    }
}
