using System;

namespace API.Areas.Identity.Models
{
    public class FacebookResponse
    {
        public dataType data { get; set; }

        public class dataType
        {
            public string app_id { get; set; }
            public string type { get; set; }
            public string application { get; set; }
            public string data_access_expires_at { get; set; }
            public Boolean is_valid { get; set; }
            public string user_id { get; set; }
            public errorType error { get; set; }

            public class errorType
            {
                public Boolean is_valid { get; set; }
            }
        }
    }
}
