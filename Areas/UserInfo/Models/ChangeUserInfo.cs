using API.Areas.UserInfo.Util;
using Newtonsoft.Json;
using System;

namespace API.Areas.UserInfo.Models
{
    public class ChangeUserInfo
    {
        public string nickname { get; set; }

        public string oldpassword { get; set; }

        public string newPassword { get; set; }
        
        [JsonConverter(typeof (ConvertBase64ToBlob))]
        public Byte[] image { get; set; }
    }
}
