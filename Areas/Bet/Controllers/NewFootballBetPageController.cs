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
            if (!caller.open) return BadRequest(new { error = "YoureBanned" });
            if (AdminPolicy.isAdmin(caller, _context)) return BadRequest("notAllowed");
            Group group = new Group();
            if(!GroupMakerFuncionlities.checkFuncionlity(caller, ref group, groupName, GroupMakerFuncionlity.STARTCREATE_FOOTBALL_BET, _context))
            {
                return BadRequest();
            }
            if(!checkMaxBetAllowed(group))
            {
                return Ok(getMaxReachResponse());
            }

            try
            {
                List<FootballMatch> availableMatches =  getAvailableMatchDays(group);
                LaunchFootballBetManager response = new LaunchFootballBetManager();
                response.typeBets = loadTypeFootballBet();
                response.typePays = loadTypePays();
                response.competitionMatches = getAvailableBets(availableMatches);

                return Ok(response);
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

        private List<FootballMatch> getAvailableMatchDays(Group group)
        {
            _context.Entry(group).Collection("bets").Load();
            DateTime now = DateTime.Now;
            DateTime aWeek = DateTime.Now.AddDays(8);
            List<TypeFootballBet> allTypes = _context.TypeFootballBet.ToList();
            List<FootballBet> doneBets = group.bets.ToList(); //Group bets
            List<FootballMatch> retmatchs = new List<FootballMatch>(); //return array
            List<MatchDay> availableMatchs = _context.MatchDays.Where(
                md => md.date > now && md.date < aWeek && md.status == "SCHEDULED").ToList(); //matchdays from now to a week

            availableMatchs.ForEach(md => 
            {
                List<FootballBet> betsOnTheMatch = doneBets.Where(db => db.matchdayid == md.id).ToList(); //Bets done on the matchday
                List<TypeFootballBet> allowedTypes = allTypes;

                if(betsOnTheMatch.Count() != 0)
                {
                    allowedTypes = availableTypeBets(betsOnTheMatch, allTypes);
                }

                addFootballMatch(retmatchs, md, allowedTypes);
            });

            return retmatchs;
        }

        private void addFootballMatch(List<FootballMatch> mainArray, MatchDay md, List<TypeFootballBet> allowedTypes)
        {
            if(allowedTypes.Count() == 0)
            {
                return;
            }

            _context.Entry(md).Reference("HomeTeam").Load();
            _context.Entry(md).Reference("AwayTeam").Load();
            _context.Entry(md).Reference("Competition").Load();

            mainArray.Add(new FootballMatch
            {
                competition = md.Competition.name,
                match_name = md.HomeTeam.name+" vs "+md.AwayTeam.name,
                matchday = md.id.ToString(),
                date = md.date,
                allowedTypeBets = getIdsFromTypeBets(allowedTypes),
            });
        }

        private List<AvailableBet> getAvailableBets(List<FootballMatch> matchs)
        {
            List<AvailableBet> availableBets = new List<AvailableBet>();
            _context.Competitions.ToList().ForEach(competition =>
            {
                List<FootballMatch> mtchs_comp = matchs.Where(m => m.competition == competition.name).ToList();
                if(mtchs_comp.Count() != 0)
                {
                    availableBets.Add(new AvailableBet
                    {
                        competition = competition.name,
                        matches = mtchs_comp
                    });
                }
            });

            return availableBets;
        }

        private List<string> getIdsFromTypeBets(List<TypeFootballBet> types)
        {
            List<string> typesRet = new List<string>();
            types.ForEach(t => typesRet.Add(t.id.ToString()));

            return typesRet;
        }

        private List<TypeFootballBet> availableTypeBets(List<FootballBet> bets, List<TypeFootballBet> alltypes)
        {
            List<TypeFootballBet> alltypesBk = new List<TypeFootballBet>(alltypes);

            bets.ForEach(bet =>
            {
                _context.Entry(bet).Reference("type").Load();
                List<TypeFootballBet> exists = alltypes.Where(t => t.id == bet.type.id).ToList();

                if (exists.Count() != 0)
                {
                    alltypesBk.Remove(exists.First());
                }
            });


            return alltypesBk;
        }

        private List<NameWinRate> loadTypeFootballBet()
        {
            List<NameWinRate> ret = new List<NameWinRate>();
            _context.TypeFootballBet.ToList().ForEach(t => ret.Add(new NameWinRate(t)));

            return ret;
        }

        private List<NameWinRate> loadTypePays()
        {
            List<NameWinRate> ret = new List<NameWinRate>();
            _context.TypePay.ToList().ForEach(t => ret.Add(new NameWinRate(t)));

            return ret;
        }

        private LaunchFootballBetManager getMaxReachResponse()
        {
            LaunchFootballBetManager res = new LaunchFootballBetManager();
            res.typeBets = new List<NameWinRate>();
            res.typePays = new List<NameWinRate>();
            res.competitionMatches = new List<AvailableBet>();
            res.competitionMatches.Add(new AvailableBet
            {
                matches = new List<FootballMatch>(),
                competition = "MaximunWeekBetsReached"
            });

            return res;
        }
    }
}
