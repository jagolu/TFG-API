using System;

namespace API.Areas.Bet.Models
{
    public class GroupBet
    {
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
            this.typeBet = new NameWinRate(bet.type);
            this.typePay = new NameWinRate(bet.typePay);
            this.minBet = bet.minBet;
            this.maxBet = bet.maxBet;
            this.matchdayDate = bet.MatchDay.date;
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
        public string bet { get; set; }
        public string competition { get; set; }
        public string betName { get; set; }
        public NameWinRate typeBet { get; set; }
        public NameWinRate typePay { get; set; }
        public int minBet { get; set; }
        public int maxBet { get; set; }
        public DateTime matchdayDate { get; set; }
        public DateTime lastBetTime { get; set; }
        public int? usersJoined { get; set; }

        //Result matchday
        public int? firstHalfHomeGoals { get; set; }
        public int? firstHalfAwayGoals { get; set; }
        public int? secondHalfHomeGoals { get; set; }
        public int? secondHalfAwayGoals { get; set; }
        public int? fullTimeHomeGoals { get; set; }
        public int? fullTimeAwayGoals { get; set; }
    }
}
