using System;

namespace API.Areas.Common.Models
{
    public class Message
    {
        public string title { get; set; }

        public string body { get; set; }

        public string owner { get; set; }

        public DateTime time { get; set; }
    }
}
