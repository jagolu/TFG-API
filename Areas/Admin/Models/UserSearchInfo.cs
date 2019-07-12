using System;

namespace API.Areas.Admin.Models
{
    public class UserSearchInfo
    {
        public string email { get; set; }
        public string username { get; set; }
        public bool open { get; set; }
        public DateTime dateSignUp { get; set; }
    }
}
