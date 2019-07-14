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

            if (type == TypeGroupNew.BLOCK_USER_USER) whatsGoingOn = blockUser_user(user, group, makeUnmake);
            else if (type == TypeGroupNew.BLOCK_USER_GROUP) whatsGoingOn = blockUser_group(user, group, makeUnmake);
            else if (type == TypeGroupNew.JOIN_LEFT_GROUP) whatsGoingOn = join_left_group(user, group, makeUnmake);
            else if (type == TypeGroupNew.JOIN_LEFT_USER) whatsGoingOn = join_left_user(user, group, makeUnmake);
            else if (type == TypeGroupNew.MAKE_ADMIN_USER) whatsGoingOn = makeAdmin_user(user, group, makeUnmake);
            else if (type == TypeGroupNew.MAKE_ADMIN_GROUP) whatsGoingOn = makeAdmin_group(user, group, makeUnmake);
            else if (type == TypeGroupNew.MAKE_PRIVATE) whatsGoingOn = makePrivate(group, makeUnmake);
            else if (type == TypeGroupNew.REMOVE_GROUP) whatsGoingOn = removeGroup(user, group, makeUnmake);
            else if (type == TypeGroupNew.KICK_USER_USER) whatsGoingOn = kickUser_user(user, group);
            else if (type == TypeGroupNew.KICK_USER_GROUP) whatsGoingOn = kickUser_group(user, group);
            else if (type == TypeGroupNew.CREATE_GROUP_USER) whatsGoingOn = createGroup_user(user, group);
            else if (type == TypeGroupNew.CREATE_GROUP_GROUP) whatsGoingOn = createGroup_group(user, group);
            else if (type == TypeGroupNew.MAKE_MAKER_GROUP) whatsGoingOn = makeMaker_group(user, group, makeUnmake);
            else if (type == TypeGroupNew.MAKE_MAKER_USER) whatsGoingOn = makeMaker_user(user, group, makeUnmake);
            else if (type == TypeGroupNew.BAN_GROUP) whatsGoingOn = ban_group(user, group, makeUnmake);

            else if (type == TypeGroupNew.LAUNCH_FOOTBALLBET_USER) whatsGoingOn = launchFootballBet_user(user, group, makeUnmake);
            else if (type == TypeGroupNew.LAUNCH_FOOTBALLBET_GROUP) whatsGoingOn = launchFootballBet_group(group);
            else if (type == TypeGroupNew.PAID_BETS_USER) whatsGoingOn = pay_bets_user(user, group);
            else if (type == TypeGroupNew.PAID_BETS_GROUP) whatsGoingOn = pay_bets_group(group);


            else if (type == TypeGroupNew.PAID_PLAYERS_USER) whatsGoingOn = pay_players_user(user, group);
            else if (type == TypeGroupNew.PAID_PLAYERS_GROUPS) whatsGoingOn = pay_players_group(group);

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

        private static New join_left_group(User user, Group group, bool joinLeft)
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

        private static New join_left_user(User user, Group group, bool joinLeft)
        {
            string title = joinLeft ? "Te has unido al grupo \""+group.name+"\"" : "Te has unido al grupo \""+group.name+"\"";
            string message = "";

            New n = new New
            {
                User = user,
                title = title,
                message = message
            };

            return n;
        }

        private static New makeAdmin_user(User user, Group group, bool makeUnmake)
        {
            string title = makeUnmake ? "Has sido elegido administrador!" : "Ya no eres administrador";
            string message = makeUnmake ? "El creador del grupo  \"" + group.name + "\" te nombrado administrador. Ahora tienes más privilegios en el grupo. No traiciones su confianza!" :
                "El creador del grupo \"" + group.name + "\" te ha quitado los privilegios de administrador. Vuelves a ser un usuario normal en el grupo.";

            New n = new New
            {
                User = user,
                title = title,
                message = message
            };

            return n;
        }

        private static New makeAdmin_group(User user, Group group, bool makeUnmake)
        {
            string title = makeUnmake ? "Hay un nuevo administrador en el grupo" : "Un administrador ha dejado de serlo";
            string message = makeUnmake ? "El creador del grupo ha nombrado como administrador a \""+user.nickname+"\"" :
                "El creador del grupo ha relegado del cargo de administrador a \"" + user.nickname + "\"";

            New n = new New
            {
                Group = group,
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

        private static New kickUser_user(User user, Group group)
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

        private static New kickUser_group(User user, Group group)
        {
            string title = "Se ha echado a un miembro del grupo!";
            string message = "Uno de los administradores del grupo ha echado a \"" + user.nickname + "\".";

            New n = new New
            {
                Group = group,
                title = title,
                message = message
            };

            return n;
        }

        private static New createGroup_user(User user, Group group)
        {
            string title = "Has creado un grupo";
            string message = "Has creado el grupo \"" + group.name + "\".";

            New n = new New
            {
                User = user,
                title = title,
                message = message
            };

            return n;
        }

        private static New createGroup_group(User user, Group group)
        {
            string title = "El grupo fue creado!";
            string message = "El grupo fue creado por \"" + user.nickname + "\" el dia " + 
                                group.dateCreated.Day + "/"+group.dateCreated.Month+"/"+group.dateCreated.Year+".";

            New n = new New
            {
                Group = group,
                title = title,
                message = message
            };

            return n;
        }

        private static New makeMaker_group(User user, Group group, bool leave)
        {
            string bitM = leave ? "se ha marchado" : "ha sido baneado";
            string title = "El creador del grupo ha cambiado";
            string message = "Debido a que el creador " +  bitM + ", se ha elegido automáticamente el nuevo creador del mismo."+
                              "El elegido es \""+user.nickname+"\".";

            New n = new New
            {
                Group = group,
                title = title,
                message = message
            };

            return n;
        }

        private static New makeMaker_user(User user, Group group, bool leave)
        {
            string bitM = leave ? "se ha marchado" : "ha sido baneado";
            string title = "Se te ha elegido como creador de un grupo!!";
            string message = "Debido a que el creador del grupo \"" + group.name + "\" " + bitM +
                            ", se te ha elegido automáticamente como nuevo creador del mismo.";

            New n = new New
            {
                User = user,
                title = title,
                message = message
            };

            return n;
        }

        private static New ban_group(User user, Group group, bool ban)
        {
            string title = ban ? "Un grupo ha sido bloqueado" : "Un grupo ha sido desbloqueado!";
            string message = ban ? "Uno de los administradores de la plataforma ha bloqueado el grupo \""+group.name+"\". "+
                              "No podrás acceder al grupo hasta que no haya sido desbloqueado" :
                            "El grupo \"" + group.name + "\" ha sido desbloqueado. Puedes volver a acceder al mismo";

            New n = new New
            {
                User = user,
                title = title,
                message = message
            };

            return n;
        }

        private static New launchFootballBet_user(User user, Group group, bool isLauncher)
        {
            string title = !isLauncher ? "Se ha lanzado una nueva apuesta en uno de tus grupos" :
                            "Has lanzado una apuesta";
            string message = !isLauncher ? "Se ha lanzado una nueva apuesta en el grupo \"" + group.name + "\"":
                                "Has lanzado una nueva apuesta en el grupo \"" + group.name + "\"";

            New n = new New
            {
                User = user,
                title = title,
                message = message
            };

            return n;
        }

        private static New launchFootballBet_group(Group group)
        {
            string title = "Se ha lanzado una nueva apuesta!";
            string message = "";

            New n = new New
            {
                Group = group,
                title = title,
                message = message
            };

            return n;
        }

        private static New pay_bets_group(Group group)
        {
            string title = "Se han pagado los resultados de las apuestas!!";
            string message = "";

            New n = new New
            {
                Group = group,
                title = title,
                message = message
            };

            return n;
        }

        private static New pay_bets_user(User user, Group group)
        {
            string title = "Se han pagado los resultados de las apuestas";
            string message = "Se han pagado los resultados de las apuestas del grupo \""+group.name+"\"";

            New n = new New
            {
                User = user,
                title = title,
                message = message
            };

            return n;
        }

        private static New pay_players_group(Group group)
        {
            string title = "Se os ha pagado las monedas semanales";
            string message = "Habeis cobrado las "+group.weeklyPay+ " monedas semanales.";

            New n = new New
            {
                Group = group,
                title = title,
                message = message
            };

            return n;
        }

        private static New pay_players_user(User user, Group group)
        {
            string title = "Has cobrado las monedas semanales";
            string message = "Has cobrado las "+group.weeklyPay+ " monedas semanales del grupo \""+group.name+"\".";

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
