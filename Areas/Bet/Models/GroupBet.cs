using System;

namespace API.Areas.Bet.Models
{
    public class GroupBet
    {
        //
        // ──────────────────────────────────────────────────────────────────────────
        //   :::::: C O N S T R U C T O R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bet">The fb</param>
        /// <param name="_context">The database context</param>
        /// <param name="includeResult">True to include the result of the match, false otherwise</param>
        public GroupBet(Data.Models.FootballBet bet, Data.ApplicationDBContext _context, bool includeResult)
        {
            _context.Entry(bet).Reference("MatchDay").Load();
            _context.Entry(bet.MatchDay).Reference("Competition").Load();
            _context.Entry(bet).Reference("type").Load();
            _context.Entry(bet).Reference("typePay").Load();
            _context.Entry(bet.MatchDay).Reference("HomeTeam").Load();
            _context.Entry(bet.MatchDay).Reference("AwayTeam").Load();

            this.bet = bet.id.ToString();
            this.competition = bet.MatchDay.Competition.name;
            this.betName = bet.MatchDay.HomeTeam.name + " vs " + bet.MatchDay.AwayTeam.name;
            this.matchdayDate = bet.MatchDay.date;
            this.typeBet = new NameWinRate(bet.type);
            this.typePay = new NameWinRate(bet.typePay);
            this.minBet = bet.minBet;
            this.maxBet = bet.maxBet;
            this.lastBetTime = bet.dateLastBet;
            if (Util.CheckBetType.isJackpot(bet, _context))
            {
                this.usersJoined = bet.usersJoined;
            }
            if (includeResult)
            {
                this.firstHalfHomeGoals = bet.MatchDay.firstHalfHomeGoals;
                this.firstHalfAwayGoals = bet.MatchDay.firstHalfAwayGoals;
                this.secondHalfHomeGoals = bet.MatchDay.secondHalfHomeGoals;
                this.secondHalfAwayGoals = bet.MatchDay.secondHalfAwayGoals;
                this.fullTimeHomeGoals = bet.MatchDay.fullTimeHomeGoals;
                this.fullTimeAwayGoals = bet.MatchDay.fullTimeAwayGoals;
            }
        }
        

        //
        // ──────────────────────────────────────────────────────────────────────
        //   :::::: C L A S S   V A R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────
        //

        /// <value>The id of the fb</value>
        public string bet { get; set; }


        //
        // ─── MATCH INFO ──────────────────────────────────────────────────
        //

        /// <value>The name of the competition</value>
        public string competition { get; set; }
        
        /// <value>The name of the match</value>
        public string betName { get; set; }
        
        /// <value>The time when the match starts</value>
        public DateTime matchdayDate { get; set; }


        //
        // ─── FOOTBALL BET INFO ───────────────────────────────────────────
        //

        /// <value>The bet type</value>        
        public NameWinRate typeBet { get; set; }
        
        /// <value>The pay type</value>
        public NameWinRate typePay { get; set; }
        
        /// <value>The min of the bet</value>
        public int minBet { get; set; }
        
        /// <value>The max of the bet</value>
        public int maxBet { get; set; }
        
        /// <value>The last time to do a user fb</value>
        public DateTime lastBetTime { get; set; }
        
        /// <value>The count of the users that has joined to this fb</value>
        public int? usersJoined { get; set; }


        //
        // ─── RESULT MATCHDAY ─────────────────────────────────────────────
        //

        /// <value>The home goals on the first part</value>            
        public int? firstHalfHomeGoals { get; set; }
        
        /// <value>The away goals on the first part</value>
        public int? firstHalfAwayGoals { get; set; }
        
        /// <value>The home goals on the second part</value>
        public int? secondHalfHomeGoals { get; set; }
        
        /// <value>The away goals on the second part</value>
        public int? secondHalfAwayGoals { get; set; }
        
        /// <value>The home goals on the full match</value>
        public int? fullTimeHomeGoals { get; set; }
        
        /// <value>The away goals on the full match</value>
        public int? fullTimeAwayGoals { get; set; }
    }
}
