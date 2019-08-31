using API.Areas.UserInfo.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace API.Areas.UserInfo.Models
{
    /// <summary>
    /// The data of a user profile
    /// </summary>
    public class UserData
    {
        /// <value>The email of the user</value>
        public string email { get; set; }
        
        /// <value>The username of the user</value>
        public string nickname { get; set; }

        [JsonConverter(typeof(ConvertBase64ToBlob))]
        /// <value>The image of the user</value>
        public Byte[] img { get; set; }

        /// <value>A list of the groups of the user</value>
        public List<string> groups { get; set; }

        /// <value>The time when the user registered in the platform</value>
        public DateTime timeSignUp { get; set; }
    }
}
