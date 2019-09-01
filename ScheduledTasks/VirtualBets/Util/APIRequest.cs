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
        //
        // ──────────────────────────────────────────────────────────────────────
        //   :::::: C L A S S   V A R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────
        //

        /// <value>The base url of the football api</value>
        private static readonly string _baseUrl = "https://api.football-data.org/v2/";


        //
        // ──────────────────────────────────────────────────────────────────────────────────
        //   :::::: P U B L I C   F U N C T I O N S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────────────
        //        

        /// <summary>
        /// Get the matches from a competition
        /// </summary>
        /// <param name="token">The secret token key of the api</param>
        /// <param name="leagueId">The id of the competition in the api</param>
        /// <param name="_http">The http factory</param>
        /// <returns>The matches in the competition</returns>
        /// See <see cref="ScheduledTasks.VirtualBets.Models.CompetitionMatches"/> to know the response structure
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

                    Thread.Sleep(new TimeSpan(0, 1, 0)); //Wait a minute & retry

                    if (err.errorCode == 429) return null;

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
        /// <summary>
        /// Get the info of a competition
        /// </summary>
        /// <param name="token">The secret token key of the api</param>
        /// <param name="leagueid">The id of the competition in the api</param>
        /// <param name="_http">The http factory</param>
        /// <returns>The competition info</returns>
        /// See <see cref="ScheduledTasks.VirtualBets.Models.CompetitionInfo"/> to know the response structure
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


        //
        // ────────────────────────────────────────────────────────────────────────────────────
        //   :::::: P R I V A T E   F U N C T I O N S : :  :   :    :     :        :          :
        // ────────────────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Do a http request to the football api
        /// </summary>
        /// <param name="path">The path of the GET Request</param>
        /// <param name="_http">The http factory</param>
        /// <param name="token">The secret token key of the api</param>
        /// <returns>The result ready to be formatted</returns>
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
