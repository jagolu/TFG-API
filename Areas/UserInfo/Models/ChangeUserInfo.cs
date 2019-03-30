using API.Util;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace API.Areas.UserInfo.Models
{
    public class ChangeUserInfo
    {
        [Required]
        [MinLength(4, ErrorMessage = "Username must have at least 3 characters")]
        [MaxLength(20, ErrorMessage = "Username must have lesss than 20 characters")]
        public string nickname { get; set; }

        [MinLength(8, ErrorMessage = "Password must have at least 8 characters")]
        [MaxLength(20, ErrorMessage = "Password must have less than 20 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{1,}$",
            ErrorMessage = "Password must have at least a lowercase, a uppercase and a number")]
        public string password { get; set; }

        [Required]
        [MaxLength]
        [JsonConverter(typeof (ConvertBase64ToBlob))]
        public Byte[] image { get; set; }
    }
}
