using API.Data.Models;
using API.Util;
using System;
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

        public static void Initialize(ApplicationDBContext context)
        {
            context.Database.EnsureCreated();

            InitializeRoles(context);

            context.SaveChanges();

            createDevelopmentUser(context); //Test users

            context.SaveChanges();
        }

        private static void createDevelopmentUser(ApplicationDBContext context)
        {
            User u1 = new User
            {
                email = "u1@gmail.com",
                nickname = "u1_test",
                password = PasswordHasher.hashPassword("asdfasdf1A"),
                tokenValidation = null,
                role = context.Role.Where(r => r.name == "NORMAL_USER").First()
            };
            User u2 = new User
            {
                email = "u2@gmail.com",
                nickname = "u2_test",
                password = PasswordHasher.hashPassword("asdfasdf1A"),
                tokenValidation = null,
                role = context.Role.Where(r => r.name == "NORMAL_USER").First()
            };
            User u3 = new User
            {
                email = "u3@gmail.com",
                nickname = "u3_test",
                password = PasswordHasher.hashPassword("asdfasdf1A"),
                tokenValidation = null,
                role = context.Role.Where(r => r.name == "NORMAL_USER").First()
            };
            User u4 = new User
            {
                email = "u4@gmail.com",
                nickname = "u4_test",
                password = PasswordHasher.hashPassword("asdfasdf1A"),
                tokenValidation = null,
                role = context.Role.Where(r => r.name == "NORMAL_USER").First()
            };
            User admin = new User
            {
                email = "a@gmail.com",
                nickname = "a_ADMIN",
                password = PasswordHasher.hashPassword("asdfasdf1A"),
                tokenValidation = null,
                role = context.Role.Where(r => r.name == "ADMIN").First()
            };


            if (context.User.Where(u => u.email == "u1@gmail.com").Count() == 0)
            {
                context.Add(u1);
                context.Limitations.Add(new Limitations { User = u1 });
            }
            if (context.User.Where(u => u.email == "u2@gmail.com").Count() == 0)
            {
                context.Add(u2);
                context.Limitations.Add(new Limitations { User = u2 });
            }
            if (context.User.Where(u => u.email == "u3@gmail.com").Count() == 0)
            {
                context.Add(u3);
                context.Limitations.Add(new Limitations { User = u3 });
            }
            if (context.User.Where(u => u.email == "u4@gmail.com").Count() == 0)
            {
                context.Add(u4);
                context.Limitations.Add(new Limitations { User = u4 });
            }
            if (context.User.Where(u => u.email == "a@gmail.com").Count() == 0)
            {
                context.Add(admin);
                context.Limitations.Add(new Limitations { User = admin });
            }
        }
    }
}
