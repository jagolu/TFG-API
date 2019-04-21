using API.Areas.UserInfo.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace API.Areas.UserInfo.Models
{
    public class UserData
    {
        public string email { get; set; }

        public string nickname { get; set; }

        [JsonConverter(typeof(ConvertBase64ToBlob))]
        public Byte[] img { get; set; }

        public String user_role { get; set; }

        public List<RoleGroup> rolesGroup { get; set; }

        public DateTime timeSignUp { get; set; }

        public bool password { get; set; }
    }
}
