using API.Areas.Common.Models;
using API.Data;
using API.Data.Models;
using System;

namespace API.Areas.Common.Util
{
    public static class GroupNew
    {
        public static bool launch(User user, Group group, TypeGroupNew type, bool makeUnmake, ApplicationDBContext context)
        {
            bool ret = false;
            if (type == TypeGroupNew.BLOCK_USER) ret = blockUser(user, group, makeUnmake, context);


            return ret;
        }

        private static bool blockUser(User user, Group group, bool makeUnmake, ApplicationDBContext context)
        {
            try
            {
                string title = makeUnmake ? "Has sido bloqueado!" : "Has sido desbloqueado!";
                string message = makeUnmake ? "Uno de los administradores del grupo \"" + group.name + "\" te ha bloqueado. No podras volver a entrar en el grupo hasta que hayas sido desbloqueado." :
                    "Uno de los administradores del grupo \"" + group.name + "\" te ha desbloqueado. Vuelves a tener acceso al grupo.";

                New n = new New
                {
                    Group = group,
                    User = user,
                    title = title,
                    message = message
                };

                context.Add(n);
                context.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
