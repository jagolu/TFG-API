using API.Models;
using API.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data
{
    public class DBInitializer
    {
        private static void InitializeUser(ApplicationDBContext context)
        {

            User admin = new User {
                email = "a@gmail.com",
                nickname = "a_ADMIN",
                password = PasswordHasher.hashPassword("asdfasdf1A"),
                tokenValidation = null,
                role = context.Role.Where(r => r.name == "ADMIN").First()
            };

            if(context.User.Where(u=> u.email == "a@gmail.com").Count() == 0)context.Add(admin);
        }

        private static void InitializeRoles(ApplicationDBContext context)
        {
            var roles = new Role[] {
                new Role{name="ADMIN"},
                new Role{name="NORMAL_USER"},
                new Role{name="GROUP_ADMIN"},
                new Role{name="GROUP_SUBADMIN"},
                new Role{name="GROUP_NORMAL"}
            };

            var rolesToAdd = from r in roles where 
                             (context.Role.Where(
                                 databaseRole => databaseRole.name == r.name).Count() == 0
                             ) select r;

            foreach (Role r in rolesToAdd) context.Role.Add(r);
        }

        public static void Initialize(ApplicationDBContext context)
        {
            context.Database.EnsureCreated();

            InitializeRoles(context);

            context.SaveChanges();

            InitializeUser(context);

            context.SaveChanges();
        }
    }
}
