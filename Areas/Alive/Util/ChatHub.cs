using API.Areas.Alive.Models;
using API.Data;
using API.Data.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Areas.Alive.Util
{
    public class ChatHub : Hub
    {
        //
        // ──────────────────────────────────────────────────────────────────────
        //   :::::: C L A S S   V A R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────
        //

        /// <value>The scope factory to get the database context</value>
        private readonly IServiceScopeFactory _scopeFactory;

        /// <value>The id for all the group socket</value>
        private readonly string _groupSocketId;


        //
        // ──────────────────────────────────────────────────────────────────────────
        //   :::::: C O N S T R U C T O R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="sf">The scope factory</param>
        /// <param name="configuration">The configuration of the application</param>
        public ChatHub(IServiceScopeFactory sf, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _scopeFactory = sf;
            _groupSocketId = configuration["socket:chatRoom"];
        }


        //
        // ──────────────────────────────────────────────────────────────────────────────────
        //   :::::: P U B L I C   F U N C T I O N S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Receives a message and send it to the group chat
        /// </summary>
        /// <param name="data">The info of the message</param>
        /// See <see cref="Areas.Alive.Models.ChatMessage"/> to see the param structure
        public async Task BroadcastChartData(ChatMessage data)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    ApplicationDBContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                    Group group = new Group();
                    Role roleUser = new Role();
                    bool isHello = isHelloMessage(data);
                    
                    
                    if (data.message.Length > 120) return;
                    if (!isHello && !checkExist(data.group, data.publicUserId, ref group, ref roleUser, dbContext)) return;

                    if(!isHello) updateData(ref data, roleUser);
                    if (!isHello) saveMessage(data, group, roleUser, dbContext);

                    if (isHello) {
                        data.username = "";
                        await Clients.Others.SendAsync(_groupSocketId + data.group, data);
                    }
                    else await Clients.All.SendAsync(_groupSocketId + data.group, data);
                }
            }
            catch(Exception)
            {
                return;
            }
        }


        //
        // ────────────────────────────────────────────────────────────────────────────────────
        //   :::::: P R I V A T E   F U N C T I O N S : :  :   :    :     :        :          :
        // ────────────────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Check if the user is joined in a group
        /// </summary>
        /// <param name="group">The name of the group</param>
        /// <param name="publicUserId">The public id of the user who sent the messsage</param>
        /// <param name="groupRet">The group object (ref object)</param>
        /// <param name="roleRet">The role of the user in the group</param>
        /// <param name="dbContext">The context of the database</param>
        /// <returns>True if the group has joined in the group, false otherwise</returns>
        private bool checkExist(string group, string publicUserId, ref Group groupRet, ref Role roleRet, ApplicationDBContext dbContext)
        {
            List<UserGroup> uExist = dbContext.UserGroup.Where(ug => ug.Group.name == group && ug.User.publicid == publicUserId && !ug.blocked).ToList();

            if (uExist.Count() != 1)
            {
                return false;
            }

            UserGroup ugFound = uExist.First();
            dbContext.Entry(ugFound).Reference("role").Load();
            dbContext.Entry(ugFound).Reference("Group").Load();

            roleRet = ugFound.role;
            groupRet = ugFound.Group;

            return true;
        }

        /// <summary>
        /// Update the data of the message
        /// </summary>
        /// <param name="data">The message</param>
        /// <param name="role">The role of the user</param>
        private void updateData(ref ChatMessage data, Role role)
        {
            data.role = role.name;
            data.time = DateTime.Now;
        }

        /// <summary>
        /// Saves the message in the database
        /// </summary>
        /// <param name="data">The message</param>
        /// <param name="group">The group of the chat</param>
        /// <param name="roleUser">The role of the user</param>
        /// <param name="dbContext">The context of the database</param>
        private void saveMessage(ChatMessage data, Group group, Role roleUser, ApplicationDBContext dbContext)
        {
            dbContext.Add(new GroupChatMessage
            {
                Group = group,
                username = data.username,
                publicUserid = data.publicUserId,
                role = roleUser,
                message = data.message,
                time = data.time
            });
            dbContext.SaveChanges();
        }

        /// <summary>
        /// Checks if the received message is a "hello message"
        /// </summary>
        /// <param name="msg">The message to check</param>
        /// <returns>True if the message is a "hello message", false otherwise</returns>
        private bool isHelloMessage(ChatMessage msg)
        {
            return msg.role == "Conexión" && msg.message.Contains("está conectado.");
        }
    }
}
