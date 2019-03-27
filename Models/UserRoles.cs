using System;

namespace API.Models
{
    public class UserRoles
    {
        public Guid userId { get; set; }
        public User User { get; set; }
        public Guid roleId { get; set; }
        public Role Role { get; set; }
    }
}
