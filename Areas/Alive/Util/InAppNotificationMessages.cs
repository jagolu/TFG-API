using API.Areas.Alive.Models;

namespace API.Areas.Alive.Util
{
    public static class InAppNotificationMessages
    {
        /// <summary>
        /// Get the text for a notification
        /// </summary>
        /// <param name="type">The type of the notification</param>
        /// <param name="target">The target of the notification</param>
        /// <returns></returns>
        public static string getMessage(NotificationType type, string target)
        {
            string message;

            switch (type)
            {
                case NotificationType.BLOCKED:
                    message = "Te han bloqueado en " + target;
                    break;
                case NotificationType.UNBLOCKED:
                    message = "Te han desbloqueado en " + target;
                    break;
                case NotificationType.MAKE_ADMIN:
                    message = "Eres administrador de " + target;
                    break;
                case NotificationType.UNMAKE_ADMIN:
                    message = "Ya no eres administrador de " + target;
                    break;
                case NotificationType.GROUP_REMOVED:
                    message = target + " ha sido eliminado";
                    break;
                case NotificationType.KICKED_GROUP:
                    message = "Te han echado de " + target;
                    break;
                case NotificationType.MAKE_MAKER:
                    message = "Eres el MAKER de " + target;
                    break;
                case NotificationType.BAN_GROUP:
                    message = "El grupo " + target + " ha sido bloqueado";
                    break;
                case NotificationType.UNBAN_GROUP:
                    message = "El grupo " + target + " ha sido desbloqueado";
                    break;
                case NotificationType.NEW_FOOTBALLBET:
                    message = "Nueva apuesta en "+target;
                    break;
                case NotificationType.CANCELLED_FOOTBALLBET:
                    message = "Apuesta cancelada en " + target;
                    break;
                case NotificationType.PAID_BETS:
                    message = "Apuestas pagadas en el grupo " + target;
                    break;
                case NotificationType.PAID_GROUPS:
                    message = "Dinero semanal pagado en el grupo " + target;
                    break;
                case NotificationType.OPEN_DM_FROM_USER:
                    message = target+" ha abierto una conversación contigo.";
                    break;
                case NotificationType.OPEN_DM_FROM_ADMIN:
                    message = "Un administrador ha abierto una conversación contigo.";
                    break;
                case NotificationType.CLOSE_DM:
                    message = "Un administrador ha cerrado una conversación contigo";
                    break;
                case NotificationType.REOPEN_DM:
                    message = "Un administrador ha abierto de nuevo una conversación contigo.";
                    break;
                case NotificationType.RECEIVED_DM:
                    message = "Has recibido un mensaje en una conversación directa.";
                    break;
                default:
                    message = "";
                    break;
            }
          


            return message;
        }
    }
}
