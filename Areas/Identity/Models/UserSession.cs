using System.Collections.Generic;

namespace API.Areas.Identity.Models
{
    /// <summary>
    /// The info of the session of the user
    /// </summary>
    public class UserSession
    {
        /// <value>The session token</value>
        public string api_token { get; set; }
        
        /// <value>The username of the user</value>
        public string username { get; set; }
        
        /// <value>The role of the user</value>
        public string role { get; set; }
        
        /// <value>The name of the groups of the user</value>
        public ICollection<string> groups { get; set; }
    }
}
