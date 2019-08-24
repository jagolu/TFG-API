using API.Data;
using API.Data.Models;
using API.ScheduledTasks.VirtualBets.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace API.ScheduledTasks.VirtualBets.Util
{
    public class InitializerVirtualDB
    {
        private ApplicationDBContext _context;
        private IConfiguration _configuration;
        private readonly IHttpClientFactory _http;

        public InitializerVirtualDB(ApplicationDBContext context, IConfiguration config, IHttpClientFactory http)
        {
            _context = context;
            _configuration = config;
            _http = http;
        }

        /**
         * Function to initialze a league and its teams and matchdays in the DB
         * @param string leagueid.
         *      The id of the competition in the api.
         * @return bool.
         *      True if the initialization was well, false otherwise.
         */
        public async Task InitializeAsync(string leagueId)
        {
            string token = _configuration["footballApi:token"];
            bool correct = true;

            CompetitionMatches comptMatchs = await APIRequest.getMatchesFromCompetition(token, leagueId, _http);
            CompetitionInfo comptInfo = await APIRequest.getCompetitionInfo(token, leagueId, _http);
            if (comptInfo == null) return;
            int actualMatchD = comptInfo.currentSeason.currentMatchday + 1;

            Competition league = FootballInitializers.initializeLeague(comptMatchs.competition.name, _context);


            comptMatchs.matches.ForEach(match =>
            {
                if(match.matchday <= actualMatchD)
                {
                    Team homeTeam = FootballInitializers.initializeTeam(match.homeTeam.name, _context);
                    Team awayTeam = FootballInitializers.initializeTeam(match.awayTeam.name, _context);

                    //There is any error inserting or updating the new matchday
                    if (!FootballInitializers.updateMatchDay(match, league, homeTeam, awayTeam, _context)) 
                    {
                        correct = false;
                    }
                }
            });

            league.actualMatchDay = actualMatchD;
            _context.Competitions.Update(league); //Set the actual matchday

            if(correct) _context.SaveChanges();
        }
    }
}