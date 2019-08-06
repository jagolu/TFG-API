using API.Areas.Alive.Util;
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
        private ApplicationDBContext _context;
        private IHubContext<NotificationHub> _hub;

        public LaunchFootballBetController(ApplicationDBContext context, IHubContext<NotificationHub> hub)
        {
            _context = context;
            _hub = hub;
        }

        [HttpPost]
        [Authorize]
        [ActionName("LaunchFootBallBet")]
        public IActionResult launchBet([FromBody] LaunchFootballBet order)
        {
            User caller = TokenUserManager.getUserFromToken(HttpContext, _context);
            if (!caller.open) return BadRequest(new { error = "YoureBanned" });
            if (AdminPolicy.isAdmin(caller, _context)) return BadRequest("notAllowed");
            Group group = new Group();
            MatchDay match = new MatchDay();
            TypeFootballBet typeBet = new TypeFootballBet();
            TypePay typePay = new TypePay();

            if (!GroupMakerFuncionlities.checkFuncionlity(caller, ref group, order.groupName, GroupMakerFuncionlity.STARTCREATE_FOOTBALL_BET, _context))
            {
                return BadRequest();
            }
            if(!GetMatchDay(ref match, order.matchday))
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
                    dateEnded = match.date.AddDays(1)
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

        private bool GetMatchDay(ref MatchDay match, string matchday)
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

        private bool checkMaxMin(int min, int max)
        {
            if (min > max) return false;
            if (min < 100 || min > 10000) return false;
            if (max < 100 || max > 10000) return false;

            return true;
        }

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
