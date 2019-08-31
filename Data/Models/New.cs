using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Models
{
    /// <summary>
    /// A new for users o groups
    /// </summary>
    public class New
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        /// <value>The id of the new</value>
        public Guid id { get; set; }

        /// <value>The id of the group which the new belongs to</value>
        public Guid? groupid { get; set; } = null;
        
        /// <value>The group which the new belongs to</value>
        public Group Group { get; set; } = null;

        /// <value>The id of the user which the new belongs to</value>
        public Guid? userid { get; set; } = null;
        
        /// <value>The user which the new belongs to</value>
        public User User { get; set; } = null;

        [Required]
        /// <value>The time when the new was launched</value>
        public DateTime date { get; set; } = DateTime.Now;

        [Required]
        [MaxLength(64)]
        /// <value>The title of the new</value>
        public string title { get; set; }

        [Required]
        [MaxLength(256)]
        /// <value>The text message of the new</value>
        public string message { get; set; }
    }
}
