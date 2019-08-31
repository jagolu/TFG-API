using System;
using System.ComponentModel.DataAnnotations;

namespace API.Data.Models
{
    /// <summary>
    /// A member of a group
    /// </summary>
    public class UserGroup
    {
        /// <value>The id of the user who is the member of the group</value>
        public Guid userid { get; set; }
        
        /// <value>The user who is the member of the group</value>
        public User User { get; set; }

        /// <value>The id of the group where the user is joined</value>
        public Guid groupid { get; set; }

        /// <value>The group where the user is joined</value>
        public Group Group { get; set; }

        [Required]
        /// <value>The role of the member in the group</value>
        public Role role { get; set; }

        [Required]
        /// <value>True if the user is blocked from the group, false otherwise</value>
        public Boolean blocked { get; set; } = false;

        /// <value>The role of the user who blocked that user</value>
        public Role blockedBy { get; set; }

        [Required]
        /// <value>The coins which the user has in the group</value>
        public int coins { get; set; }

        [Required]
        /// <value>The date when the user joined in the group</value>
        public DateTime dateJoin { get; set; } = DateTime.Today;

        [Required]
        /// <value>The time when that user get his actual role</value>
        public DateTime dateRole { get; set; }
    }
}
