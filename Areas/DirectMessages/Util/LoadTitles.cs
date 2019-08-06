using API.Areas.DirectMessages.Models;
using API.Data;
using API.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace API.Areas.DirectMessages.Util
{
    public static class LoadTitles
    {
        public static List<DMTitle> load(User u, ApplicationDBContext _context)
        {
            List<DMTitle> retList = new List<DMTitle>();

            _context.Entry(u).Collection("directMessages").Load();
            bool isAdmin = API.Util.AdminPolicy.isAdmin(u, _context);
            List<DirectMessageTitle> all = new List<DirectMessageTitle>();

            all.AddRange(u.directMessages);
            all.AddRange(_context.DirectMessagesTitle.Where(dm => dm.receiver == u).ToList());

            unreadTitles(all, isAdmin).ForEach(dm => retList.Add(new DMTitle(dm, u.id, isAdmin, _context)));
            unclosedTitles(all, isAdmin).ForEach(dm => retList.Add(new DMTitle(dm, u.id, isAdmin, _context)));
            closedTitles(all).ForEach(dm => retList.Add(new DMTitle(dm, u.id, isAdmin, _context)));

            return retList;
        }

        public static List<DirectMessageTitle> unreadTitles(List<DirectMessageTitle> msgs, bool isAdmin)
        {
            if (isAdmin)
            {
                return msgs.Where(dm => dm.unreadMessagesForAdmin > 0 && !dm.closed).OrderByDescending(dm=> dm.lastUpdate).ToList();
            }

            return msgs.Where(dm => dm.unreadMessagesForUser > 0 && !dm.closed).OrderByDescending(dm => dm.lastUpdate).ToList();
        }

        public static List<DirectMessageTitle> unclosedTitles(List<DirectMessageTitle> msgs, bool isAdmin)
        {
            if (isAdmin)
            {
                return msgs.Where(dm => dm.unreadMessagesForAdmin == 0 && !dm.closed).OrderByDescending(dm => dm.lastUpdate).ToList();
            }

            return msgs.Where(dm => dm.unreadMessagesForUser == 0 && !dm.closed).OrderByDescending(dm => dm.lastUpdate).ToList();
        }

        public static List<DirectMessageTitle> closedTitles(List<DirectMessageTitle> msgs)
        {
            return msgs.Where(dm => dm.closed).OrderByDescending(dm => dm.lastUpdate).ToList();
        }
    }
}
