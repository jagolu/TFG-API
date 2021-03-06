﻿namespace API.Areas.Home.Models
{
    /// <summary>
    /// The type of a new
    /// </summary>
    public enum TypeGroupNew
    {
        CREATE_GROUP_GROUP = 1001,
        CREATE_GROUP_USER = 1002,
        BLOCK_USER_USER = 2001,
        BLOCK_USER_GROUP = 2002,
        JOIN_LEFT_GROUP = 3001,
        JOIN_LEFT_USER = 3002,
        MAKE_ADMIN_USER = 4001,
        MAKE_ADMIN_GROUP = 4002,
        MAKE_PRIVATE = 5001,
        REMOVE_GROUP = 6001,
        KICK_USER_USER = 7001,
        KICK_USER_GROUP = 7002,
        MAKE_MAKER_GROUP = 8001,
        MAKE_MAKER_USER = 8002,
        BAN_GROUP = 9001,


        LAUNCH_FOOTBALLBET_GROUP = 1101,
        LAUNCH_FOOTBALLBET_USER = 1102,
        PAID_BETS_USER = 1201,
        PAID_BETS_GROUP = 1202,
        FOOTBALLBET_CANCELLED_GROUP = 1301,
        FOOTBALLBET_CANCELLED_USER = 1302,


        PAID_PLAYERS_USER = 1401,
        PAID_PLAYERS_GROUPS = 1402,


        CHANGE_WEEKLYPAY_USER = 1501,
        CHANGE_WEEKLYPAY_GROUP = 1502,


        WELCOME = 2101,
        WELCOMEBACK = 2102
    }
}
