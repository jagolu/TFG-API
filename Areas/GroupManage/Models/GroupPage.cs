using System;
using System.Collections.Generic;

namespace API.Areas.GroupManage.Models
{
    public class GroupPage
    {
        public string name { get; set; }
        public bool type { get; set; }
        public string role { get; set; }
        public DateTime dateJoin { get; set; }
        public DateTime dateRole { get; set; }
        public bool canPutPassword { get; set; }
        public bool hasPassword { get; set; }
        public int maxCapacity { get; set; }
        public int actualCapacity { get; set; }
        public DateTime createDate { get; set; }
        public List<GroupBet> bets { get; set; }
        public List<GroupMember> members { get; set; }
    }
}
