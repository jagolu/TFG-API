using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class UserPermission
    {
        public Guid userId { get; set; }
        public User User { get; set; }
        public Guid permissionId { get; set; }
        public Permission Permission { get; set; }
    }
}
