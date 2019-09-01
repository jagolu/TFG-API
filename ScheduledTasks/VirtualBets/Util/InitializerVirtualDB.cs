using API.Data;
using API.Data.Models;
using API.ScheduledTasks.VirtualBets.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace API.ScheduledTasks.VirtualBets.Util
{
    /// <summary>
    /// Initializes or update the matchdays, teams and competitions in the database
    /// </summary>
    public class InitializerVirtualDB
    {
        //
        // ──────────────────────────────────────────────────────────────────────
        //   :::::: C L A S S   V A R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────
        //

        /// <value>The context of the database</value>
        private ApplicationDBContext _context;
        
        /// <value>The configuration of the application</value>
        private IConfiguration _configuration;
        
        /// <value>The http client factory to do the http request to the football api</value>
        private readonly IHttpClientFactory _http;


        //
        // ──────────────────────────────────────────────────────────────────────────
        //   :::::: C O N S T R U C T O R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Initializes the class vars
        /// </summary>
        /// <param name="context">The database context</param>
        /// <param name="config">The configuration of the application</param>
        /// <param name="http">The client factory</param>
        public InitializerVirtualDB(ApplicationDBContext context, IConfiguration config, IHttpClientFactory http)
        {
            _context = context;
            _configuration = config;
            _http = http;
        }


        //
        // ──────────────────────────────────────────────────────────────────────────────────
        //   :::::: P U B L I C   F U N C T I O N S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────────────
        //
        
        /// <summary>
        /// Initializes or updates the matchs and teams of a competition
        /// </summary>
        /// <param name="leagueId">The id in the api of the competition</param>
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