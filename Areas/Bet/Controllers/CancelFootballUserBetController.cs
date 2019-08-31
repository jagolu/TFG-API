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
        public CancelFootballUserBetController(ApplicationDBContext context)
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
        [ActionName("CancelUserFootballBet")]
        /// <summary>
        /// Cancel a user fb
        /// </summary>
        /// <param name="order">The info to cancel the user fb</param>
        /// See <see cref="Areas.Bet.Models.CancelUserFootballBet"/> to know the param structure
        /// <returns>The IActionResult of the cancel a user fb action</returns>
        /// See <see cref="Areas.GroupManage.Models.GroupPage"/> to know the response structure
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
                ugCaller.coins += CheckBetType.calculateCancelRate(footballBet, userFootballBet.bet, _context);
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


        //
        // ────────────────────────────────────────────────────────────────────────────────────
        //   :::::: P R I V A T E   F U N C T I O N S : :  :   :    :     :        :          :
        // ────────────────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Check if the order is correct
        /// </summary>
        /// <param name="userFB">A new UserFootballBet object, to save the user fb in it</param>
        /// <param name="userBet">The id of the user fb</param>
        /// <param name="footballBet">The fb where the user fb belongs to</param>
        /// <param name="caller">The user who wants to cancel the user fb</param>
        /// <returns>True if user fb exists and belongs to the user, false otherwise</returns>
        private bool checkBet(ref UserFootballBet userFB, string userBet, FootballBet footballBet, User caller)
        {
            //User bets by the user with the userBet & valids
            _context.Entry(footballBet).Collection("userBets").Load();
            List<UserFootballBet> bet = footballBet.userBets.Where(ub => 
                        ub.id.ToString() == userBet && ub.userid == caller.id && ub.valid).ToList();

            if(bet.Count() != 1)
            {
                return false;
            }

            userFB = bet.First();
            return true;
        }
    }
}
