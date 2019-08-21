using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using API.Areas.Alive.Util;
using API.Areas.Bet.Util;
using API.Areas.GroupManage.Util;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace API.Areas.Bet.Controllers
{
    [Route("Bet/[action]")]
    [ApiController]
    public class CancelFootballBetController : ControllerBase
    {
        private ApplicationDBContext _context;
        private readonly IServiceScopeFactory _scopeFactory;
        private IHubContext<NotificationHub> _hub;

        public CancelFootballBetController(ApplicationDBContext context, IServiceScopeFactory sf, IHubContext<NotificationHub> hub)
        {
            _context = context;
            _scopeFactory = sf;
            _hub = hub;
        }

        [HttpGet]
        [Authorize]
        [ActionName("CancelFootballBet")]
        public IActionResult cancel([Required] string betId)
        {
            User user = TokenUserManager.getUserFromToken(HttpContext, _context);
            if (!user.open) return BadRequest(new { error = "YoureBanned" });
            if (AdminPolicy.isAdmin(user, _context)) return BadRequest("notAllowed");

            List<FootballBet> bets = _context.FootballBets.Where(g => g.id.ToString() == betId).ToList();

            if (bets.Count() != 1) 
            {
                return BadRequest();
            }

            FootballBet bet = bets.First();
            _context.Entry(bet).Reference("Group").Load();
            Group group = bet.Group;
            if (!checkValidBet(bet))
            {
                return BadRequest();
            }
            if (!checkAdmin(bet, user)) 
            {
                return BadRequest();
            }
            if(bet.dateEnded < DateTime.Now)
            {
                return BadRequest(new { error = "CantCancelTheFootballBet" });
            }

            try
            {
                Home.Util.GroupNew.launch(null, group, bet, Home.Models.TypeGroupNew.FOOTBALLBET_CANCELLED_GROUP, false, _context);
                getMoneyBackAndLaunchNews(bet, group);

                bet.cancelled = true;
                bet.dateCancelled = DateTime.Now;
                
                _context.SaveChanges();
                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                    Group dbgroup = dbContext.Group.Where(g => g.name == group.name).First();
                    User dbUser = dbContext.User.Where(u => u.id == user.id).First();

                    return Ok(GroupPageManager.GetPage(dbUser, dbgroup, dbContext));
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        private bool checkValidBet(FootballBet bet)
        {
            if (bet.cancelled || bet.ended)
            {
                return false;
            }

            return true;
        }

        private bool checkAdmin(FootballBet bet, User u)
        {
            _context.Entry(bet).Reference("Group").Load();
            _context.Entry(bet.Group).Collection("users").Load();
            Role maker = RoleManager.getGroupMaker(_context);

            List<UserGroup> admins = bet.Group.users.Where(uu => uu.role == maker && uu.userid == u.id).ToList();
            if(admins.Count() != 1)
            {
                return false;
            }

            return true;
        }

        private void getMoneyBackAndLaunchNews(FootballBet bet, Group group)
        {
            _context.Entry(bet).Collection("userBets").Load();
            _context.Entry(group).Collection("users").Load();
            bool isJackpot = CheckBetType.isJackpot(bet, _context);

            bet.userBets.ToList().ForEach(ub =>
            {
                _context.Entry(ub).Reference("User").Load();
                UserGroup userg = group.users.Where(u => u.userid == ub.userid).First();
                int coinsBet = ub.bet;

                if (isJackpot || ub.valid)
                {
                    userg.coins += coinsBet;
                }
                else
                {
                    int retCoins = CheckBetType.calculateCancelRate(bet, coinsBet, _context);
                    userg.coins += (coinsBet - retCoins);
                }
                _context.UserGroup.Update(userg);
                _context.SaveChanges();

                _context.Entry(userg).Reference("User").Load();
            });

            _context.UserFootballBet.RemoveRange(bet.userBets.ToList());
            _context.SaveChanges();

            _context.Entry(group).Collection("users");
            group.users.ToList().ForEach(async u =>
            {
                _context.Entry(u).Reference("role").Load();
                _context.Entry(u).Reference("User").Load();
                User recv = u.User;

                Home.Util.GroupNew.launch(recv, group, bet, Home.Models.TypeGroupNew.FOOTBALLBET_CANCELLED_USER, u.role == RoleManager.getGroupMaker(_context), _context);
                await SendNotification.send(_hub, group.name, recv, Alive.Models.NotificationType.CANCELLED_FOOTBALLBET, _context);
            });
        }
    }
}
