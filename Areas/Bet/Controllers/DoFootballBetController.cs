using API.Areas.Bet.Models;
using API.Areas.Bet.Util;
using API.Areas.GroupManage.Util;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;


namespace API.Areas.Bet.Controllers
{
    [Route("Bet/[action]")]
    [ApiController]
    public class DoFootballBetController : ControllerBase
    {
        //
        // ──────────────────────────────────────────────────────────────────────
        //   :::::: C L A S S   V A R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────
        //

        /// <value>The database context of the application</value>
        private ApplicationDBContext _context;


        //
        // ──────────────────────────────────────────────────────────────────────────
        //   :::::: C O N S T R U C T O R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The database context</param>
        public DoFootballBetController(ApplicationDBContext context)
        {
            _context = context;
        }


        //
        // ──────────────────────────────────────────────────────────────────────────────────
        //   :::::: P U B L I C   F U N C T I O N S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────────────
        //
        
        [HttpPost]
        [Authorize]
        [ActionName("DoFootballBet")]
        /// <summary>
        /// Do a user fb on a fb
        /// </summary>
        /// <param name="order">The info to do the user fb</param>
        /// See <see cref="Areas.Bet.Models.DoFootballBet"/> to know the param structure
        /// <returns>IActionResult of the do user fb action</returns>
        /// See <see cref="Areas.GroupManage.Models.GroupPage"/> to know the response structure
        public IActionResult doFootballBet([FromBody] DoFootballBet order)
        {
            User caller = TokenUserManager.getUserFromToken(HttpContext, _context);
            if (!caller.open) return BadRequest(new { error = "YoureBanned" });
            if (AdminPolicy.isAdmin(caller, _context)) return BadRequest("notAllowed");
            UserGroup ugCaller = new UserGroup();
            Group group = new Group();
            FootballBet fb = new FootballBet();

            if(!UserInFootballBet.check(caller, ref group, order.groupName, ref ugCaller, ref fb, order.footballbet, _context))
            {
                return BadRequest();
            }
            if (fb.cancelled)
            {
                return BadRequest(new { error = "BetCancelled" });
            }
            if (fb.ended)
            {
                return BadRequest(new { error = "BetEnded" });
            }
            if (fb.dateLastBet < DateTime.Now)
            {
                return BadRequest(new { error = "BetLastBetPassed" });
            }
            if(!checkBet(order.bet, ugCaller.coins, fb))
            {
                return BadRequest();
            }
            if (!checkTypePriceWithBet(fb, order.homeGoals, order.awayGoals, order.winner))
            {
                return BadRequest();
            }
            try
            {
                _context.Add(new UserFootballBet
                {
                    FootballBet = fb,
                    User = caller,
                    bet = order.bet,
                    winner = order.winner,
                    homeGoals = order.homeGoals,
                    awayGoals = order.awayGoals
                });

                fb.usersJoined++;
                ugCaller.coins -= order.bet;
                _context.Update(ugCaller);
                _context.Update(fb);

                _context.SaveChanges();

                return Ok(GroupPageManager.GetPage(caller, group, _context));
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }


        //
        // ────────────────────────────────────────────────────────────────────────────────────
        //   :::::: P R I V A T E   F U N C T I O N S : :  :   :    :     :        :          :
        // ────────────────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Check the new user fb
        /// </summary>
        /// <param name="userBet">The coins bet by the user</param>
        /// <param name="userCoins">The total coins of the user</param>
        /// <param name="fb">The fb to bet on</param>
        /// <returns>True if the user can do the bet, false otherwise</returns>
        private bool checkBet(int userBet, int userCoins, FootballBet fb)
        {
            bool typeJackpot = CheckBetType.isJackpot(fb, _context);

            if(userBet > userCoins)
            {
                return false;
            }
            if(typeJackpot && userBet != fb.minBet)
            {
                return false;
            }
            if(!typeJackpot && (userBet<fb.minBet || userBet>fb.maxBet))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Check if the guessed of the bet is correct on its type
        /// </summary>
        /// <param name="fb">The fb</param>
        /// <param name="homeGoals">The home goals guessed by the user</param>
        /// <param name="awayGoals">The away goals guessed by the user</param>
        /// <param name="winner">The winner guessed by the user</param>
        /// <returns>Check if the guessed by the user is correct on the fb type</returns>
        private bool checkTypePriceWithBet(FootballBet fb, int ? homeGoals, int ? awayGoals, int? winner)
        {
            bool type_winner = CheckBetType.isWinner(fb, _context);

            if(type_winner && ( winner==null ||winner>2 || winner < 0))
            {
                return false;
            }
            if(!type_winner && (homeGoals==null || awayGoals == null || (homeGoals<0 || homeGoals>20) || (awayGoals < 0 || awayGoals > 20)))
            {
                return false;
            }

            return true;
        }
    }
}
