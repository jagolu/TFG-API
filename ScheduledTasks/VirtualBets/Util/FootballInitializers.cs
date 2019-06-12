using API.Data;
using API.Data.Models;
using API.ScheduledTasks.VirtualBets.Models;
using System;
using System.Linq;

namespace API.ScheduledTasks.VirtualBets.Util
{
    public static class FootballInitializers
    {

        /**
         * Function that insert in the database a new matchday
         * @param Match match. 
         *      The match of the matchday
         * @param Competition league. 
         *      The league to which the matchday belongs
         * @param Team homeTeam. 
         *      The home team which plays the match
         * @param Team awayTeam. 
         *      The away team which plays the match
         * @return bool
         *      True if the matchday doesn't exists and we did the insert well
         *      False if the matchday exist or there was any error in the insert statement
         */
        public static bool initializeMatchDay(Match match, Competition league, Team homeTeam, Team awayTeam, ApplicationDBContext _context)
        {
            try
            {
                int exist = _context.MatchDays.Where(md => md.CompetitionId == league.id &&
                                               md.HomeTeamId == homeTeam.id &&
                                               md.AwayTeamId == awayTeam.id).Count();
                if (exist != 0) return false;

                _context.Add(new MatchDay
                {
                    Competition = league,
                    date = parse(match.utcDate),
                    status = match.status,
                    number = match.matchday.Value,
                    group = match.group,
                    HomeTeam = homeTeam,
                    AwayTeam = awayTeam,
                    firstHalfHomeGoals = match.status == "FINISHED" ? match.score.halfTime.homeTeam : null,
                    firstHalfAwayGoals = match.status == "FINISHED" ? match.score.halfTime.awayTeam : null,
                    secondHalfHomeGoals = match.status == "FINISHED" ? (match.score.fullTime.homeTeam - match.score.halfTime.homeTeam): null,
                    secondHalfAwayGoals = match.status == "FINISHED" ? (match.score.fullTime.awayTeam - match.score.halfTime.awayTeam) : null,
                    fullTimeHomeGoals = match.status == "FINISHED" ? match.score.fullTime.homeTeam : null,
                    fullTimeAwayGoals = match.status == "FINISHED" ? match.score.fullTime.awayTeam : null
                });

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        /**
         * Function to update a matchday if it's ended
         * @param Match match. 
         *      The match of the matchday
         * @param Competition league. 
         *      The league to which the matchday belongs
         * @param Team homeTeam. 
         *      The home team which plays the match
         * @param Team awayTeam. 
         *      The away team which plays the match
         */
        public static void updateMatchDay(Match match, Competition league, Team homeTeam, Team awayTeam, ApplicationDBContext _context)
        {
            if (match.status != "FINISHED") return; //If the match isnt ended dont update

            var matchD = _context.MatchDays.Where(md => md.CompetitionId == league.id &&
                                                        md.HomeTeamId == homeTeam.id &&
                                                        md.AwayTeamId == awayTeam.id);

            if (matchD.Count() != 1) return; //Check if the matchday exists to avoid exceptions

            MatchDay matchday = matchD.First();

            matchday.status = match.status;
            matchday.firstHalfHomeGoals = match.score.halfTime.homeTeam;
            matchday.firstHalfAwayGoals = match.score.halfTime.awayTeam;
            matchday.secondHalfHomeGoals = match.score.fullTime.homeTeam-match.score.halfTime.homeTeam;
            matchday.secondHalfAwayGoals = match.score.fullTime.awayTeam-match.score.halfTime.awayTeam;
            matchday.fullTimeHomeGoals = match.score.fullTime.homeTeam;
            matchday.fullTimeAwayGoals = match.score.fullTime.awayTeam;

            _context.MatchDays.Update(matchday);
        }


        /**
         * Function that insert a new team  in the DB
         * @param string teamName.
         *      The name of the team
         * @return Team.
         *      The select of the team if its exists, the new team otherwise. 
         *      Null if any exception occurs 
         */
        public static Team initializeTeam(string teamName, ApplicationDBContext _context)
        {
            try
            {
                var teamExist = _context.Teams.Where(t => t.name == teamName);

                if (teamExist.Count() != 0) return teamExist.First();

                Team newTeam = new Team { name = teamName };

                _context.Teams.Add(newTeam);

                _context.SaveChanges();

                return newTeam;
            }
            catch (Exception)
            {
                return null;
            }
        }


        /**
         * Function that inserts a new competition in the DB
         * @param string leagueName.
         *      The name of the new competition
         * @return Competition.
         *      The select of the competition if its exists, the new competition otherwise. 
         *      Null if any exception occurs 
         */
        public static Competition initializeLeague(string leagueName, ApplicationDBContext _context)
        {
            try
            {
                var existLeague = _context.Competitions.Where(c => c.name == leagueName);

                if (existLeague.Count() != 0) return existLeague.First();

                Competition newComptetition = new Competition { name = leagueName };

                _context.Competitions.Add(newComptetition);

                _context.SaveChanges();

                return newComptetition;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /**
         * Function that parses a utcDate string value to
         * a DateTime value
         */
        private static DateTime parse(string date)
        {
            DateTime dateret = DateTime.MinValue;
            if (DateTime.TryParse(date, out dateret))
            {
                return dateret;
            }

            return dateret;
        }
    }
}
