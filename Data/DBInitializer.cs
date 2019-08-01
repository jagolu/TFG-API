using API.Areas.Bet.Util;
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
                new Role{name="GROUP_MAKER"},
                new Role{name="GROUP_ADMIN"},
                new Role{name="GROUP_NORMAL"}
            };

            roles.Where(r => context.Role.All(role => role.name != r.name)).ToList().ForEach(rr => context.Add(rr));
        }

        private static void InitializeTypeFootballBet(ApplicationDBContext context)
        {
            var types = new TypeFootballBet[]
            {
                new TypeFootballBet{name="FULLTIME_SCORE", winRate=0.55, winLoseCancel=0.1,
                    description ="Los participantes deberán acertar el resultado exacto al final del partido."},
                new TypeFootballBet{name="FIRSTHALF_SCORE", winRate=0.6, winLoseCancel=0.12,
                    description ="Los participantes deberán acertar el resultado exacto del parcial de la primera parte."},
                new TypeFootballBet{name="SECONDHALF_SCORE", winRate=0.6, winLoseCancel=0.12,
                    description ="Los participantes deberán acertar el resultado exacto del parcial de la segunda parte."},
                new TypeFootballBet{name="FULLTIME_WINNER", winRate=0.35, winLoseCancel=0.13,
                    description ="Los participantes deberán acertar el ganador al final del partido."},
                new TypeFootballBet{name="FIRSTHALF_WINNER", winRate=0.4, winLoseCancel=0.15,
                    description ="Los participantes deberán acertar el ganador del parcial de la primera parte."},
                new TypeFootballBet{name="SECONDHALF_WINNER", winRate=0.4, winLoseCancel=0.15,
                    description ="Los participantes deberán acertar el ganador del parcial de la segunda parte."}
            };

            types.Where(t => context.TypeFootballBet.All(type => type.name != t.name)).ToList().ForEach(tfb => context.Add(tfb));
        }

        private static void InitializeTypePay(ApplicationDBContext context)
        {
            var types = new TypePay[]
            {
                new TypePay{name=CheckBetType.getJackpotExact(), winRate=0, winLoseCancel=100,
                    description ="Tiene bote. El bote sera para el participante que acierte el resultado exacto. Si nadie acierta el resultado, todos perderán las monedas apostadas."},
                new TypePay{name=CheckBetType.getJackpotCloser(), winRate=0, winLoseCancel=100,
                    description ="Tiene bote. El bote será para el participante que se acerque más al resultado exacto."},
                new TypePay{name=CheckBetType.getSoloExact(), winRate=1.5, winLoseCancel=0.3,
                    description ="Cada participante participa el solo. Las apuestas tienen una cuota. Si el participante no gana perderá sus monedas."},
            };

            types.Where(t => context.TypePay.All(type => type.name != t.name)).ToList().ForEach(tp => context.Add(tp));
        }

        private static void createDevelopmentUser(ApplicationDBContext context)
        {
            List<User> test_users = new List<User>();
            Role normal = RoleManager.getNormalUser(context);
            string hashedPassword = PasswordHasher.hashPassword("asdfasdf1A");

            for (int i = 0; i < 7; i++)
            {
                test_users.Add(new User
                {
                    email = "u" + i + "@gmail.com",
                    nickname = "u" + i + "_test",
                    password = hashedPassword,
                    tokenValidation = null,
                    role = normal
                });
            }
            User admin = new User
            {
                email = "a@gmail.com",
                nickname = "a_ADMIN",
                password = hashedPassword,
                tokenValidation = null,
                role = RoleManager.getAdmin(context)
            };

            User admin2 = new User
            {
                email = "a2@gmail.com",
                nickname = "a2_ADMIN",
                password = hashedPassword,
                tokenValidation = null,
                role = RoleManager.getAdmin(context)
            };


            test_users.Where(u => context.User.All(dbUser => dbUser.email != u.email)).ToList().ForEach(newU => context.Add(newU));

            if (context.User.Where(u => u.email == "a@gmail.com").Count() == 0) context.Add(admin);
            if (context.User.Where(u => u.email == "a2@gmail.com").Count() == 0) context.Add(admin2);
        }

        private static void createNews(ApplicationDBContext context)
        {
            string title = "Aviso de los administradores!";
            var news = new New[]
            {
                new New
                {
                    Group = null, User = null, groupId = null, userId = null,
                    title = title,
                    message = "Beta de la aplicación lanzada!!",
                    date = new DateTime(2019, 07, 14)
                }
            };

            news.Where(nn => context.News.All(n => n.message != nn.message)).ToList().ForEach(nws => context.Add(nws));
        }

        public static void Initialize(ApplicationDBContext context)
        {
            context.Database.EnsureCreated();

            InitializeRoles(context);
            InitializeTypeFootballBet(context);
            InitializeTypePay(context);
            createNews(context);
            context.SaveChanges();

            createDevelopmentUser(context); //Test users & admin user
            context.SaveChanges();

            //aumentarFechaParapruebas(context);//Para pruebas, se borra despues
        }





        private static void aumentarFechaParapruebas(ApplicationDBContext context)
        {
            context.MatchDays.ToList().ForEach(md =>
            {
                md.date = md.date.AddDays(+(2 * 7));
                //md.status = "SCHEDULED";
                Random rnd = new Random();
                //md.status = "FINISHED";
                md.firstHalfHomeGoals = rnd.Next(3);
                md.fullTimeHomeGoals = rnd.Next(3);
                md.fullTimeAwayGoals = rnd.Next(3);
                md.firstHalfAwayGoals = rnd.Next(3);
                //context.SaveChanges();
            });
            context.SaveChanges();
        }
    }
}
