﻿using API.Areas.Alive.Util;
using API.Areas.Bet.Models;
using API.Areas.Bet.Util;
using API.Areas.GroupManage.Util;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Areas.Bet.Controllers
{
    [Route("Bet/[action]")]
    [ApiController]
    public class LaunchFootballBetController : ControllerBase
    {
        //
        // ──────────────────────────────────────────────────────────────────────
        //   :::::: C L A S S   V A R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────
        //

        /// <value>The database context of the application</value>
        private ApplicationDBContext _context;

        /// <value>The notification hub</value>
        private IHubContext<NotificationHub> _hub;


        //
        // ──────────────────────────────────────────────────────────────────────────
        //   :::::: C O N S T R U C T O R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The database context</param>
        /// <param name="hub">The notification hub</param>
        public LaunchFootballBetController(ApplicationDBContext context, IHubContext<NotificationHub> hub)
        {
            _context = context;
            _hub = hub;
        }


        //
        // ──────────────────────────────────────────────────────────────────────────────────
        //   :::::: P U B L I C   F U N C T I O N S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────────────
        //
        
        [HttpPost]
        [Authorize]
        [ActionName("LaunchFootBallBet")]
        /// <summary>
        /// Launchs a new fb
        /// </summary>
        /// <param name="order">The info to launch a new fb</param>
        /// See <see cref="Areas.Bet.Models.LaunchFootballBet"/> to know the param structure
        /// <returns>IActionResult of the launch fb action</returns>
        /// See <see cref="Areas.GroupManage.Models.GroupPage"/> to know the response structure
        public IActionResult launchBet([FromBody] LaunchFootballBet order)
        {
            User caller = TokenUserManager.getUserFromToken(HttpContext, _context);
            if (!caller.open) return BadRequest(new { error = "YoureBanned" });
            if (AdminPolicy.isAdmin(caller, _context)) return BadRequest("notAllowed");
            Group group = new Group();
            MatchDay match = new MatchDay();
            TypeFootballBet typeBet = new TypeFootballBet();
            TypePay typePay = new TypePay();

            if (!GroupMakerFuncionlities.checkFuncionality(caller, ref group, order.groupName, GroupMakerFuncionality.STARTCREATE_FOOTBALL_BET, _context))
            {
                return BadRequest();
            }
            if(!getMatchDay(ref match, order.matchday))
            {
                return BadRequest();
            }
            if (!checkMaxBetAllowed(group))
            {
                return BadRequest();
            }
            if(!checkParams(ref typeBet, order.typeBet, ref typePay, order.typePay))
            {
                return BadRequest();
            }
            if(order.lastBetTime > match.date)
            {
                return BadRequest();
            }
            if(!checkMaxMin(order.minBet, order.maxBet)){
                return BadRequest();
            }
            try
            {
                FootballBet fb = new FootballBet
                {
                    MatchDay = match,
                    Group = group,
                    type = typeBet,
                    typePay = typePay,
                    minBet = order.minBet,
                    maxBet = order.maxBet,
                    winRate = typeBet.winRate + typePay.winRate,
                    dateLastBet = order.lastBetTime,
                    dateEnded = match.date
                };
                _context.Add(fb);
                _context.SaveChanges();

                launchNews(caller, group, fb);

                return Ok(GroupPageManager.GetPage(caller, group, _context));
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        //
        // ──────────────────────────────────────────────────────────────────────────────────
        //   :::::: P U B L I C   F U N C T I O N S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Get the matchday that the fb is going to be
        /// </summary>
        /// <param name="match">A new matchday object, to save the matchday on it</param>
        /// <param name="matchday">The id of the matchday</param>
        /// <returns>True if the matchday exists, false otherwise</returns>
        private bool getMatchDay(ref MatchDay match, string matchday)
        {
            List<MatchDay> matchs = _context.MatchDays.Where(md => md.id.ToString() == matchday).ToList();
            if(matchs.Count() != 1)
            {
                return false;
            }
            match = matchs.First();

            if(DateTime.Now>match.date || DateTime.Now.AddDays(8) < match.date)
            {
                return false;
            }
            
            return true;
        }

        /// <summary>
        /// Check if the group has reached the max weekly bets allowed
        /// </summary>
        /// <param name="group">The group</param>
        /// <returns>True if the group can do more bets this week, false otherwise</returns>
        private bool checkMaxBetAllowed(Group group)
        {
            _context.Entry(group).Collection("bets").Load();
            DateTime now = DateTime.Now;
            DateTime aWeekAgo = DateTime.Now.AddDays(-7);
            //Bets released the last week and cancelled
            int betsCancelled = group.bets.Where(b => b.dateReleased > aWeekAgo && b.dateReleased < now && b.cancelled).Count();
            //There is a free cancellation, but just one (ever)
            betsCancelled = (betsCancelled - 1) <= 0 ? 0 : (betsCancelled - 1);
            //Bets released but not cancelled
            int betsAlreadyDone = group.bets.Where(b => b.dateReleased > aWeekAgo && b.dateReleased < now && !b.cancelled).Count();
            //Real amount of bets done
            betsAlreadyDone += betsCancelled;

            if (betsAlreadyDone >= group.maxWeekBets)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Check if the params of the fb are correct
        /// </summary>
        /// <param name="type_bet">A new TypeFootballBet object, to save the type on it</param>
        /// <param name="typeBet">The name of the bet type</param>
        /// <param name="type_pay">A new TypePay object, to save the type on it</param>
        /// <param name="typePay">The name of the pay type</param>
        /// <returns>True if the types are correct, false otherwise</returns>
        private bool checkParams(ref TypeFootballBet type_bet, string typeBet, ref TypePay type_pay, string typePay)
        {
            try
            {
                var typeB = _context.TypeFootballBet.Where(t => t.name == typeBet);
                var typeP = _context.TypePay.Where(t => t.name == typePay);

                if(typeB.Count() != 1 || typeP.Count() != 1)
                {
                    return false;
                }
                if(CheckBetType.typeIsWinner(typeB.First()) && CheckBetType.typeIsCloser(typeB.First()))
                {
                    return false;
                }

                type_bet = typeB.First();
                type_pay = typeP.First();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Check the max and min of the new fb
        /// </summary>
        /// <param name="min">The min coins of the new fb</param>
        /// <param name="max">The max coins of the new fb</param>
        /// <returns>True if the max and min are correct, false otherwise</returns>
        private bool checkMaxMin(int min, int max)
        {
            if (min > max) return false;
            if (min < 100 || min > 10000) return false;
            if (max < 100 || max > 10000) return false;

            return true;
        }

        /// <summary>
        /// Launch the news for the group and for the members on it
        /// </summary>
        /// <param name="u">The user who has launch the fb</param>
        /// <param name="group">The group where the new fb has been launched</param>
        /// <param name="fb">The fb that has been launched</param>
        private void launchNews(User u, Group group, FootballBet fb)
        {
            _context.Entry(group).Collection("users").Load();
            Home.Util.GroupNew.launch(null, group, fb, Home.Models.TypeGroupNew.LAUNCH_FOOTBALLBET_GROUP, false, _context);

            group.users.Where(g => !g.blocked).ToList().ForEach(async ug =>
            {
                _context.Entry(ug).Reference("User").Load();
                bool isLauncher = ug.userid == u.id;
                User recv = ug.User;

                Home.Util.GroupNew.launch(recv, group, fb, Home.Models.TypeGroupNew.LAUNCH_FOOTBALLBET_USER, isLauncher, _context);
                await SendNotification.send(_hub, group.name, recv, Alive.Models.NotificationType.NEW_FOOTBALLBET, _context);
            });
        }
    }
}
