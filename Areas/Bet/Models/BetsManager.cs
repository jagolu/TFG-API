using System;

namespace API.Areas.Bet.Models
{
    /// <summary>
    /// A fb that the maker of the group can manage
    /// </summary>
    public class BetsManager
    {
        /// <value>The id of the fb</value>
        public string betId { get; set; }
        
        /// <value>The info of the bet</value>
        public GroupBet bet { get; set; }
        
        /// <value>The date when the fb was launched</value>
        public DateTime dateLaunch { get; set; }
        
        /// <value>The date when the match ends</value>
        public DateTime dateEnd { get; set; }
        
        /// <value>True if the bet has end, false otherwise</value>
        public bool ended { get; set; }
        
        /// <value>Date when the fb was cancelled</value>
        public DateTime dateCancelled { get; set; }
        
        /// <value>True if the fb has been cancelled, false otherwise</value>
        public bool cancelled { get; set; }

        /// <value>True if the fb can be cancelled, false otherwise</value>
        public bool canBeCancelled { get; set; }
    }
}
