﻿using API.Data.Models;
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
                new TypeFootballBet{name="FULLTIME_SCORE"},
                new TypeFootballBet{name="PARTTIME_SCORE"},
                new TypeFootballBet{name="FULLTIME_WINNER"},
                new TypeFootballBet{name="PARTTIME_WINNER"}
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
                new TypePay{name="EXACT_BET"},
                new TypePay{name="CLOSER_BET"}
            };

            foreach(TypePay fb in types)
            {
                if(context.TypeFootballBet.Where(t => t.name == fb.name).Count() == 0)
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
                md.date = md.date.AddMonths(4);
                md.status = "SCHEDULED";
            });

            context.SaveChanges();
        }
    }
}
