using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Models
{
    /// <summary>
    /// A user in the platform
    /// </summary>
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        /// <value>The id of the user</value>
        public Guid id { get; set; }

        [Required]
        [MaxLength(256)]
        /// <value>The public id of the user</value>
        public string publicid { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [MaxLength(128)]
        /// <value>The email of the user</value>
        public string email { get; set; }

        [Required]
        [MaxLength(128)]
        /// <value>The username of the user</value>
        public string nickname { get; set; }
        
        [Required]
        [MaxLength(512)]
        /// <value>The password of the user</value>
        public string password { get; set; }

        [Required]
        /// <value>False if the user is banned, false otherwise</value>
        public Boolean open { get; set; } = true;

        /// <value>The profile image of the user</value>
        public Byte[] profileImg { get; set; } = null;

        [MaxLength(256)]
        /// <value>The token to validate the email</value>
        public string tokenValidation { get; set; } = Guid.NewGuid().ToString();

        [MaxLength(256)]
        /// <value>The token to recuperate the password</value>
        public string tokenPassword { get; set; } = null;

        /// <value>The time when the token to recuperate the password expires</value>
        public DateTime tokenP_expiresTime { get; set; } = new DateTime(1,1,1);

        [Required]
        /// <value>The date when the users registered</value>
        public DateTime dateSignUp { get; set; } = DateTime.Today;

        [Required]
        /// <value>The role of the user</value>
        public Role role { get; set; }

        /// <value>The date of the last time when the user created a group</value>
        public DateTime? lastTimeCreateGroup { get; set; }

        [Required]
        /// <value>The max of groups which the user can join</value>
        public int maxGroupJoins { get; set; } = 10;

        /// <value>The time when the user ask for delete his account</value>
        public DateTime? dateDeleted { get; set; }


        /// <value>The session tokens of the user</value>
        public ICollection<UserToken> tokens { get; set; } = new HashSet<UserToken>();
        
        /// <value>The groups of the user</value>
        public ICollection<UserGroup> groups { get; set; } = new HashSet<UserGroup>();
        
        /// <value>The group interactions of the user</value>
        public ICollection<GroupInteraction> groupInteractions { get; set; } = new HashSet<GroupInteraction>();
        
        /// <value>The bets done by the user</value>
        public ICollection<UserFootballBet> footballBets { get; set; } = new HashSet<UserFootballBet>();
        
        /// <value>The news of the user</value>
        public ICollection<New> news { get; set; } = new HashSet<New>();
        
        /// <value>The direct messages of the user</value>
        public ICollection<DirectMessageTitle> directMessages { get; set; } = new HashSet<DirectMessageTitle>();
        
        /// <value>The notifications of the user</value>
        public ICollection<Notifications> notifications { get; set; } = new HashSet<Notifications>();
    }
}
