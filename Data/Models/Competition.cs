using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Models
{
    public class Competition
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }

        [Required]
        [MaxLength(64)]
        public string name { get; set; }

        public int actualMatchDay { get; set; } = 0;


        public ICollection<MatchDay> matchDays { get; set; } = new HashSet<MatchDay>();
    }
}
