using API.Data.Models;
using API.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Data
{
    public class DBInitializer
    {
        private static void InitializeRoles(ApplicationDBContext context)
        {
            var roles = new Role[] {
                new Role{name="ADMIN"},
                new Role{name="NORMAL_USER"},
                new Role{name="GROUP_MAKER" },
                new Role{name="GROUP_ADMIN"},
                new Role{name="GROUP_NORMAL"}
            };

            foreach (Role r in roles) {
                Boolean isIn = false;
                foreach(Role r2 in context.Role) {
                    if (r.name == r2.name) {
                        isIn = true;
                        break;
                    }
                }
                if (!isIn) context.Role.Add(r);
            }
        }

        private static void InitializeTypeFootballBet(ApplicationDBContext context)
        {
            var types = new TypeFootballBet[]
            {
                new TypeFootballBet{name="FULLTIME_SCORE", winRate=0.55, winLoseCancel=0.1,
                    description ="The players must guess the exact result of the match."},
                new TypeFootballBet{name="FIRSTHALF_SCORE", winRate=0.6, winLoseCancel=0.12,
                    description ="The players must guess the exact result of the first half of the match."},
                new TypeFootballBet{name="SECONDHALF_SCORE", winRate=0.6, winLoseCancel=0.12,
                    description ="The players must guess the exact result of the second half of the match."},
                new TypeFootballBet{name="FULLTIME_WINNER", winRate=0.35, winLoseCancel=0.13,
                    description ="The players must guess the winner of the match."},
                new TypeFootballBet{name="FIRSTHALF_WINNER", winRate=0.4, winLoseCancel=0.15,
                    description ="The players must guess the winner of the first half of the match."},
                new TypeFootballBet{name="SECONDHALF_WINNER", winRate=0.4, winLoseCancel=0.15,
                    description ="The players must guess the winner of the second half of the match."}
            };

            foreach(TypeFootballBet fb in types)
            {
                if(context.TypeFootballBet.Where(t => t.name == fb.name).Count() == 0)
                {
                    context.TypeFootballBet.Add(fb);
                }
            }
        }

        private static void InitializeTypePay(ApplicationDBContext context)
        {
            var types = new TypePay[]
            {
                new TypePay{name="JACKPOT_EXACT_BET", winRate=0, winLoseCancel=100,
                    description ="It has a jackpot. The jackpot will be for the player who hits the exact result. If nobody wins, everybody will lose their coins."},
                new TypePay{name="JACKPOT_CLOSER_BET", winRate=0, winLoseCancel=100,
                    description ="It has a jackpot. The jackpot will be for the player or players who come closest to the exact result."},
                new TypePay{name="SOLO_EXACT_BET", winRate=1.5, winLoseCancel=0.3,
                    description ="Every player bets alone and win a prize by a winrate, if the player does not win, the player will lose his coins."},
            };

            foreach(TypePay fb in types)
            {
                if(context.TypePay.Where(t => t.name == fb.name).Count() == 0)
                {
                    context.TypePay.Add(fb);
                }
            }
        }

        public static void Initialize(ApplicationDBContext context)
        {
            context.Database.EnsureCreated();

            InitializeRoles(context);
            InitializeTypeFootballBet(context);
            InitializeTypePay(context);

            context.SaveChanges();

            createDevelopmentUser(context); //Test users

            context.SaveChanges();

            //aumentarFechaParapruebas(context);//Para pruebas, se borra despues
        }

        private static void createDevelopmentUser(ApplicationDBContext context)
        {
            List<User> test_users = new List<User>();

            for(int i = 0; i < 7; i++)
            {
                test_users.Add(new User
                {
                    email = "u"+i+"@gmail.com",
                    nickname = "u"+i+"_test",
                    password = PasswordHasher.hashPassword("asdfasdf1A"),
                    tokenValidation = null,
                    role = context.Role.Where(r => r.name == "NORMAL_USER").First()
                });
            }
            User admin = new User
            {
                email = "a@gmail.com",
                nickname = "a_ADMIN",
                password = PasswordHasher.hashPassword("asdfasdf1A"),
                tokenValidation = null,
                role = context.Role.Where(r => r.name == "ADMIN").First()
            };

            test_users.ForEach(tu =>
            {
                if (context.User.Where(u => u.email == tu.email).Count() == 0)
                {
                    context.Add(tu);
                    context.Limitations.Add(new Limitations { User = tu });
                }
            });
            if (context.User.Where(u => u.email == "a@gmail.com").Count() == 0)
            {
                context.Add(admin);
                context.Limitations.Add(new Limitations { User = admin });
            }
        }




        private static void aumentarFechaParapruebas(ApplicationDBContext context)
        {
            context.MatchDays.ToList().ForEach(md =>
            {
                //md.date = md.date.AddMonths(5);
                //md.status = "SCHEDULED";
                md.status = "FINISHED";
            });

            context.SaveChanges();
        }
    }
}
