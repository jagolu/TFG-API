using API.Data.Models;

namespace API.Areas.Bet.Models
{
    public class NameWinRate
    {
        public NameWinRate(TypeFootballBet type)
        {
            this.name = type.name;
            this.description = type.description;
            this.winRate = type.winRate;
            this.cancelRate = type.winLoseCancel;
        }
        public NameWinRate(TypePay type)
        {
            this.name = type.name;
            this.description = type.description;
            this.winRate = type.winRate;
            this.cancelRate = type.winLoseCancel;
        }
        public string name { get; set; }
        public string description { get; set; }
        public double winRate { get; set; }
        public double cancelRate { get; set; }
    }
}
