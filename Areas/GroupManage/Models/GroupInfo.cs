using System;
using System.Collections.Generic;

namespace API.Areas.GroupManage.Models
{
    public class GroupInfo
    {
        public string name { get; set; }
        public bool password { get; set; }
        public int placesOcupped { get; set; }
        public int totalPlaces { get; set; }
        public DateTime dateCreate { get; set; }
        public bool open { get; set; }
        public List<GroupMemberAdmin> members { get; set; }

        public class GroupMemberAdmin
        {
            public string username { get; set; }
            public string email { get; set; }
            public string role { get; set; }
            public DateTime dateJoin { get; set; }
            public DateTime dateRole { get; set; }
            public bool blocked { get; set; }
            public int? coins { get; set; }
        }
    }
}
