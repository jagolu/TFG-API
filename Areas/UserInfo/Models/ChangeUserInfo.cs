using API.Areas.UserInfo.Util;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace API.Areas.UserInfo.Models
{
    public class ChangeUserInfo
    {
        [MinLength(3)]
        [MaxLength(20)]
        [RegularExpression(@"^[A-Za-z0-9_-]*$")]
        public string nickname { get; set; }

        [MinLength(8)]
        [MaxLength(20)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{1,}$")]
        public string oldpassword { get; set; }

        [MinLength(8)]
        [MaxLength(20)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{1,}$")]
        public string newPassword { get; set; }
        
        [JsonConverter(typeof (ConvertBase64ToBlob))]
        public Byte[] image { get; set; }
    }
}
