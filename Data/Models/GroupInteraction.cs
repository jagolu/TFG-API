using System;
using System.ComponentModel.DataAnnotations;

namespace API.Data.Models
{
    /// <summary>
    /// A interaction of the user with a group
    /// </summary>
    public class GroupInteraction
    {
        /// <value>The id of the user who interacted with the group</value>
        public Guid userid { get; set; }
        
        /// <value>The user who interacted with the group</value>
        public User User { get; set; }


        /// <value>The id of the group with user interacted</value>
        public Guid groupid { get; set; }
        
        /// <value>The group with user interacted</value>
        public Group Group { get; set; }


        /// <value>The time when the user leaved the group</value>
        public DateTime dateLeave { get; set; } = DateTime.Now;

        [Required]
        /// <value>True if the user was kicked, false otherwise</value>
        public bool kicked { get; set; } = false;

        [Required]
        /// <value>True if the user leaved the group, false otherwise</value>
        public bool leaved { get; set; } = false;
    }
}
