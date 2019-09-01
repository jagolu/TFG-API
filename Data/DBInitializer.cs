using API.Areas.Bet.Util;
using API.Data.Models;
using API.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Data
{
    /// <summary>
    /// Initializes the database
    /// </summary>
    public class DBInitializer
    {
        //
        // ────────────────────────────────────────────────────────────────────────────────────
        //   :::::: P R I V A T E   F U N C T I O N S : :  :   :    :     :        :          :
        // ────────────────────────────────────────────────────────────────────────────────────
        //
        
        /// <summary>
        /// Initializes the roles 
        /// </summary>
        /// <param name="context">The database context</param>
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

        /// <summary>
        /// Initializes the football bet types
        /// </summary>
        /// <param name="context">The database context</param>
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

        /// <summary>
        /// Initializes the pay types
        /// </summary>
        /// <param name="context">The database context</param>
        private static void InitializeTypePay(ApplicationDBContext context)
        {
            var types = new TypePay[]
            {
                new TypePay{name=CheckBetType.getJackpotExact(), winRate=0, winLoseCancel=100,
                    description ="Tiene bote. El bote sera para el participante que acierte el resultado exacto. Si nadie acierta el resultado, todos perderán las monedas apostadas."},
                new TypePay{name=CheckBetType.getJackpotCloser(), winRate=0, winLoseCancel=100,
                    description ="Tiene bote. El bote será para el participante que se acerque más al resultado exacto."},
                new TypePay{name=CheckBetType.getSoloExact(), winRate=1.5, winLoseCancel=0.2,
                    description ="Cada participante participa el solo. Las apuestas tienen una cuota. Si el participante no gana perderá sus monedas."},
            };

            types.Where(t => context.TypePay.All(type => type.name != t.name)).ToList().ForEach(tp => context.Add(tp));
        }

        /// <summary>
        /// Initializes the development users
        /// </summary>
        /// <param name="context">The database context</param>
        private static void createDevelopmentUser(ApplicationDBContext context)
        {
            List<User> test_users = new List<User>();
            Role normal = RoleManager.getNormalUser(context);
            Role admin = RoleManager.getAdmin(context);
            string hashedPassword = PasswordHasher.hashPassword("asdfasdf1A");

            for (int i = 0; i < 7; i++)
            {
                test_users.Add(new User
                {
                    email = "u" + i + "@gmail.com",
                    nickname = "u" + i + "_test",
                    password = hashedPassword,
                    tokenValidation = null,
                    role = normal,
                    dateSignUp = new DateTime(2019, 07, i+20, 3*i, 8*i, 00)
                });
            }
            User admin1 = new User
            {
                email = "a@gmail.com",
                nickname = "Javier Gómez",
                password = hashedPassword,
                tokenValidation = null,
                role = RoleManager.getAdmin(context),
                dateSignUp = new DateTime(2019, 06, 20, 17, 00, 00)
            };

            User admin2 = new User
            {
                email = "a2@gmail.com",
                nickname = "Diego Carrillo",
                password = hashedPassword,
                tokenValidation = null,
                role = admin,
                dateSignUp = new DateTime(2019, 06, 20, 17, 20, 00)
            };


            test_users.Where(u => context.User.All(dbUser => dbUser.email != u.email)).ToList().ForEach(newU => context.Add(newU));

            if (context.User.Where(u => u.email == "a@gmail.com").Count() == 0) context.Add(admin1);
            if (context.User.Where(u => u.email == "a2@gmail.com").Count() == 0) context.Add(admin2);
        }

        /// <summary>
        /// Creates some news
        /// </summary>
        /// <param name="context">The database context</param>
        private static void createNews(ApplicationDBContext context)
        {
            string title = "Aviso de los administradores!";
            var news = new New[]
            {
                new New
                {
                    Group = null, User = null, groupid = null, userid = null,
                    title = title,
                    message = "Beta de la aplicación lanzada!!",
                    date = new DateTime(2019, 06, 20)
                },
                new New
                {
                    Group = null, User = null, groupid = null, userid = null,
                    title = title,
                    message = "Hemos añadido un chat grupal. Podreis hablar con vuestros compañeros de grupo.",
                    date = new DateTime(2019, 06, 29)
                },
                new New
                {
                    Group = null, User = null, groupid = null, userid = null,
                    title = title,
                    message = "Hemos añadido una lista de noticias para los grupos y para la página principal. Podreis ver un historial de todo lo que os pasado en VirtualBet.",
                    date = new DateTime(2019, 07, 05)
                },
                new New
                {
                    Group = null, User = null, groupid = null, userid = null,
                    title = title,
                    message = "Hemos añadido notificaciones en tiempo real para que os entereis al momento de cualquier cosa que os suceda a vosotros o en alguno de tus grupos.",
                    date = new DateTime(2019, 07, 11)
                },
                new New
                {
                    Group = null, User = null, groupid = null, userid = null,
                    title = title,
                    message = "Beta de la versión para android lanzada!!",
                    date = new DateTime(2019, 07, 30)
                },
                new New
                {
                    Group = null, User = null, groupid = null, userid = null,
                    title = title,
                    message = "Hemos añadido una sección de ayuda donde se explican las cuestíones más frecuentes.",
                    date = new DateTime(2019, 08, 30)
                }
            };

            news.Where(nn => context.News.All(n => n.message != nn.message)).ToList().ForEach(nws => context.Add(nws));
        }

        /// <summary>
        /// Function to do development stuff with the matchdays
        /// DO NOT USE IN PRODUCTION MODE
        /// </summary>
        /// <param name="context">The database context</param>
        private static void aumentarFechaParapruebas(ApplicationDBContext context)
        {
            //context.MatchDays.ToList().ForEach(md =>
            //{
                //Random rnd = new Random();
                //DateTime n = DateTime.Now;
                //md.date = n.AddDays(-7);
                //md.status = "SCHEDULED";
                //md.status = "FINISHED";
                //md.firstHalfHomeGoals = rnd.Next(3);
                //md.fullTimeHomeGoals = rnd.Next(3);
                //md.fullTimeAwayGoals = rnd.Next(3);
                //md.firstHalfAwayGoals = rnd.Next(3);
                //md.secondHalfAwayGoals = rnd.Next(3);
                //md.secondHalfHomeGoals = rnd.Next(3);
                //context.SaveChanges();
            //});
            //context.SaveChanges();
        }

        //
        // ──────────────────────────────────────────────────────────────────────────────────
        //   :::::: P U B L I C   F U N C T I O N S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────────────
        //

        
        /// <summary>
        /// Initializes the database
        /// </summary>
        /// <param name="context"></param>
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
    }
}
