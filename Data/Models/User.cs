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
        [MaxLength(256)]
        public string publicid { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [MaxLength(128)]
        public string email { get; set; }

        [Required]
        [MaxLength(32)]
        public string nickname { get; set; }
        
        [Required]
        [MaxLength(512)]
        public string password { get; set; }

        [Required]
        public Boolean open { get; set; } = true;

        public Byte[] profileImg { get; set; } = null;

        [MaxLength(256)]
        public string tokenValidation { get; set; } = Guid.NewGuid().ToString();

        [MaxLength(256)]
        public string tokenPassword { get; set; } = null;

        public DateTime tokenPassword_expirationTime { get; set; } = new DateTime(1,1,1);

        [Required]
        public DateTime dateSignUp { get; set; } = DateTime.Today;

        [Required]
        public Role role { get; set; }

        public DateTime? lastTimeCreateGroup { get; set; }

        [Required]
        public int maxGroupJoins { get; set; } = 10;

        public DateTime? dateDeleted { get; set; }

        public ICollection<UserToken> tokens { get; set; } = new HashSet<UserToken>();
        public ICollection<UserGroup> groups { get; set; } = new HashSet<UserGroup>();
        public ICollection<GroupInteraction> groupInteractions { get; set; } = new HashSet<GroupInteraction>();
        public ICollection<UserFootballBet> footballBets { get; set; } = new HashSet<UserFootballBet>();
        public ICollection<New> news { get; set; } = new HashSet<New>();
        public ICollection<DirectMessageTitle> directMessages { get; set; } = new HashSet<DirectMessageTitle>();
        public ICollection<Notifications> notifications { get; set; } = new HashSet<Notifications>();
    }
}
