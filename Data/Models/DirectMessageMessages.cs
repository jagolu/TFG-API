using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Models
{
    /// <summary>
    /// The messages in a direct messages conversation
    /// </summary>
    public class DirectMessageMessages
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        /// <value>The id of the dm</value>
        public Guid id { get; set; }

        [Required]
        /// <value>True if the sender is an admin, false otherwise</value>
        public bool isAdmin { get; set; }

        [Required]
        [MaxLength(512)]
        /// <value>The text message</value>
        public string message { get; set; }

        [Required]
        /// <value>The time when the message was sended</value>
        public DateTime time { get; set; } = DateTime.Now;

        [Required]
        /// <value>The direct message where the message was sent</value>
        public DirectMessageTitle DirectMessageTitle { get; set; }
        
        /// <value>The id of the direct message where the message was sent</value>
        public Guid directMessageTitleid { get; set; }
    }
}
