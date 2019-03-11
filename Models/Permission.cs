using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class Permission
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }

        [Required]
        public virtual Role role { get; set; }

        [Required]
        [StringLength(maximumLength: 50, MinimumLength = 4)]
        public string name { get; set; }

        public virtual ICollection<UserPermission> users { get; set; } = new HashSet<UserPermission>();
    }
}
