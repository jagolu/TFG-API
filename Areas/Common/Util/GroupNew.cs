using API.Areas.Common.Models;
using API.Data;
using API.Data.Models;
using System;

namespace API.Areas.Common.Util
{
    public static class GroupNew
    {
        public static void launch(User user, Group group, TypeGroupNew type, bool makeUnmake, ApplicationDBContext context)
        {
            New whatsGoingOn = new New();

            if (type == TypeGroupNew.BLOCK_USER) whatsGoingOn = blockUser(user, group, makeUnmake);
            else if (type == TypeGroupNew.JOIN_LEFT) whatsGoingOn = blockUser(user, group, makeUnmake);

            try
            {
                context.Add(whatsGoingOn);
                context.SaveChanges();
            }
            catch (Exception){}
        }

        private static New blockUser(User user, Group group, bool makeUnmake)
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

            return n;
        }

        private static New join_left(User user, Group group, bool joinLeft)
        {
            string title = joinLeft ? "Nuevo miembro en \""+group.name+"\"" : "Un miembro ha dejado \""+group.name+"\"";
            string message = joinLeft ? user.nickname+" se ha unido al grupo." : user.nickname+" ha abandonado el grupo.";

            New n = new New
            {
                Group = group,
                title = title,
                message = message
            };

            return n;
        }
    }
}
