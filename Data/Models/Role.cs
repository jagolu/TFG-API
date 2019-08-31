using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Models
{
    /// <summary>
    /// A role of a user
    /// </summary>
    public class Role
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        /// <value>The id of the role</value>
        public Guid id { get; set; }
        
        [Required]
        [MaxLength(64)]
        /// <value>The name of the role</value>
        public string name { get; set; }
    }
}
