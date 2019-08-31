using System;

namespace API.Areas.Home.Models
{
    /// <summary>
    /// The data necessary for fill a new in the frontend
    /// </summary>
    public class NewMessage
    {
        //
        // ──────────────────────────────────────────────────────────────────────────
        //   :::::: C O N S T R U C T O R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────
        //
        
        /// <summary>
        /// Create the new object
        /// </summary>
        /// <param name="notice">The new</param>
        /// <param name="isAdmin">True if the caller is an admin, false otherwise</param>
        public NewMessage(Data.Models.New notice, bool isAdmin)
        {
            this.id = isAdmin ? notice.id.ToString() : null;
            this.title = notice.title;
            this.body = notice.message;
            this.time = notice.date;
        }


        //
        // ──────────────────────────────────────────────────────────────────────
        //   :::::: C L A S S   V A R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────
        //        
        
        /// <value>The id of the new</value>
        public String id { get; set; }

        /// <value>The title of the new</value>
        public string title { get; set; }

        /// <value>The body of the new</value>
        public string body { get; set; }
        
        /// <value>The time when the new was launched</value>
        public DateTime time { get; set; }
    }
}
