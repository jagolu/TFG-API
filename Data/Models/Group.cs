using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Models
{
    public class Group
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(20)]
        public string name { get; set; }

        [Required] //1-->Apuestas oficiales 0-->Apuestas virtuales
        public Boolean type { get; set; }

        [MaxLength]
        public string password { get; set; } = null;

        [Required]
        public bool canPutPassword { get; set; } = false;

        [Required]
        public Boolean open { get; set; } = true;

        [Required]
        public int capacity { get; set; } = 7;

        [Required]
        public DateTime dateCreated { get; set; } = DateTime.Today;

        public ICollection<UserGroup> users { get; set; } = new HashSet<UserGroup>();
        public ICollection<FootballBet> bets { get; set; } = new HashSet<FootballBet>();
    }
}
