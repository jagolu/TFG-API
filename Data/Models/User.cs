using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Models
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }

        [Required]
        [MaxLength]
        public string publicId { get; set; } = Guid.NewGuid().ToString();

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

        public Byte[] profileImg { get; set; } = null;

        [MaxLength]
        public string tokenValidation { get; set; } = Guid.NewGuid().ToString();

        [MaxLength]
        public string tokenPassword { get; set; } = null;

        public DateTime tokenPassword_expirationTime { get; set; } = new DateTime(1,1,1);

        [Required]
        public DateTime dateSignUp { get; set; } = DateTime.Today;

        [Required]
        public Role role { get; set; }

        [Required]
        public ICollection<UserToken> tokens { get; set; } = new HashSet<UserToken>();

        public ICollection<UserGroup> groups { get; set; } = new HashSet<UserGroup>();

        [Required]
        public Limitations limitations { get; set; }
    }
}
