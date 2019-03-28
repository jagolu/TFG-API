using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }

        [Required]
        [MaxLength]
        [MinLength(4)]
        public string email { get; set; }

        [Required]
        [StringLength(maximumLength:20, MinimumLength =4)]
        public string nickname { get; set; }
        
        [MaxLength]
        public string password { get; set; }

        [Required]
        public Boolean open { get; set; } = true;

        public string profileImg { get; set; } = null;

        [MaxLength]
        public string tokenValidation { get; set; } = Guid.NewGuid().ToString("N");

        [Required]
        public DateTime dateSignUp { get; set; } = DateTime.Today;

        [Required]
        public Role role { get; set; }

        [Required]
        public ICollection<UserToken> tokens { get; set; } = new HashSet<UserToken>();


        public ICollection<UserGroup> groups { get; set; } = new HashSet<UserGroup>();
    }
}
