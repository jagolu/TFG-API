using System;

namespace API.Areas.Identity.Models
{
    /// <summary>
    /// The response of the validation Facebook token request
    /// </summary>
    public class FacebookResponse
    {
        /// <value>The data of the body response</value>
        public dataType data { get; set; }

        /// <summary>
        /// The data with all the info
        /// </summary>
        public class dataType
        {
            /// <value>The id of the app</value>
            public string app_id { get; set; }
            
            /// <value>The type of the call</value>
            public string type { get; set; }
            
            /// <value>The name of the app</value>
            public string application { get; set; }
            
            /// <value>The time when the token expires</value>
            public string data_access_expires_at { get; set; }
            
            /// <value>True if the token is valid, false otherwise</value>
            public Boolean is_valid { get; set; }
            
            /// <value>The id of the user</value>
            public string user_id { get; set; }
            
            /// <value>Erros (if they exist)</value>
            public errorType error { get; set; }

            /// <value>Error class</value>
            public class errorType
            {
                /// <value>True if the token is valid, false otherwise</value>
                public Boolean is_valid { get; set; }
            }
        }
    }
}
