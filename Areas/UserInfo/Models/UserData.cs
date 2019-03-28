using System;
using System.Collections.Generic;

namespace API.Areas.UserInfo.Models
{
    public class UserData
    {
        public string email { get; set; }
        public string nickname { get; set; }
        public String img { get; set; }
        public string user_role { get; set; }
        public List<RoleGroup> rolesGroup { get; set; }
        public DateTime timeSignUp { get; set; }
    }
}
