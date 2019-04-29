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
                    number = match.matchday.Value,
                    group = match.group,
                    HomeTeam = homeTeam,
                    AwayTeam = awayTeam,
                    homeGoals = match.status == "FINISHED" ? match.score.fullTime.homeTeam : null,
                    awayGoals = match.status == "FINISHED" ? match.score.fullTime.awayTeam : null,
                    homeEndPenalties = match.status == "FINISHED" ? match.score.penalties.homeTeam : null,
                    awayEndPenalties = match.status == "FINISHED" ? match.score.penalties.awayTeam : null
                });

                return true;
            }
            catch (Exception)
            {
                return false;
            }
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
    }
}
