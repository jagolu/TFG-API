using API.Areas.DirectMessages.Models;
using System.Collections.Generic;
using System.Linq;

namespace API.Areas.DirectMessages.Util
{
    public static class LoadMessages
    {
        public static List<DMMessages> load(Data.Models.DirectMessageTitle title, Data.ApplicationDBContext _context)
        {
            _context.Entry(title).Collection("messages").Load();
            List<DMMessages> retMsgs = new List<DMMessages>();

            title.messages.OrderByDescending(tt => tt.time).ToList().ForEach(m => retMsgs.Add(new DMMessages(m)));

            return retMsgs;
        }
    }
}
