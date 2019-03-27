﻿using System;

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
