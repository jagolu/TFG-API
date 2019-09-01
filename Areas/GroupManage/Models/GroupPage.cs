using API.Areas.Bet.Models;
using System;
using System.Collections.Generic;

namespace API.Areas.GroupManage.Models
{
    /// <summary>
    /// The full info of a group page
    /// </summary>
    public class GroupPage
    {
        /// <value>The name of the group</value>
        public string name { get; set; }
        
        /// <value>The time when the caller joined in the group</value>
        public DateTime dateJoin { get; set; }
        
        /// <value>The time when the called got his actual role</value>
        public DateTime dateRole { get; set; }
        
        /// <value>True if the group has password, false otherwise</value>
        public bool hasPassword { get; set; }
        
        /// <value>The max capacity of the group</value>
        public int maxCapacity { get; set; }
        
        /// <value>The actual capacity of the group</value>
        public int actualCapacity { get; set; }
        
        /// <value>The time when the group was created</value>
        public DateTime createDate { get; set; }
        
        /// <value>The weekly pay on the group</value>
        public int weeklyPay { get; set; }
        
        /// <value>The new fb in that week</value>
        public List<GroupBet> bets { get; set; }
        
        /// <value>All the fb launched in the group (only for the maker of the group)</value>
        public List<BetsManager> manageBets { get; set; }
        
        /// <value>The fb that the caller has bet on this week</value>
        public List<EndedFootballBet> myBets { get; set; }
        
        /// <value>All the ended fb that the user has bet on</value>
        public List<EndedFootballBet> betsHistory { get; set; }
        
        /// <value>The members of the group</value>
        public List<GroupMember> members { get; set; }
        
        /// <value>The news of the group</value>
        public List<Home.Models.NewMessage> news { get; set; }
    }
}
