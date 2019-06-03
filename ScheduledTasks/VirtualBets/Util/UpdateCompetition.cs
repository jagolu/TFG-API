using API.Data;
using API.Data.Models;
using API.ScheduledTasks.VirtualBets.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace API.ScheduledTasks.VirtualBets.Util
{
    public class UpdateCompetition
    {
        private ApplicationDBContext _context;
        private IConfiguration _configuration;
        private readonly IHttpClientFactory _http;

        public UpdateCompetition(ApplicationDBContext context, IConfiguration config, IHttpClientFactory http)
        {
            _context = context;
            _configuration = config;
            _http = http;
        }

        public async Task updateAsync(string leagueid, int actualMatchDay)
        {
            string token = _configuration["footballApi:token"];
            string fullnameleague = "";

            //Get the full name of the competition
            fullnameleague = leagueid.Equals("PL") ? "Premier League" : fullnameleague;
            fullnameleague = leagueid.Equals("PD") ? "Primera Division" : fullnameleague;

            Competition league = FootballInitializers.initializeLeague(fullnameleague, _context); //Get the Competition DBObject
            CompetitionMatches nextMatchDayMatches = await APIRequest.getMatchesFromMatchDay(token, leagueid, actualMatchDay+1, _http); //Matches the next matchday
            CompetitionMatches actualMatchDayMatches = await APIRequest.getMatchesFromMatchDay(token, leagueid, actualMatchDay, _http); //Matches the actual matchday

            for (int i = 0; i < nextMatchDayMatches.matches.Count(); i++)
            {
                //Update new matchday
                Team homeTeam = FootballInitializers.initializeTeam(nextMatchDayMatches.matches[i].homeTeam.name, _context);
                Team awayTeam = FootballInitializers.initializeTeam(nextMatchDayMatches.matches[i].awayTeam.name, _context);

                FootballInitializers.initializeMatchDay(nextMatchDayMatches.matches[i], league, homeTeam, awayTeam, _context); //There is any error inserting the new matchday                

               //Update actual matchday
                homeTeam = FootballInitializers.initializeTeam(actualMatchDayMatches.matches[i].homeTeam.name, _context);
                awayTeam = FootballInitializers.initializeTeam(actualMatchDayMatches.matches[i].awayTeam.name, _context);

                FootballInitializers.updateMatchDay(actualMatchDayMatches.matches[i], league, homeTeam, awayTeam, _context); //There is any error inserting the new matchday

            }

            //Update actual matchday
            CompetitionInfo comptInfo = await APIRequest.getCompetitionInfo(token, leagueid, _http);
            league.actualMatchDay = comptInfo.currentSeason.currentMatchday;
            _context.Competitions.Update(league);

            _context.SaveChanges();
        }
    }
}
