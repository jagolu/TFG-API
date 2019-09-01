using API.Areas.UserInfo.Util;
using Newtonsoft.Json;
using System;

namespace API.Areas.GroupManage.Models
{
    /// <summary>
    /// A full info of a member in a group
    /// </summary>
    public class GroupMember
    {
        /// <value>The username of the member</value>
        public string userName { get; set; }
        
        /// <value>The public id of the member</value>
        public string publicUserId { get; set; }
        
        /// <value>The role of the member in the group</value>
        public string role { get; set; }
        
        /// <value>The time when the user joined in the group</value>
        public DateTime dateJoin { get; set; }
        
        /// <value>The time when the user got his actual role</value>
        public DateTime dateRole { get; set; }
        
        /// <value>False if the user is banned from the group, false otherwise</value>
        public bool blocked { get; set; }
        
        /// <value>The role of the user who banned that user</value>
        public String blockedBy { get; set; }

        [JsonConverter(typeof(ConvertBase64ToBlob))]
        /// <value>The profile image of the user</value>
        public Byte[] img { get; set; }

        /// <value>The coins that the user has in the group</value>
        public int coins { get; set; }
    }
}
