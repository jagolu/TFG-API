using API.Areas.Home.Models;
using API.Data;
using API.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Areas.Home.Util
{
    /// <summary>
    /// Static class for get the news
    /// </summary>
    public static class GetNews
    {
        //
        // ──────────────────────────────────────────────────────────────────────────────────
        //   :::::: P U B L I C   F U N C T I O N S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────────────
        //

        
        /// <summary>
        /// Get the news for logged users
        /// </summary>
        /// <param name="userid">The id of the logged users</param>
        /// <param name="dbContext">The database context</param>
        /// <returns>The list of the logged news</returns>
        /// See <see cref="Areas.Home.Models.NewMessage"/> to see the response structure
        public static List<NewMessage> getAuthNews(Guid userid, ApplicationDBContext dbContext)
        {
            List<New> news = dbContext.News.Where(n => n.groupid == null && (n.userid==null ||  n.userid == userid)).OrderByDescending(nn => nn.date).Take(50).ToList();
            List<NewMessage> retMessage = addNews(news, false);

            return retMessage;
        }

        /// <summary>
        /// Get the news for not logged users or admin users
        /// </summary>
        /// <param name="isAdmin">True if the caller is an admin, false otherwise</param>
        /// <param name="dbContext">The context of the database</param>
        /// <returns>The list of the not logged news</returns>
        /// See <see cref="Areas.Home.Models.NewMessage"/> to see the response structure
        public static List<NewMessage> getStandNews(bool isAdmin, ApplicationDBContext dbContext)
        {
            List<New> news = dbContext.News.Where(n => n.userid == null && n.groupid == null).OrderByDescending(nn => nn.date).Take(50).ToList();
            List<NewMessage> retMessage = addNews(news, isAdmin);

            return retMessage;
        }

        /// <summary>
        /// Get the news for a group
        /// </summary>
        /// <param name="groupid">The id of the group</param>
        /// <param name="dbContext">The context of the database</param>
        /// <returns>The list of the groupnews</returns>
        /// See <see cref="Areas.Home.Models.NewMessage"/> to see the response structure
        public static List<NewMessage> getGroupNews(Guid groupid, ApplicationDBContext dbContext)
        {
            List<New> news = dbContext.News.Where(n => n.groupid == groupid).OrderByDescending(nn => nn.date).Take(50).ToList();
            List<NewMessage> retMessage = addNews(news, false);

            return retMessage;
        }


        //
        // ────────────────────────────────────────────────────────────────────────────────────
        //   :::::: P R I V A T E   F U N C T I O N S : :  :   :    :     :        :          :
        // ────────────────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Add a list of <see cref="Data.Models.New"/> to the response list
        /// </summary>
        /// <param name="news">The news to add</param>
        /// <param name="isAdmin">True if the caller is an admin, false otherwise</param>
        /// <returns>The <see cref="Areas.Home.Models.NewMessage"/> list with all the param news</returns>
        private static List<NewMessage> addNews(List<New> news, bool isAdmin)
        {
            List<NewMessage> retMessage = new List<NewMessage>();
            news.ForEach(n => retMessage.Add(new NewMessage(n, isAdmin)));

            return retMessage;
        }
    }
}
