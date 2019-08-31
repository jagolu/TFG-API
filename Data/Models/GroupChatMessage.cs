using System;
using System.ComponentModel.DataAnnotations;

namespace API.Data.Models
{
    /// <summary>
    /// A message in a group chat
    /// </summary>
    public class GroupChatMessage
    {
        /// <value>The id of the group where the message belongs to</value>
        public Guid groupid { get; set; }
        
        /// <value>The group where the message belongs to</value>
        public Group Group { get; set; }

        [Required]
        /// <value>The username of the user who sent the message</value>
        public string username { get; set; }

        [Required]
        [MaxLength(256)]
        /// <value>The public id of the user who sent the message</value>
        public string publicUserid { get; set; }

        [Required]
        /// <value>The role of the user who sent the message</value>
        public Role role { get; set; }

        [Required]
        [StringLength(128)]
        /// <value>The text of the message</value>
        public string message { get; set; }

        [Required]
        /// <value>The time when the message was sent</value>
        public DateTime time { get; set; }
    }
}
