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
                    bool isHello = isHelloMessage(data);
                    
                    
                    if (data.message.Length > 128) return;
                    if (!isHello && !checkExist(data.group, data.publicUserId, ref group, ref roleUser, dbContext)) return;

                    if(!isHello) updateData(ref data, roleUser);
                    if (!isHello) saveMessage(data, group, roleUser, dbContext);

                    if (isHello) {
                        data.username = "";
                        await Clients.Others.SendAsync(groupSocketId + data.group, data);
                    }
                    else await Clients.All.SendAsync(groupSocketId + data.group, data);
                }
            }
            catch(Exception)
            {
                return;
            }
        }

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
                publicUserid = data.publicUserId,
                role = roleUser,
                message = data.message,
                time = data.time
            });
            dbContext.SaveChanges();
        }

        public bool isHelloMessage(ChatMessage msg)
        {
            return msg.role == "Conexión" && msg.message.Contains("está conectado.");
        }
    }
}
