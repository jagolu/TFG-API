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

            u.directMessages.ToList().ForEach(dm => retList.Add(new DMTitle(dm, u.id, isAdmin, _context)));
            _context.DirectMessagesTitle.Where(dm => dm.Receiver == u).ToList().ForEach(mm => retList.Add(new DMTitle(mm, u.id, isAdmin, _context)));

            return retList.OrderByDescending(dm => dm.openDate).ToList();
        }
    }
}
