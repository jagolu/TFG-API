using API.Areas.Bet.Models;
using API.Areas.Bet.Util;
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
    public class CancelFootballUserBetController : ControllerBase
    {
        public ApplicationDBContext _context;

        public CancelFootballUserBetController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        [ActionName("CancelUserFootballBet")]
        public IActionResult cancelUserFootballBet([FromBody] CancelUserFootballBet order)
        {
            User caller = TokenUserManager.getUserFromToken(HttpContext, _context);
            if (!caller.open) return BadRequest(new { error = "YoureBanned" });
            if (AdminPolicy.isAdmin(caller, _context)) return BadRequest("notAllowed");
            UserGroup ugCaller = new UserGroup();
            Group group = new Group();
            FootballBet footballBet = new FootballBet();
            UserFootballBet userFootballBet = new UserFootballBet();

            if (!UserInFootballBet.check(caller, ref group, order.groupName, ref ugCaller, ref footballBet, order.footballBet, _context, false))
            {
                return BadRequest();
            }
            if (footballBet.cancelled)
            {
                return BadRequest(new { error = "CancelBetCancelled" });
            }
            if (footballBet.ended)
            {
                return BadRequest(new { error = "CancelBetEnded" });
            }
            if (footballBet.dateLastBet < DateTime.Now)
            {
                return BadRequest(new { error = "CancelBetLastBetPassed" });
            }
            if (!checkBet(ref userFootballBet, order.userBet, footballBet, caller))
            {
                return BadRequest();
            }
            try
            {
                ugCaller.coins += returnMoney(footballBet, userFootballBet.bet);
                userFootballBet.valid = false;
                userFootballBet.dateInvalid = DateTime.Now;

                _context.Update(ugCaller);
                _context.Update(userFootballBet);
                _context.SaveChanges();

                return Ok(GroupPageManager.GetPage(caller, group, _context));
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        private bool checkBet(ref UserFootballBet userFB, string userBet, FootballBet footballBet, User caller)
        {
            //User bets by the user with the userBet & valids
            _context.Entry(footballBet).Collection("userBets").Load();
            List<UserFootballBet> bet = footballBet.userBets.Where(ub => 
                        ub.id.ToString() == userBet && ub.userId == caller.id && ub.valid).ToList();

            if(bet.Count() != 1)
            {
                return false;
            }

            userFB = bet.First();
            return true;
        }

        private int returnMoney(FootballBet footballBet, int coinsBet)
        {
            _context.Entry(footballBet).Reference("type").Load();
            _context.Entry(footballBet).Reference("typePay").Load();
            double less1 = footballBet.type.winLoseCancel;
            double less2 = footballBet.typePay.winLoseCancel;

            //The player dont get back any coin (fuck cowards)
            if (less2 == 100) return 0;

            double ret_coins= coinsBet * (less1 + less2);

            return (int)Math.Round(ret_coins, MidpointRounding.AwayFromZero);
        }
    }
}
