using API.Areas.DirectMessages.Models;
using API.Data;
using API.Data.Models;
using API.Util;
using System.Collections.Generic;
using System.Linq;

namespace API.Areas.DirectMessages.Util
{
    public static class LoadTitles
    {
        public static List<DMTitles> load(User u, ApplicationDBContext _context)
        {
            List<DMTitles> retList = new List<DMTitles>();
            bool isAdmin = AdminPolicy.isAdmin(u, _context);

            _context.Entry(u).Collection("directMessages").Load();
            u.directMessages.OrderByDescending(dmm => dmm.openDate).ToList().ForEach(dm =>
            {
                retList.Add(new DMTitles(dm, isAdmin, _context));
            });

            return retList;
        }
    }
}
