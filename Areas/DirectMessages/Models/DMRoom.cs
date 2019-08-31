using API.Data.Models;
using API.Data;
using System.Collections.Generic;
using System.Linq;

namespace API.Areas.DirectMessages.Models
{
    /// <summary>
    /// The info of a direct message conversation and its messages
    /// </summary>
    public class DMRoom
    {
        //
        // ──────────────────────────────────────────────────────────────────────────
        //   :::::: C O N S T R U C T O R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="title">The dm conversation object</param>
        /// <param name="caller">The caller of the function</param>
        /// <param name="_context">The database context</param>
        public DMRoom(DirectMessageTitle title, User caller, ApplicationDBContext _context)
        {
            this.title = this.addTitle(title, caller, _context);
            this.clusters = addDMClusters(title, _context);
        }


        //
        // ────────────────────────────────────────────────────────────────────────────────────
        //   :::::: P R I V A T E   F U N C T I O N S : :  :   :    :     :        :          :
        // ────────────────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Add the dm conversation info to the private vars
        /// </summary>
        /// <param name="title">The dm conversation</param>
        /// <param name="caller">The caller of the function</param>
        /// <param name="context">The database context</param>
        /// <returns>The info of the dm conversation</returns>
        /// See <see cref="Areas.DirectMessages.Models.DMTitle"/> to know the response structure
        private DMTitle addTitle(DirectMessageTitle title, User caller, ApplicationDBContext context)
        {
            bool isAdmin = API.Util.AdminPolicy.isAdmin(caller, context);
            return new DMTitle(title, caller.id, isAdmin, context);
        }

        /// <summary>
        /// Add the messages clusters to the private vars
        /// </summary>
        /// <param name="title">The dm conversation</param>
        /// <param name="context">The database context</param>
        /// <returns>A list of clusters of messages of the dm conversations</returns>
        private List<DMMessageCluster> addDMClusters(DirectMessageTitle title, ApplicationDBContext context)
        {
            context.Entry(title).Collection("messages").Load();
            List<DirectMessageMessages> msgs = title.messages.OrderBy(tt => tt.time).ToList();
            List<DirectMessageMessages> addMessages = new List<DirectMessageMessages>();
            List<DMMessageCluster> retClusters = new List<DMMessageCluster>();
            bool last = false;

            if (msgs.Count() > 0) last = msgs.First().isAdmin;

            msgs.ForEach(m =>
            {
                if (m.isAdmin != last)
                {
                    retClusters.Add(new DMMessageCluster(addMessages, last));
                    addMessages = new List<DirectMessageMessages>();
                    last = m.isAdmin;
                }

                addMessages.Add(m);
            });

            if(addMessages.Count()>0) retClusters.Add(new DMMessageCluster(addMessages, last));

            return retClusters;
        }

        //
        // ──────────────────────────────────────────────────────────────────────
        //   :::::: C L A S S   V A R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────
        //

        /// <value>The info of the dm conversation</value>
        public DMTitle title { get; set; }
        
        /// <value>The cluster of user messages of the dm conversation</value>
        public List<DMMessageCluster> clusters { get; set; }
    }
}
