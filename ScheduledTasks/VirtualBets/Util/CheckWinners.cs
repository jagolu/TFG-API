using API.Areas.Alive.Util;
using API.Areas.Bet.Util;
using API.Data;
using API.Data.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.ScheduledTasks.VirtualBets.Util
{
    public static class CheckWinners
    {
        public static void checkWinner(FootballBet footballBet, ApplicationDBContext _context, IHubContext<NotificationHub> hub)
        {
            _context.Entry(footballBet).Reference("type").Load();
            _context.Entry(footballBet).Reference("typePay").Load();
            _context.Entry(footballBet).Reference("MatchDay").Load();
            _context.Entry(footballBet).Reference("Group").Load();
            _context.Entry(footballBet).Collection("userBets").Load();
            int time = getTypeTime(footballBet.type.name);
            Group group = footballBet.Group;
            List<List<Guid>> winners;

            winners = CheckBetType.isWinner(footballBet, _context) ? 
                      calculateResult(footballBet, time, _context) :
                      calculateTypeScore(footballBet, time, _context);

            if (CheckBetType.isJackpotExact(footballBet, _context))
            {
                payJackpot(footballBet, winners.First(), group, _context);
            }
            else if (CheckBetType.isJackpotCloser(footballBet, _context))
            {
                payJackpotCloser(footballBet, winners, group, _context);
            }
            else if (CheckBetType.isSoloExact(footballBet, _context))
            {
                paySoloBet(footballBet, winners.First(), group, _context);
            }
            else throw new Exception();

            _context.Update(group);
            launchNews(footballBet, _context, hub);
        }

        private static List<List<Guid>> calculateTypeScore(FootballBet fb, int time, ApplicationDBContext _context)
        {
            List<List<Guid>> ret = new List<List<Guid>>();
            ret.Add(new List<Guid>());
            ret.Add(new List<Guid>());
            List<Tuple<Guid, int>> diff = new List<Tuple<Guid, int>>();
            int? homeGoals = getGoals(fb.MatchDay, time, true);
            int? awayGoals = getGoals(fb.MatchDay, time, false);
            int closer = 999;
            fb.userBets.Where(b => b.valid).ToList().ForEach(ub =>
            {
                int diffHome = Math.Abs((int)(homeGoals - ub.homeGoals));
                int diffAway = Math.Abs((int)(awayGoals - ub.awayGoals));
                int fullDiff = diffHome + diffAway;
                if (fullDiff < closer) closer = fullDiff;
                diff.Add(new Tuple<Guid, int>(ub.id, diffHome+diffAway));
            });
            diff.ForEach(u =>
            {
                if(u.Item2 == 0)  ret.First().Add(u.Item1); 
                else if(u.Item2 == closer) ret.Last().Add(u.Item1);
            });

            return ret;
        }

        private static List<List<Guid>> calculateResult(FootballBet fb, int time, ApplicationDBContext _context)
        {
            List<List<Guid>> ret = new List<List<Guid>>();
            ret.Add(new List<Guid>());
            ret.Add(new List<Guid>());
            int winner = getWinner(fb.MatchDay, time);
            fb.userBets.Where(b => b.valid).ToList().ForEach(ub =>
            {
                if (ub.winner == winner) ret.First().Add(ub.id);
                else ret.Last().Add(ub.id);
            });

            return ret;
        }

        private static int? getGoals(MatchDay md, int time, bool team/*true --> homeTeam, false --> awayTeam*/)
        {
            if (time == 1) return team ? md.firstHalfHomeGoals : md.firstHalfAwayGoals;
            else if (time == 2) return team ? md.secondHalfHomeGoals : md.secondHalfAwayGoals;
            else return team ? md.fullTimeHomeGoals : md.fullTimeAwayGoals;
        }

        private static int getWinner(MatchDay md, int time)
        {
            int? homeGoals = getGoals(md, time, true);
            int? awayGoals = getGoals(md, time, false);

            if (homeGoals == awayGoals) return 0;
            else if (homeGoals > awayGoals) return 1;
            else return 2;
        }

        private static int getTypeTime(string time)
        {
            if (time.Contains("FULLTIME")) return 3;
            else if (time.Contains("FIRSTHALF")) return 1;
            else return 2;
        }

        private static void payJackpot(FootballBet fb, List<Guid> winners, Group group, ApplicationDBContext _context)
        {
            _context.Entry(group).Collection("users").Load();
            _context.Entry(fb).Collection("userBets").Load();
            int jackpot = fb.usersJoined * fb.minBet;

            if (winners.Count() == 0) return;

            int div_jack = jackpot / winners.Count();
            fb.userBets.Where(ub => winners.Contains(ub.id)).ToList().ForEach(userBet =>
            {
                userBet.earnings = div_jack;
                UserGroup u = group.users.Where(g => g.userid == userBet.userid).First();
                u.coins += div_jack;
                _context.Update(u);
            });

        }

        private static void payJackpotCloser(FootballBet fb, List<List<Guid>> winners, Group group, ApplicationDBContext _context)
        {
            _context.Entry(group).Collection("users").Load();
            _context.Entry(fb).Collection("userBets").Load();
            List<Guid> toPay = winners.First().Count() > 0 ? winners.First() : winners.Last();
            int jackpot = fb.usersJoined * fb.minBet;
            int div_jack = jackpot / toPay.Count();

            fb.userBets.Where(ub => toPay.Contains(ub.id)).ToList().ForEach(userBet =>
            {
                userBet.earnings = div_jack;
                UserGroup u = group.users.Where(g => g.userid == userBet.userid).First();
                u.coins += div_jack;
                _context.Update(u);
            });
        }

        private static void paySoloBet(FootballBet fb, List<Guid> winners, Group group, ApplicationDBContext _context)
        {
            _context.Entry(fb).Reference("type").Load();
            _context.Entry(fb).Reference("typePay").Load();
            _context.Entry(fb).Collection("userBets").Load();
            _context.Entry(group).Collection("users").Load();

            fb.userBets.Where(ub => winners.Contains(ub.id)).ToList().ForEach(userBet =>
            {
                UserGroup u = group.users.Where(g => g.userid == userBet.userid).First();
                double coinsWin = userBet.bet * fb.winRate;
                u.coins += (int)coinsWin;
                userBet.earnings = (int)coinsWin;
                _context.Update(u);
            });
        }

        private static void launchNews(FootballBet fb, ApplicationDBContext _context, IHubContext<NotificationHub> hub)
        {
            _context.Entry(fb).Reference("Group").Load();
            Group group = fb.Group;
            List<User> newGroups = new List<User>();

            Areas.Home.Util.GroupNew.launch(null, group, fb, Areas.Home.Models.TypeGroupNew.PAID_BETS_GROUP, false, _context);

            _context.Entry(fb).Collection("userBets").Load();
            fb.userBets.ToList().ForEach(u => 
            {
                _context.Entry(u).Reference("User").Load();
                if (newGroups.All(uu => uu.id != u.userid)) newGroups.Add(u.User);
            });

            newGroups.ForEach(async u =>
            {
                Areas.Home.Util.GroupNew.launch(u, group, fb, Areas.Home.Models.TypeGroupNew.PAID_BETS_USER, false, _context);
                await SendNotification.send(hub, group.name, u, Areas.Alive.Models.NotificationType.PAID_BETS, _context);
            });
        }
    }
}
