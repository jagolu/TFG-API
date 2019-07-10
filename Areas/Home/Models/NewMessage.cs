using System;

namespace API.Areas.Home.Models
{
    public class NewMessage
    {

        public string title { get; set; }

        public string body { get; set; }
        
        public DateTime time { get; set; }
    }
}
