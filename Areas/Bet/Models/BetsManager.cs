using System;

namespace API.Areas.Bet.Models
{
    public class BetsManager
    {
        public GroupBet bet { get; set; }
        public DateTime dateLaunch { get; set; }
        public DateTime dateEnd { get; set; }
        public bool ended { get; set; }
        public DateTime dateCancelled { get; set; }
        public bool cancelled { get; set; }
    }
}
