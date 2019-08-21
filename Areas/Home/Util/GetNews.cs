using API.Areas.Home.Models;
using API.Data;
using API.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Areas.Home.Util
{
    public static class GetNews
    {
        public static List<NewMessage> getAuthNews(Guid userid, ApplicationDBContext dbContext)
        {
            List<New> news = dbContext.News.Where(n => n.groupid == null && (n.userid==null ||  n.userid == userid)).OrderByDescending(nn => nn.date).Take(50).ToList();
            List<NewMessage> retMessage = addNews(news, false);

            return retMessage;
        }

        public static List<NewMessage> getStandNews(bool isAdmin, ApplicationDBContext dbContext)
        {
            List<New> news = dbContext.News.Where(n => n.userid == null && n.groupid == null).OrderByDescending(nn => nn.date).Take(50).ToList();
            List<NewMessage> retMessage = addNews(news, isAdmin);

            return retMessage;
        }

        public static List<NewMessage> getGroupNews(Guid groupid, ApplicationDBContext dbContext)
        {
            List<New> news = dbContext.News.Where(n => n.groupid == groupid).OrderByDescending(nn => nn.date).Take(50).ToList();
            List<NewMessage> retMessage = addNews(news, false);

            return retMessage;
        }

        private static List<NewMessage> addNews(List<New> news, bool isAdmin)
        {
            List<NewMessage> retMessage = new List<NewMessage>();
            news.ForEach(n => retMessage.Add(new NewMessage(n, isAdmin)));

            return retMessage;
        }
    }
}
