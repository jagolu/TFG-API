using API.Areas.Home.Models;
using API.Data;
using API.Data.Models;
using System;

namespace API.Areas.Home.Util
{
    public static class GroupNew
    {
        public static void launch(User user, Group group, TypeGroupNew type, bool makeUnmake, ApplicationDBContext context)
        {
            New whatsGoingOn = new New();

            else if (type == TypeGroupNew.JOIN_LEFT) whatsGoingOn = join_left(user, group, makeUnmake);
            if (type == TypeGroupNew.BLOCK_USER_USER) whatsGoingOn = blockUser_user(user, group, makeUnmake);
            else if (type == TypeGroupNew.BLOCK_USER_GROUP) whatsGoingOn = blockUser_group(user, group, makeUnmake);
            else if (type == TypeGroupNew.MAKE_ADMIN) whatsGoingOn = makeAdmin(user, group, makeUnmake);
            else if (type == TypeGroupNew.MAKE_PRIVATE) whatsGoingOn = makePrivate(group, makeUnmake);
            else if (type == TypeGroupNew.REMOVE_GROUP) whatsGoingOn = removeGroup(user, group, makeUnmake);
            else if (type == TypeGroupNew.KICK_USER) whatsGoingOn = kickUser(user, group);

            try
            {
                context.Add(whatsGoingOn);
                context.SaveChanges();
            }
            catch (Exception){}
        }

        private static New blockUser_group(User user, Group group, bool makeUnmake)
        {
            string title = makeUnmake ? "Un miembro ha sido bloqueado!" : "Un miembro ha sido desbloqueado desbloqueado!";
            string message = makeUnmake ? "Uno de los administradores del grupo \"" + group.name + "\" ha bloqueado a \""+user.nickname+"\"." :
                "Uno de los administradores del grupo \"" + group.name + "\" ha desbloqueado a \"" + user.nickname + "\".";

            New n = new New
            {
                Group = group,
                title = title,
                message = message
            };

            return n;
        }

        private static New blockUser_user(User user, Group group, bool makeUnmake)
        {
            string title = makeUnmake ? "Has sido bloqueado!" : "Has sido desbloqueado!";
            string message = makeUnmake ? "Uno de los administradores del grupo \"" + group.name + "\" te ha bloqueado. No podras volver a entrar en el grupo hasta que hayas sido desbloqueado." :
                "Uno de los administradores del grupo \"" + group.name + "\" te ha desbloqueado. Vuelves a tener acceso al grupo.";

            New n = new New
            {
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

        private static New makeAdmin(User user, Group group, bool makeUnmake)
        {
            string title = makeUnmake ? "Has sido elegido administrador!" : "Ya no eres administrador";
            string message = makeUnmake ? "El creador del grupo  \"" + group.name + "\" te nombrado administrador. Ahora tienes más privilegios en el grupo. No traiciones su confianza!" :
                "El creador del grupo \"" + group.name + "\" te ha quitado los privilegios de administrador. Vuelves a ser un usuario normal en el grupo.";

            New n = new New
            {
                Group = group,
                User = user,
                title = title,
                message = message
            };

            return n;
        }

        private static New makePrivate(Group group, bool makeUnmake)
        {
            string title = makeUnmake ? "Un grupo se ha vuelto privado" : "Un grupo se ha vuelto publico";
            string message = makeUnmake ? "El creador del grupo  \"" + group.name + "\" ha establecido una contraseña al mismo. A partir de ahora todo nuevo usuario deberá introducir una nueva contraseña para poder unirse." :
                "El creador del grupo \"" + group.name + "\" ha quitado la contraseña de acceso al grupo. Cualquiera puede unirse libremente.";

            New n = new New
            {
                Group = group,
                title = title,
                message = message
            };

            return n;
        }

        private static New removeGroup(User user, Group group, bool isMaker)
        {
            string title = isMaker ? "Has eliminado un grupo" : "El creado del grupo lo ha eliminado!";
            string message = isMaker ? "Has eliminado el grupo  \"" + group.name + "\". Todos los miembros han salido del mismo y sus datos han sido borrados." :
                "El creador del grupo \"" + group.name + "\" lo ha eliminado. Por lo tanto se te ha echado y se han eliminado todos los datos relacionados con él..";

            New n = new New
            {
                User = user,
                title = title,
                message = message
            };

            return n;
        }

        private static New kickUser(User user, Group group)
        {
            string title = "Te han echado de un grupo.";
            string message = "Uno de los administradores del grupo \"" + group.name + "\" te ha echado.";

            New n = new New
            {
                User = user,
                title = title,
                message = message
            };

            return n;
        }
    }
}
