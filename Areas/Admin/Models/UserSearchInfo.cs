using System;
using System.Collections.Generic;

namespace API.Areas.Admin.Models
{
    /// <summary>
    /// The info of a user and the groups which belongs to
    /// </summary>
    public class UserSearchInfo
    {
        /// <value>The public id of the user</value>
        public string publicUserId { get; set; }

        /// <value>The email of the user</value>
        public string email { get; set; }

        /// <value>The username of the user</value>
        public string username { get; set; }

        /// <value>True if the user is banned, false otherwise</value>
        public bool open { get; set; }

        /// <value>The time when the user did the register</value>
        public DateTime dateSignUp { get; set; }

        /// <value>The groups which the user belongs to, and its info</value>
        public List<UserInGroup> groups { get; set; }

        /// <summary>
        /// The info of the user in a group
        /// </summary>
        public class UserInGroup
        {
            /// <value>The name of the group</value>
            public string groupName { get; set; }

            /// <value>The role of the user in the group</value>
            public string role { get; set; }

            /// <value>True if the user is blocked in the group, false otherwise</value>
            public bool blocked { get; set; }

            /// <value>The time when the user joined in the group</value>
            public DateTime joinTime { get; set; }

            /// <value>The time when the user got the actual role in the group</value>
            public DateTime roleTime { get; set; }
        }
    }
}
