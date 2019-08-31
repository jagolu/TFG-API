using System;

namespace API.Areas.Bet.Models
{
    /// <summary>
    /// A user football bet on the group history section
    /// </summary>
    public class HistoryUserFootballBet
    {
        //
        // ──────────────────────────────────────────────────────────────────────────
        //   :::::: C O N S T R U C T O R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ub">The user fb</param>
        /// <param name="_context">The database context</param>
        /// <param name="ended">true if the fb has ended, false otherwise</param>
        public HistoryUserFootballBet(API.Data.Models.UserFootballBet ub, API.Data.ApplicationDBContext _context, bool ended)
        {
            _context.Entry(ub).Reference("User").Load();
            this.userFootballBet = ub.id.ToString();
            this.username = ub.User.nickname;
            this.valid = ub.valid;
            this.dateInvalid = ub.dateInvalid;
            if (ended)
            {
                this.bet = ub.bet;
                this.homeGoals = ub.homeGoals;
                this.awayGoals = ub.awayGoals;
                this.winner = ub.winner;
                this.dateDone = ub.dateDone;
                this.earnings = ub.earnings;
            }
        }
        

        //
        // ──────────────────────────────────────────────────────────────────────
        //   :::::: C L A S S   V A R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────
        //

        /// <value>The id of the user fb</value>
        public string userFootballBet { get; set; }
        
        /// <value>The username of the owner of the user fb</value>
        public string username { get; set; }
        
        /// <value>The coins bet by the user</value>
        public int? bet { get; set; }
        
        /// <value>The home goals guessed by the user</value>
        public int? homeGoals { get; set; }
        
        /// <value>The away goals guessed by the user</value>
        public int? awayGoals { get; set; }
        
        /// <value>The winner guessed by the user</value>
        public int? winner { get; set; }
        
        /// <value>The time when the user fb was done</value>
        public DateTime dateDone { get; set; }
        
        /// <value>False if the user fb was cancelled, false othwerwise</value>
        public bool valid { get; set; }
        
        /// <value>Date when the user fb was cancelled</value>
        public DateTime dateInvalid { get; set; }
        
        /// <value>The earnings of the user with that user fb</value>
        public int? earnings { get; set; }
    }
}
