using API.Data.Models;
using API.Data;
using System.Collections.Generic;
using System.Linq;

namespace API.Areas.DirectMessages.Models
{
    public class DMRoom
    {
        public DMRoom(DirectMessageTitle title, User caller, ApplicationDBContext _context)
        {
            this.title = this.addTitle(title, caller, _context);
            this.clusters = addDMClusters(title, _context);
        }

        private DMTitle addTitle(DirectMessageTitle title, User caller, ApplicationDBContext context)
        {
            bool isAdmin = API.Util.AdminPolicy.isAdmin(caller, context);
            return new DMTitle(title, caller.id, isAdmin, context);
        }

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

        public DMTitle title { get; set; }
        public List<DMMessageCluster> clusters { get; set; }
    }
}
