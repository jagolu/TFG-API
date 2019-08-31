using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Models
{
    /// <summary>
    /// A user notification
    /// </summary>
    public class Notifications
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        /// <value>The id of the notification</value>
        public Guid id { get; set; }

        [Required]
        /// <value>The id of the user which the notifications belongs to</value>
        public Guid userid { get; set; }
        
        /// <value>The user which the notifications belongs to</value>
        public User User { get; set; }

        [Required]
        [MaxLength(64)]
        /// <value>The text of the notification</value>
        public string message { get; set; }

        [Required]
        /// <value>The time when the notification was sent</value>
        public DateTime time { get; set; } = DateTime.Now;
    }
}
