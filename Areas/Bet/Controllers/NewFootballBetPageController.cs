using API.Areas.Bet.Models;
using API.Areas.GroupManage.Util;
using API.Data;
using API.Data.Models;
using API.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace API.Areas.Bet.Controllers
{
    [Route("Bet/[action]")]
    [ApiController]
    public class NewFootballBetPageController : ControllerBase
    {
        private ApplicationDBContext _context;

        public NewFootballBetPageController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        [ActionName("FootBallBetPage")]
        public IActionResult getFootBallPage([Required] string groupName)
        {
            User caller = TokenUserManager.getUserFromToken(HttpContext, _context);
            UserGroup ugCaller = new UserGroup();
            Group group = new Group();
            if(!CallerInGroup.CheckUserCapabilities(caller, ref group, groupName, TypeCheckCapabilites.STARTCREATE_FOOTBALL_BET, _context))
            {
                return BadRequest();
            }
            if (!checkMaxBetAllowed(group))
            {
                return Ok("MaximunWeekBetsReached");
            }

            try
            {
                List<FootBallMatch> availableMatches =  getAvailableMatchDays(group);
                return Ok(getAvailableBets(availableMatches));
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
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

        private List<FootBallMatch> getAvailableMatchDays(Group group)
        {
            DateTime now = DateTime.UtcNow;
            DateTime aWeek = DateTime.UtcNow.AddDays(8);
            List<TypeFootballBet> allTypes = _context.TypeFootballBet.ToList();
            List<FootballBet> doneBets = group.bets.ToList(); //Group bets
            List<FootBallMatch> retmatchs = new List<FootBallMatch>(); //return array
            List<MatchDay> availableMatchs = _context.MatchDays.Where(
                md => md.date > now && md.date < aWeek && md.status == "SCHEDULED").ToList(); //matchdays from now to a week

            availableMatchs.ForEach(md =>
            {
                var sameBet = doneBets.Where(db => db.matchdayId == md.id);
                List<TypeFootballBet> allowedTypes = allTypes;
                if (sameBet.Count() != 0)
                {
                    allowedTypes = availableTypeBets(sameBet.First());
                }
                if(allowedTypes.Count() != 0)
                {
                    addFootBallMatch(retmatchs, md, allowedTypes);
                }
            });

            return retmatchs;
        }

        private List<TypeFootballBet> availableTypeBets(FootballBet bet)
        {
            List<TypeFootballBet> alltypes = _context.TypeFootballBet.ToList();
            _context.Entry(bet).Reference("type").Load();
            return _context.TypeFootballBet.Where(t => !alltypes.Contains(bet.type)).ToList();
        }

        private void addFootBallMatch(List<FootBallMatch> mainArray, MatchDay md, List<TypeFootballBet> allowedTypes)
        {
            if(allowedTypes.Count() == 0)
            {
                return;
            }

            _context.Entry(md).Reference("HomeTeam").Load();
            _context.Entry(md).Reference("AwayTeam").Load();
            _context.Entry(md).Reference("Competition").Load();

            mainArray.Add(new FootBallMatch
            {
                competition = md.Competition.name,
                match_name = md.HomeTeam.name+" vs "+md.AwayTeam.name,
                matchday = md.id.ToString(),
                date = md.date,
                allowedTypeBets = convertTypeToString(allowedTypes),
            });
        }

        private List<NameWinRate> convertTypeToString(List<TypeFootballBet> types)
        {
            List<NameWinRate> ret = new List<NameWinRate>();
            types.ForEach(t =>
            {
                ret.Add(new NameWinRate
                {
                    name = t.name,
                    description = t.description,
                    winRate = t.winRate
                });
            });

            return ret;
        }

        private List<AvailableBet> getAvailableBets(List<FootBallMatch> matchs)
        {
            List<AvailableBet> availableBets = new List<AvailableBet>();
            _context.Competitions.ToList().ForEach(competition =>
            {
                List<FootBallMatch> mtchs_comp = matchs.Where(m => m.competition == competition.name).ToList();
                if(mtchs_comp.Count() != 0)
                {
                    availableBets.Add(new AvailableBet
                    {
                        competition = competition.name,
                        matches = mtchs_comp,
                        allowedTypePays = getTypePays()
                    });
                }

            });

            return availableBets;
        }

        private List<NameWinRate> getTypePays()
        {
            List<NameWinRate> tp = new List<NameWinRate>();
            _context.TypePay.ToList().ForEach(type =>
            {
                tp.Add(new NameWinRate
                {
                    name = type.name,
                    description = type.description,
                    winRate = type.winRate
                });
            });

            return tp;
        }
    }
}
