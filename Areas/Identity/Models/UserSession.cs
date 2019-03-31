using API.Util;
using Newtonsoft.Json;
using System;

namespace API.Areas.Identity.Models
{
    public class UserSession
    {
        public string api_token { get; set; }

        public string email { get; set; }

        public string nickname { get; set; }

        public string role { get; set; }

        [JsonConverter(typeof (ConvertBase64ToBlob))]
        public Byte[] image_url { get; set; }
    }
}
