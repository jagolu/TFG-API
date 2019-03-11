using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class User
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }

        [Required]
        [MaxLength]
        [MinLength(4)]
        public string email { get; set; }

        [Required]
        [StringLength(maximumLength:20, MinimumLength =4)]
        public string nickname { get; set; }

        [Required]
        [MaxLength]
        public string password { get; set; }

        [Required]
        public Boolean open { get; set; } = true;

        [System.ComponentModel.DefaultValue("imagenPorDefecto")]
        public string profileImg { get; set; } = null;

        [MaxLength]
        public string tokenValidation { get; set; } = Guid.NewGuid().ToString();

        [Required]

        public virtual ICollection<UserPermission> permissions { get; set; } = new HashSet<UserPermission>();
    }
}
