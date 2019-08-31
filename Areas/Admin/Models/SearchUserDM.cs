namespace API.Areas.Admin.Models
{
    /// <summary>
    /// The simple info of a user
    /// </summary>
    public class SearchUserDM
    {
        //
        // ──────────────────────────────────────────────────────────────────────────
        //   :::::: C O N S T R U C T O R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────
        //
        
        /// <summary>
        /// Constructor to fill the class vars
        /// </summary>
        /// <param name="u">The user which we want to get the info</param>
        public SearchUserDM(Data.Models.User u)
        {
            this.email = u.email;
            this.username = u.nickname;
        }


        //
        // ──────────────────────────────────────────────────────────────────────
        //   :::::: C L A S S   V A R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────
        //

        /// <value>The email of the user</value>
        public string email { get; set; }

        /// <value>The username of the user</value>
        public string username { get; set; }
    }
}
