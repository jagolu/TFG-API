using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class Role
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }
        
        [Required]
        [StringLength(maximumLength:50, MinimumLength =4)]
        public string name { get; set; }

        public virtual ICollection<UserRoles> users { get; set; } = new HashSet<UserRoles>();
    }
}
