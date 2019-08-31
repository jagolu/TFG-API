using API.Data.Models;

namespace API.Areas.Bet.Models
{
    /// <summary>
    /// The of a bet/pay type
    /// </summary>
    public class NameWinRate
    {
        //
        // ──────────────────────────────────────────────────────────────────────────
        //   :::::: C O N S T R U C T O R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Intializes the object with bet type
        /// </summary>
        /// <param name="type">The bet type</param>
        public NameWinRate(TypeFootballBet type)
        {
            this.id = type.id.ToString();
            this.name = type.name;
            this.description = type.description;
            this.winRate = type.winRate;
            this.cancelRate = type.winLoseCancel;
        }
        
        /// <summary>
        /// Initializes the object with a pay type
        /// </summary>
        /// <param name="type">The pay type</param>
        public NameWinRate(TypePay type)
        {
            this.id = type.id.ToString();
            this.name = type.name;
            this.description = type.description;
            this.winRate = type.winRate;
            this.cancelRate = type.winLoseCancel;
        }


        //
        // ──────────────────────────────────────────────────────────────────────────────────
        //   :::::: P U B L I C   F U N C T I O N S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────────────
        //

        /// <value>The id of the type</value>
        public string id { get; set; }
        
        /// <value>The name of the type</value>
        public string name { get; set; }
        
        /// <value>The description of the type</value>
        public string description { get; set; }
        
        /// <value>The win rate of the type</value>
        public double winRate { get; set; }
        
        /// <value>The cancel rate of the type</value>
        public double cancelRate { get; set; }
    }
}
