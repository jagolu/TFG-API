using API.Data;
using API.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.ScheduledTasks.VirtualBets.Util
{
    public static class CheckWinners
    {
        public static void checkWinner(FootballBet footballBet, ApplicationDBContext _context)
        {
            _context.Entry(footballBet).Reference("type").Load();
            _context.Entry(footballBet).Reference("typePay").Load();
            _context.Entry(footballBet).Reference("MatchDay").Load();
            _context.Entry(footballBet).Reference("Group").Load();
            _context.Entry(footballBet).Collection("userBets").Load();
            int time = getTypeTime(footballBet.type.name);
            Group group = footballBet.Group;
            List<List<Guid>> winners;

            winners = footballBet.type.name.Contains("WINNER") ? 
                      calculateResult(footballBet, time, _context) :
                      calculateTypeScore(footballBet, time, _context);

            if (footballBet.typePay.name.Contains("JACKPOT_EXACT_BET"))
            {
                payJackpot(footballBet, winners.First(), group, _context);
            }
            else if (footballBet.typePay.name.Contains("JACKPOT_CLOSER_BET"))
            {
                payJackpotCloser(footballBet, winners, group, _context);
            }
            else if (footballBet.typePay.name.Contains("SOLO_EXACT_BET"))
            {
                paySoloBet(footballBet, winners.First(), group, _context);
            }
            else throw new Exception();

            _context.Update(group);
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
                diff.Add(new Tuple<Guid, int>(ub.userId, diffHome+diffAway));
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
                if (ub.winner == winner) ret.First().Add(ub.userId);
                else ret.Last().Add(ub.userId);
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
            int jackpot = fb.userBets.Count() * fb.minBet;

            if (winners.Count() == 0) return;

            int div_jack = jackpot / winners.Count();
            group.users.Where(u => winners.Contains(u.userId)).ToList().ForEach(user =>
            {
                user.coins += div_jack;
                fb.userBets.Where(uu => uu.userId == user.userId).First().earnings = div_jack;
                _context.Update(user);
            });
        }

        private static void payJackpotCloser(FootballBet fb, List<List<Guid>> winners, Group group, ApplicationDBContext _context)
        {
            _context.Entry(group).Collection("users").Load();
            List<Guid> toPay = winners.First().Count() > 0 ? winners.First() : winners.Last();
            int jackpot = fb.userBets.Count() * fb.minBet;
            int div_jack = jackpot / toPay.Count();

            group.users.Where(u => toPay.Contains(u.userId)).ToList().ForEach(user =>
            {
                user.coins += div_jack;
                fb.userBets.Where(uu => uu.userId == user.userId).First().earnings = div_jack;
                _context.Update(user);
            });
        }

        private static void paySoloBet(FootballBet fb, List<Guid> winners, Group group, ApplicationDBContext _context)
        {
            _context.Entry(fb).Reference("type").Load();
            _context.Entry(fb).Reference("typePay").Load();
            _context.Entry(group).Collection("users").Load();
            double winRate = fb.type.winRate + fb.typePay.winRate;

            group.users.Where(u => winners.Contains(u.userId)).ToList().ForEach(user =>
            {
                int coinsBet = fb.userBets.Where(ub => ub.userId == user.userId).First().bet;
                double coinsWin = coinsBet * winRate;
                user.coins += (int)coinsWin;
                fb.userBets.Where(uu => uu.userId == user.userId).First().earnings = (int)coinsWin;
                _context.Update(user);
            });
        }
    }
}
