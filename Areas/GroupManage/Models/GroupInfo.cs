using System;
using System.Collections.Generic;

namespace API.Areas.GroupManage.Models
{
    /// <summary>
    /// A basic info of a group
    /// </summary>
    public class GroupInfo
    {
        /// <value>The name of the group</value>
        public string name { get; set; }
        
        /// <value>True if the group has password, false otherwise</value>
        public bool password { get; set; }
        
        /// <value>The places ocupped in the group</value>
        public int placesOcupped { get; set; }
        
        /// <value>The total places of the group</value>
        public int totalPlaces { get; set; }
        
        /// <value>The time when the group was created</value>
        public DateTime dateCreate { get; set; }
        
        /// <value>False if the group is banned, false otherwise</value>
        public bool open { get; set; }
        
        /// <value>A list of members of the group</value>
        public List<GroupMemberAdmin> members { get; set; }

        /// <summary>
        /// A basic info of the member of the group
        /// </summary>
        public class GroupMemberAdmin
        {
            /// <value>The username of the user</value>
            public string username { get; set; }
            
            /// <value>The email of the user</value>
            public string email { get; set; }
            
            /// <value>The role of the member in the group</value>
            public string role { get; set; }
            
            /// <value>The time when the user joined to the group</value>
            public DateTime dateJoin { get; set; }
            
            /// <value>The time when the user got the role</value>
            public DateTime dateRole { get; set; }
            
            /// <value>True if the user is blocked, false otherwise</value>
            public bool blocked { get; set; }
            
            /// <value>The coins that user has in the group</value>
            public int? coins { get; set; }
        }
    }
}
