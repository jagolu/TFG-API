using API.Areas.DirectMessages.Models;
using API.Data;
using API.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace API.Areas.DirectMessages.Util
{
    public static class LoadTitles
    {
        public static List<DMTitles> load(User u, ApplicationDBContext _context)
        {
            List<DMTitles> retList = new List<DMTitles>();

            _context.Entry(u).Collection("directMessages").Load();
            u.directMessages.ToList().ForEach(dm => retList.Add(new DMTitles(dm, u, _context)));
            _context.DirectMessagesTitle.Where(dm => dm.Receiver == u).ToList().ForEach(mm => retList.Add(new DMTitles(mm, u, _context)));

            return retList.OrderByDescending(dm => dm.openDate).ToList();
        }
    }
}
