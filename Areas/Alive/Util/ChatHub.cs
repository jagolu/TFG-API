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
        private readonly IServiceScopeFactory scopeFactory;
        private readonly string groupSocketId;
        private static List<ChatIdLog> logUsers = new List<ChatIdLog>();

        public ChatHub(IServiceScopeFactory sf, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            scopeFactory = sf;
            groupSocketId = configuration["socket:chatRoom"];
        }

        public async Task BroadcastChartData(ChatMessage data)
        {
            try
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    ApplicationDBContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                    Group group = new Group();
                    Role roleUser = new Role();
                    
                    if (data.message.Length > 128) return;
                    if (!checkExist(data.group, data.publicUserId, ref group, ref roleUser, dbContext)) return;

                    updateData(ref data, roleUser);
                    saveMessage(data, group, roleUser, dbContext);
                    addId(group.name, data.username);


                    await Clients.All.SendAsync(groupSocketId+data.group, data);
                }
            }
            catch(Exception)
            {
                return;
            }
        }

        private bool checkExist(string group, string publicUserId, ref Group groupRet, ref Role roleRet, ApplicationDBContext dbContext)
        {
            List<UserGroup> uExist = dbContext.UserGroup.Where(ug => ug.Group.name == group && ug.User.publicId == publicUserId).ToList();

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

        private void updateData(ref ChatMessage data, Role role)
        {
            data.role = role.name;
            data.time = DateTime.Now;
        }

        private void saveMessage(ChatMessage data, Group group, Role roleUser, ApplicationDBContext dbContext)
        {
            dbContext.Add(new GroupChatMessage
            {
                Group = group,
                username = data.username,
                publicUserId = data.publicUserId,
                role = roleUser,
                message = data.message,
                time = data.time
            });
            dbContext.SaveChanges();
        }

        public void addId(string groupName, string username)
        {
            List<ChatIdLog> mightLog = logUsers.Where(log => log.groupName == groupName).ToList();
            string connectionId = "";

            try
            {
                connectionId = Context.ConnectionId;
            }catch(Exception e)
            {
                return;
            }

            if(mightLog.Count() != 1)
            {
                List<CoupleUserConnectionId> newIds = new List<CoupleUserConnectionId>();
                newIds.Add(new CoupleUserConnectionId { username = username, connectionid = connectionId});

                logUsers.Add(new ChatIdLog
                {
                    groupName = groupName,
                    users = newIds
                });
            }
            else
            {
                ChatIdLog groupLog = mightLog.First();
                List<CoupleUserConnectionId> posibleUsers = groupLog.users.Where(u => u.username == username).ToList();

                if (posibleUsers.Count() != 1)
                {
                    posibleUsers.Add(new CoupleUserConnectionId
                    {
                        connectionid = connectionId,
                        username = username
                    });
                }
                else
                {
                    posibleUsers.First().connectionid = connectionId;
                }
            }
        }
    }
}
