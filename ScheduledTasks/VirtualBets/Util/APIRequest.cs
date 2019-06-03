using API.ScheduledTasks.VirtualBets.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace API.ScheduledTasks.VirtualBets.Util
{
    public static class APIRequest
    {
        private static readonly string _baseUrl = "https://api.football-data.org/v2/";

        /**
         * Function to get the matches from a specific competition
         * @param string token.
         *      The secret token key of the api.
         * @param string leagueid.
         *      The id of the competition in the api.
         * @param IHttpClientFactory _http.
         * @return CompetitionMatches.
         *      The matches in the competition
         */
        public async static Task<CompetitionMatches> getMatchesFromCompetition(string token, string leagueId, IHttpClientFactory _http)
        {
            string path = "competitions/" + leagueId + "/matches";

            string result = await getRequest(path, _http, token);

            try
            {
                CompetitionMatches comptMatchs = JsonConvert.DeserializeObject<CompetitionMatches>(result);

                return comptMatchs;
            }
            catch (Exception) //Some kind of error
            {
                try //If ok the error is that we only can do 10 requests per min
                {
                    ErrorFootballApiMessage err = JsonConvert.DeserializeObject<ErrorFootballApiMessage>(result);

                    Thread.Sleep(new TimeSpan(0, 1, 0)); //Wait a minute retry

                    if (err.errorCode == 429) return await getMatchesFromCompetition(token, leagueId, _http);

                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }


        /**
         * Function to get the matches from a specific competition in a specific matchday
         * @param string token.
         *      The secret token key of the api.
         * @param string leagueid.
         *      The id of the competition in the api.
         * @param int matchday.
         *      The matchday in te competition
         * @param IHttpClientFactory _http.
         * @return CompetitionMatches.
         *      The matches in the competition
         */
        public async static Task<CompetitionMatches> getMatchesFromMatchDay(string token, string leagueid, int matchday, IHttpClientFactory _http)
        {
            string path = "competitions/" + leagueid + "/matches?matchday="+matchday;

            string result = await getRequest(path, _http, token);

            try
            {
                CompetitionMatches comptMatchs = JsonConvert.DeserializeObject<CompetitionMatches>(result);
                return comptMatchs;
            }
            catch (Exception) //Some kind of error
            {
                try //If ok the error is that we only can do 10 requests per min
                {
                    ErrorFootballApiMessage err = JsonConvert.DeserializeObject<ErrorFootballApiMessage>(result);

                    Thread.Sleep(new TimeSpan(0, 1, 0)); //Wait a minute retry

                    if(err.errorCode == 429) return await getMatchesFromMatchDay(token, leagueid, matchday, _http);

                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }


        /**
         * Function to get the info of an speficic competition
         * @param string token.
         *      The secret token key of the api.
         * @param string leagueid.
         *      The id of the competition in the api.
         * @param IHttpClientFactory _http.
         * @return CompetitionInfo.
         *      The competition info
         */
        public async static Task<CompetitionInfo> getCompetitionInfo(string token, string leagueid, IHttpClientFactory _http)
        {
            string path = "competitions/" + leagueid;

            string result = await getRequest(path, _http, token);

            try
            {
                CompetitionInfo comptInfo = JsonConvert.DeserializeObject<CompetitionInfo>(result);

                return comptInfo;
            }
            catch (Exception) //Some kind of error
            {
                try //If ok the error is that we only can do 10 requests per min
                {
                    ErrorFootballApiMessage err = JsonConvert.DeserializeObject<ErrorFootballApiMessage>(result);

                    Thread.Sleep(new TimeSpan(0, 1, 0)); //Wait a minute retry

                    if (err.errorCode == 429) return await getCompetitionInfo(token, leagueid, _http);

                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }


        /**
         * Funtion to do a GET Request to the football api
         * @param string path.
         *      The path of the GET Request
         * @param IHttpClientFactory _http.
         * @param string token.
         *      The secret token key of the api
         * @return string.
         *      The result ready to be formatted
         */
        private async static Task<String> getRequest(string path, IHttpClientFactory _http, string token)
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                _baseUrl + path
            );

            var client = _http.CreateClient();

            client.DefaultRequestHeaders.Add("X-Auth-Token", token);

            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode) return null;

            string result = await response.Content.ReadAsStringAsync();

            return result;
        }
    }
}
