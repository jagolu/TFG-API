using API.Models;
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
            /*var users = new User[] {
                new User{userID = Guid.NewGuid(), email="asdfasd11", nickname="nick1", password="pass1", img="asdfasdf",role="asdfasd"},
            };

            foreach (User c in users) {
                if(!context.User.Contains(c)) context.User.Add(c);
            }*/
        }

        private static void InitializeRoles(ApplicationDBContext context)
        {
            var roles = new Role[] {
                new Role{name="ADMIN"},
                new Role{name="NORMAL"},
                new Role{name="GROUP_ADMIN"}
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

            InitializeUser(context);
            InitializeRoles(context);

            context.SaveChanges();
        }
    }
}
