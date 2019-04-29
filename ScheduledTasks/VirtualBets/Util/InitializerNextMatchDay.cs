using API.Data;
using API.Data.Models;
using API.ScheduledTasks.VirtualBets.Models;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace API.ScheduledTasks.VirtualBets.Util
{
    public class InitializerNextMatchDay
    {
        private ApplicationDBContext _context;
        private IConfiguration _configuration;
        private readonly IHttpClientFactory _http;

        public InitializerNextMatchDay(ApplicationDBContext context, IConfiguration config, IHttpClientFactory http)
        {
            _context = context;
            _configuration = config;
            _http = http;
        }

        public async Task InitializeAsync(string leagueid, int matchday)
        {
            string token = _configuration["footballApi:token"];
            string fullnameleague = "";

            //Get the full name of the competition
            fullnameleague = leagueid.Equals("PL") ? "Premier League" : fullnameleague;
            fullnameleague = leagueid.Equals("PD") ? "Primera Division" : fullnameleague;

            Competition league = FootballInitializers.initializeLeague(fullnameleague, _context); //Get the Competition DBObject
            CompetitionMatches comptMatchs = await APIRequest.getMatchesFromMatchDay(token, leagueid, matchday, _http); //Matches in the matchday

            comptMatchs.matches.ForEach(match =>
            {
                Team homeTeam = FootballInitializers.initializeTeam(match.homeTeam.name, _context);
                Team awayTeam = FootballInitializers.initializeTeam(match.awayTeam.name, _context);

                FootballInitializers.initializeMatchDay(match, league, homeTeam, awayTeam, _context); //There is any error inserting the new matchday
            });

            _context.SaveChanges();
        }
    }
}
