using API.Areas.Alive.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace API.Areas.Alive.Util
{
    public static class KickChatNotification
    {
        //
        // ──────────────────────────────────────────────────────────────────────
        //   :::::: C L A S S   V A R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────
        //

        /// <value>The id of the chat socket</value>
        private static string _groupChatSocketId;

        /// <value>The id of the kick chat</value>
        private static string _kickChatGuid;


        //
        // ──────────────────────────────────────────────────────────────────────────
        //   :::::: C O N S T R U C T O R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Initializes the private vars of the class
        /// </summary>
        /// <param name="config">The configuration of the application</param>
        public static void Initialize(IConfiguration config)
        {
            _groupChatSocketId = config["socket:chatRoom"];
            _kickChatGuid = config["socket:kickChatKey"];
        }

        
        //
        // ──────────────────────────────────────────────────────────────────────────────────
        //   :::::: P U B L I C   F U N C T I O N S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Sends the kick notification to a user
        /// </summary>
        /// <param name="groupName">The name of the group</param>
        /// <param name="publicUserId">The public id of the user to kick</param>
        /// <param name="chatHub">The hub of the chat</param>
        public static async Task sendKickMessageAsync(string groupName, string publicUserId, IHubContext<ChatHub> chatHub)
        {
            try
            {
                await chatHub.Clients.All.SendAsync(_groupChatSocketId + groupName,
                    new ChatMessage
                    {
                        group = "",
                        username = "",
                        publicUserId = publicUserId,
                        role = "",
                        message = _kickChatGuid,
                        time = DateTime.Now
                    }
                );
            }
            catch(Exception)
            {

            }
        }
    }
}
