﻿using System.Collections.Generic;

namespace API.Areas.GroupManage.Models
{
    public class GroupPage
    {
        public string groupName { get; set; }
        public bool groupType { get; set; }
        public string role { get; set; }
        public List<GroupBet> bets { get; set; }
        public List<GroupMember> members { get; set; }
    }
}
