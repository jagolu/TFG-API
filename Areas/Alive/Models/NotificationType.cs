namespace API.Areas.Alive.Models
{
    public enum NotificationType
    {
        BLOCKED = 100,
        UNBLOCKED = 101,
        MAKE_ADMIN = 200,
        UNMAKE_ADMIN = 201,
        GROUP_REMOVED = 400,
        KICKED_GROUP = 500,
        MAKE_MAKER = 600,
        BAN_GROUP = 700,
        UNBAN_GROUP = 701,
        

        NEW_FOOTBALLBET = 110,
        CANCELLED_FOOTBALLBET = 111,
        PAID_BETS = 120,
        PAID_GROUPS = 130,


        OPEN_DM_FROM_USER = 210,
        OPEN_DM_FROM_ADMIN = 211,
        CLOSE_DM = 220,
        REOPEN_DM = 221,
    }
}
