using API.Areas.Bet.Models;
using API.Areas.GroupManage.Util;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;


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
            UserGroup ugCaller = new UserGroup();
            Group group = new Group();
            FootballBet fb = new FootballBet();

            if(!UserInGroup.checkUserInGroup(caller.id, ref group, order.groupName, ref ugCaller, _context))
            {
                return BadRequest();
            }
            if (!getBet(ref fb, order.footballbet, group))
            {
                return BadRequest();
            }
            if(!checkUserInBet(fb, caller))
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

                ugCaller.coins -= order.bet;
                _context.Update(ugCaller);

                _context.SaveChanges();

                return Ok(GroupPageManager.GetPage(caller, group, _context));
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        private bool getBet(ref FootballBet fb, string betId, Group group)
        {
            List<FootballBet> fbs = _context.FootballBets
                .Where(md => md.id.ToString() == betId && md.groupId == group.id).ToList();
            if (fbs.Count() != 1)
            {
                return false;
            }
            if (group.type)
            {
                return false;
            }

            fb = fbs.First();
            return true;
        }

        private bool checkBet(int userBet, int userCoins, FootballBet fb)
        {
            _context.Entry(fb).Reference("typePay").Load();
            bool typeJackpot = fb.typePay.name.Contains("JACKPOT");

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
            _context.Entry(fb).Reference("type").Load();
            bool type_winner = fb.type.name.Contains("WINNER");

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

        private bool checkUserInBet(FootballBet fb, User caller)
        {
            var existBet =_context.UserFootballBet.Where(ufb => ufb.userId == caller.id && ufb.FootballBetId == fb.id);
            if(existBet.Count() != 0)
            {
                return false;
            }
            return true;
        }
    }
}
