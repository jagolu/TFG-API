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
        public ApplicationDBContext _context;

        public DoFootballBetController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        [ActionName("DoFootballBet")]
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
