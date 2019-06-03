using System.Collections.Generic;

namespace API.Areas.Identity.Models
{
    public class UserSession
    {
        public string api_token { get; set; }

        public string username { get; set; }

        public string role { get; set; }

        public ICollection<UserGroups> groups { get; set; }
    }
}
