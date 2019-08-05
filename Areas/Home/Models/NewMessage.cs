using System;

namespace API.Areas.Home.Models
{
    public class NewMessage
    {
        public NewMessage(Data.Models.New notice, bool isAdmin)
        {
            this.id = isAdmin ? notice.id.ToString() : null;
            this.title = notice.title;
            this.body = notice.message;
            this.time = notice.date;
        }

        public String id { get; set; }

        public string title { get; set; }

        public string body { get; set; }
        
        public DateTime time { get; set; }
    }
}
