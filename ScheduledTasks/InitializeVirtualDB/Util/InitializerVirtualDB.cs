using API.Data;
using API.Data.Models;
using API.ScheduledTasks.InitializeVirtualDB.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace API.ScheduledTasks.InitializeVirtualDB.Util
{
    public class InitializerVirtualDB
    {
        private ApplicationDBContext _context;
        private IConfiguration _configuration;
        private readonly IHttpClientFactory _http;
        private readonly string  _baseUrl = "https://api.football-data.org/v2/";
        private ILogger _logger;

        public InitializerVirtualDB(ApplicationDBContext context, IConfiguration config, IHttpClientFactory http, ILogger logg)
        {
            _context = context;
            _configuration = config;
            _http = http;
            _logger = logg;
            _logger.LogInformation("inicializada clase inicializer");
        }

        public async Task<bool> InitializeAsync()
        {
            string token = _configuration["footballApi:token"];
            int matchd = 0;
            bool correct = true;

            CompetitionMatches comptMatchs = await getMatches(token);
            Competition league = initializeLeague(comptMatchs.competition.name);


            comptMatchs.matches.ForEach(match =>
            {
                if(match.status == "FINISHED" && match.matchday == matchd)
                {
                    Team homeTeam = initializeTeam(match.homeTeam.name);
                    Team awayTeam = initializeTeam(match.awayTeam.name);

                    if (!initializeMatchDay(match, league, homeTeam, awayTeam)) //There is any error inserting the new matchday
                    {
                        correct = false;
                    }
                }

                //Only inserts the matchdays which are finished
                if (match.status == "FINISHED" && match.matchday > matchd) matchd = match.matchday;
            });

            if(correct) _context.SaveChanges();

            return correct;
        }

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
        private bool initializeMatchDay(Match match, Competition league, Team homeTeam, Team awayTeam)
        {
            try
            {
                _context.Add(new MatchDay
                {
                    Competition = league,
                    number = match.matchday,
                    group = match.group,
                    HomeTeam = homeTeam,
                    AwayTeam = awayTeam,
                    homeGoals = match.score.fullTime.homeTeam,
                    awayGoals = match.score.fullTime.awayTeam,
                    homeEndPenalties = match.score.penalties.homeTeam,
                    awayEndPenalties = match.score.penalties.awayTeam
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
        private Team initializeTeam(string teamName)
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
        private Competition initializeLeague(string leagueName)
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
         * Function to call the football api and get the full competition matches & results
         * @param string token.
         *      Private token of the api
         * @return array<CompetitionMatches>.
         *      Array with the competition and the matches with their results.
         */
        private async Task<CompetitionMatches> getMatches(string token)
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                _baseUrl + "competitions/PD/matches"
            );

            var client = _http.CreateClient();

            client.DefaultRequestHeaders.Add("X-Auth-Token", token);

            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode) return null;

            string result = await response.Content.ReadAsStringAsync();

            CompetitionMatches comptMatchs = JsonConvert.DeserializeObject<CompetitionMatches>(result);

            return comptMatchs;
        }
    }
}