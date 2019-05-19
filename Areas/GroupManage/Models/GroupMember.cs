using System;

namespace API.Areas.GroupManage.Models
{
    public class GroupMember
    {
        public string userName { get; set; }
        public string publicUserId { get; set; }
        public string role { get; set; }
        public DateTime dateJoin { get; set; }
        public DateTime dateRole { get; set; }
    }
}
