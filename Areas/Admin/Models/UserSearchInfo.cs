using System;
using System.Collections.Generic;

namespace API.Areas.Admin.Models
{
    public class UserSearchInfo
    {
        public string email { get; set; }
        public string username { get; set; }
        public bool open { get; set; }
        public DateTime dateSignUp { get; set; }
        public List<UserInGroup> groups { get; set; }


        public class UserInGroup
        {
            public string groupName { get; set; }
            public string role { get; set; }
            public bool blocked { get; set; }
            public DateTime joinTime { get; set; }
            public DateTime roleTime { get; set; }
        }
    }
}
