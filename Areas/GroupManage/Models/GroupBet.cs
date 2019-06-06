using API.Areas.Bet.Models;
using System;

namespace API.Areas.GroupManage.Models
{
    public class GroupBet
    {
        public string bet { get; set; }
        public string competition { get; set; }
        public string betName { get; set; }
        public NameWinRate typeBet { get; set; }
        public NameWinRate typePay { get; set; }
        public int minBet { get; set; }
        public int maxBet { get; set; }
        public DateTime matchdayDate { get; set; }
        public DateTime lastBetTime { get; set; }
    }
}
