using API.Data;
using API.Data.Models;

namespace API.Areas.Bet.Util
{
    public static class CheckBetType
    {
        private static string __typeBet = "type";
        private static string __typePay = "typePay";
        private static string _jackpot = "JACKPOT";
        private static string _winner = "WINNER";
        private static string _closer = "CLOSER";

        private static string _jackpotExact = "JACKPOT_EXACT_BET";
        private static string _jackpotCloser = "JACKPOT_CLOSER_BET";
        private static string _soloExact = "SOLO_EXACT_BET";

        public static bool isJackpot(FootballBet bet, ApplicationDBContext _context)
        {
            _context.Entry(bet).Reference(__typePay).Load();

            return bet.typePay.name.Contains(_jackpot);
        }

        public static bool isWinner(FootballBet bet, ApplicationDBContext _context)
        {
            _context.Entry(bet).Reference(__typeBet).Load();

            return bet.type.name.Contains(_winner);
        }

        public static bool typeIsWinner(TypeFootballBet type)
        {
            return type.name.Contains(_winner);
        }

        public static bool typeIsCloser(TypeFootballBet type)
        {
            return type.name.Contains(_closer);
        }

        public static bool isJackpotExact(FootballBet bet, ApplicationDBContext _context)
        {
            _context.Entry(bet).Reference(__typePay).Load();
            return bet.typePay.name.Contains(_jackpotExact);
        }

        public static bool isJackpotCloser(FootballBet bet, ApplicationDBContext _context)
        {
            _context.Entry(bet).Reference(__typePay).Load();
            return bet.typePay.name.Contains(_jackpotCloser);
        }

        public static bool isSoloExact(FootballBet bet, ApplicationDBContext _context)
        {
            _context.Entry(bet).Reference(__typePay).Load();
            return bet.typePay.name.Contains(_soloExact);
        }



        public static string getJackpotExact()
        {
            return _jackpotExact;
        }

        public static string getJackpotCloser()
        {
            return _jackpotCloser;
        }

        public static string getSoloExact()
        {
            return _soloExact;
        }

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
