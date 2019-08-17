using API.Areas.Alive.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace API.Areas.Alive.Util
{
    public static class KickChatNotification
    {
        private static string _groupChatSocketId;
        private static string _kickChatGuid;

        public static void Initialize(IConfiguration config)
        {
            _groupChatSocketId = config["socket:chatRoom"];
            _kickChatGuid = config["socket:kickChatKey"];
        }

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
