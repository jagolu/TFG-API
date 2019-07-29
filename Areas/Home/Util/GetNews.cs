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
            List<New> news = dbContext.News.Where(n => n.groupId == null && (n.userId==null ||  n.userId == userid)).OrderByDescending(nn => nn.date).ToList();
            List<NewMessage> retMessage = addNews(news);

            return retMessage;
        }

        public static List<NewMessage> getStandNews(ApplicationDBContext dbContext)
        {
            List<New> news = dbContext.News.Where(n => n.userId == null && n.groupId == null).OrderByDescending(nn => nn.date).ToList();
            List<NewMessage> retMessage = addNews(news);

            return retMessage;
        }


        private static List<NewMessage> addNews(List<New> news)
        {
            List<NewMessage> retMessage = new List<NewMessage>();
            news.ForEach(n => retMessage.Add(new NewMessage(n)));

            return retMessage;
        }
    }
}
