using API.Areas.UserInfo.Util;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace API.Areas.UserInfo.Models
{
    /// <summary>
    /// Info to change the info of a user profile
    /// </summary>
    public class ChangeUserInfo
    {
        [MinLength(3)]
        [MaxLength(20)]
        [RegularExpression(@"^[A-Za-z0-9_-]*$")]
        /// <value>The nickname of the user</value>
        public string nickname { get; set; }

        [MinLength(8)]
        [MaxLength(20)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{1,}$")]
        /// <value>The old password of the user</value>
        public string oldpassword { get; set; }

        [MinLength(8)]
        [MaxLength(20)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{1,}$")]
        /// <value>The new password of the user</value>
        public string newPassword { get; set; }
        
        [JsonConverter(typeof (ConvertBase64ToBlob))]
        /// <value>The image of the user</value>
        public Byte[] image { get; set; }
    }
}
