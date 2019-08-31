using API.Data;
using API.Data.Models;

namespace API.Areas.Bet.Util
{
    /// <summary>
    /// Checks the bets of a fb
    /// </summary>
    public static class CheckBetType
    {
        //
        // ──────────────────────────────────────────────────────────────────────
        //   :::::: C L A S S   V A R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────
        //

        /// <value>The name of the relation of fb-type</value>
        private static string __typeBet = "type";
        
        /// <value>The name of the relation of fb-payType</value>
        private static string __typePay = "typePay";
        
        /// <value>Jackpot string</value>
        private static string _jackpot = "JACKPOT";
        
        /// <value>Winner string</value>
        private static string _winner = "WINNER";
        
        /// <value>Closer string</value>
        private static string _closer = "CLOSER";


        /// <value>The name of the type jackpot exact bet</value>
        private static string _jackpotExact = "JACKPOT_EXACT_BET";
        
        /// <value>The name of the type jackpot closer bet</value>
        private static string _jackpotCloser = "JACKPOT_CLOSER_BET";
        
        /// <value>The name of the type solo exact bet</value>
        private static string _soloExact = "SOLO_EXACT_BET";


        //
        // ──────────────────────────────────────────────────────────────────────────────────
        //   :::::: P U B L I C   F U N C T I O N S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Checks if the type is a jackpot type
        /// </summary>
        /// <param name="bet">The football bet</param>
        /// <param name="_context">The database context</param>
        /// <returns>true if the fb is a jackpot fb, false otherwise</returns>
        public static bool isJackpot(FootballBet bet, ApplicationDBContext _context)
        {
            _context.Entry(bet).Reference(__typePay).Load();

            return bet.typePay.name.Contains(_jackpot);
        }

        /// <summary>
        /// Checks if the type is a winner type with the fb object
        /// </summary>
        /// <param name="bet">The football bet</param>
        /// <param name="_context">The database context</param>
        /// <returns>true if the fb is a winner fb, false otherwise</returns>
        public static bool isWinner(FootballBet bet, ApplicationDBContext _context)
        {
            _context.Entry(bet).Reference(__typeBet).Load();

            return bet.type.name.Contains(_winner);
        }
        
        /// <summary>
        /// Checks if the type is a winner type with the object type
        /// </summary>
        /// <param name="bet">The football bet type</param>
        /// <returns>true if the fb is a winner fb, false otherwise</returns>
        public static bool typeIsWinner(TypeFootballBet type)
        {
            return type.name.Contains(_winner);
        }

        /// <summary>
        /// Checks if the type is a closer type
        /// </summary>
        /// <param name="bet">The football bet type</param>
        /// <returns>true if the fb is a closer fb, false otherwise</returns>
        public static bool typeIsCloser(TypeFootballBet type)
        {
            return type.name.Contains(_closer);
        }

        /// <summary>
        /// Checks if the type is a jackpot exact type
        /// </summary>
        /// <param name="bet">The football bet</param>
        /// <param name="_context">The database context</param>
        /// <returns>true if the fb is a jackpot exact fb, false otherwise</returns>
        public static bool isJackpotExact(FootballBet bet, ApplicationDBContext _context)
        {
            _context.Entry(bet).Reference(__typePay).Load();
            return bet.typePay.name.Contains(_jackpotExact);
        }

        /// <summary>
        /// Checks if the type is a jackpot closer type
        /// </summary>
        /// <param name="bet">The football bet</param>
        /// <param name="_context">The database context</param>
        /// <returns>true if the fb is a jackpot closer fb, false otherwise</returns>
        public static bool isJackpotCloser(FootballBet bet, ApplicationDBContext _context)
        {
            _context.Entry(bet).Reference(__typePay).Load();
            return bet.typePay.name.Contains(_jackpotCloser);
        }

        /// <summary>
        /// Checks if the type is a solo type
        /// </summary>
        /// <param name="bet">The football bet</param>
        /// <param name="_context">The database context</param>
        /// <returns>true if the fb is a solo fb, false otherwise</returns>
        public static bool isSoloExact(FootballBet bet, ApplicationDBContext _context)
        {
            _context.Entry(bet).Reference(__typePay).Load();
            return bet.typePay.name.Contains(_soloExact);
        }

        /// <summary>
        /// Get the jackpot exact bet name
        /// </summary>
        /// <returns>The jackpot exact bet name</returns>
        public static string getJackpotExact()
        {
            return _jackpotExact;
        }

        /// <summary>
        /// Get the jackpot closer bet name
        /// </summary>
        /// <returns>The jackpot closer bet name</returns>
        public static string getJackpotCloser()
        {
            return _jackpotCloser;
        }

        /// <summary>
        /// Get the solo exact bet name
        /// </summary>
        /// <returns>The solo exact bet name</returns>
        public static string getSoloExact()
        {
            return _soloExact;
        }

        /// <summary>
        /// Get the coins that the user received when cancels a bet
        /// </summary>
        /// <param name="fb">The fb where the user wants to cancel his bet</param>
        /// <param name="coinsBet">The coins that the user bet</param>
        /// <param name="_context">The database context</param>
        /// <returns>The coins that user will receive when cancel his bet</returns>
        public static int calculateCancelRate(FootballBet fb, int coinsBet, ApplicationDBContext _context)
        {
            _context.Entry(fb).Reference(__typeBet).Load();
            _context.Entry(fb).Reference(__typePay).Load();
            double less1 = fb.type.winLoseCancel;
            double less2 = fb.typePay.winLoseCancel;

            if (less2 == 100) return 0;

            double retCoins = coinsBet * (less1 + less2);

            return (int)System.Math.Round(retCoins, System.MidpointRounding.AwayFromZero);
        }
    }
}
