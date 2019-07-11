﻿namespace API.Areas.Home.Models
{
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

        LAUNCH_FOOTBALLBET_GROUP = 1101,
        LAUNCH_FOOTBALLBET_USER = 1102,
        PAID_BETS_USER = 1201,
        PAID_BETS_GROUP = 1202
    }
}
