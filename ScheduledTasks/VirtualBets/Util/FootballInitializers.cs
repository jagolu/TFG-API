using API.Data;
using API.Data.Models;
using API.ScheduledTasks.VirtualBets.Models;
using System;
using System.Linq;

namespace API.ScheduledTasks.VirtualBets.Util
{
    /// <summary>
    /// Class to manage the football data in the database with the football api
    /// </summary>
    public static class FootballInitializers
    {
        //
        // ──────────────────────────────────────────────────────────────────────────────────
        //   :::::: P U B L I C   F U N C T I O N S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Function that insert/update in the database a new matchday
        /// </summary>
        /// <param name="match">The match of the matchday</param>
        /// <param name="league">The league to which the matchday belongs</param>
        /// <param name="homeTeam">The home team which plays the match</param>
        /// <param name="awayTeam">The away team which plays the match</param>
        /// <param name="_context">The database context</param>
        /// <returns>True if the matchday was updated succesfully, false otherwise</returns>
        public static bool updateMatchDay(Match match, Competition league, Team homeTeam, Team awayTeam, ApplicationDBContext _context)
        {
            try
            {
                var exist = _context.MatchDays.Where(md => md.competitionid == league.id &&
                                                           md.homeTeamId == homeTeam.id &&
                                                           md.awayTeamid == awayTeam.id &&
                                                           md.season == match.season.id &&
                                                           md.number == match.matchday);
                if (exist.Count() != 0)
                {
                    updateExistingMatchDay(match, league, homeTeam, awayTeam, exist.First(), _context);
                    return true;
                }

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
                    fullTimeAwayGoals = match.status == "FINISHED" ? match.score.fullTime.awayTeam : null,
                    season = match.season.id
                });

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Initialize a team in the database
        /// </summary>
        /// <param name="teamName">The name of the team</param>
        /// <param name="_context">The database context</param>
        /// <returns>The team object of the database</returns>
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

        /// <summary>
        /// Initialize a competition in the database
        /// </summary>
        /// <param name="leagueName">The name of the competition</param>
        /// <param name="_context">The database context</param>
        /// <returns>The competition object of the database</returns>
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


        //
        // ────────────────────────────────────────────────────────────────────────────────────
        //   :::::: P R I V A T E   F U N C T I O N S : :  :   :    :     :        :          :
        // ────────────────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Update a matchday in the datbase
        /// </summary>
        /// <param name="match">The match of the matchday</param>
        /// <param name="league">The league to which the matchday belongs</param>
        /// <param name="homeTeam">The home team which plays the match</param>
        /// <param name="awayTeam">The away team which plays the match</param>
        /// <param name="md">The matchday to update</param>
        /// <param name="_context">The database context</param>
        private static void updateExistingMatchDay(Match match, Competition league, Team homeTeam, Team awayTeam, MatchDay md, ApplicationDBContext _context)
        {
            if (match.status != "FINISHED") return; //If the match isnt ended dont update

            md.status = match.status;
            md.firstHalfHomeGoals = match.score.halfTime.homeTeam;
            md.firstHalfAwayGoals = match.score.halfTime.awayTeam;
            md.secondHalfHomeGoals = match.score.fullTime.homeTeam-match.score.halfTime.homeTeam;
            md.secondHalfAwayGoals = match.score.fullTime.awayTeam-match.score.halfTime.awayTeam;
            md.fullTimeHomeGoals = match.score.fullTime.homeTeam;
            md.fullTimeAwayGoals = match.score.fullTime.awayTeam;
            md.season = match.season.id;

            _context.MatchDays.Update(md);
        }

        /// <summary>
        /// Parse a date string to datetime
        /// </summary>
        /// <param name="date">The string date to parse</param>
        /// <returns>The datetime parsed</returns>
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
