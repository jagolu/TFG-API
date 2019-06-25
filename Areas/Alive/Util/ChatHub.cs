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
        public ChatHub(IServiceScopeFactory sf)
        {
            scopeFactory = sf;
        }

        public async Task BroadcastChartData(ChatMessage data)
        {
            try
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    ApplicationDBContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                    List<Group> groupExists = dbContext.Group.Where(g => g.name == data.group).ToList();
                    List<User> userExists = dbContext.User.Where(u => u.publicId == data.publicUserId).ToList();
                    if (groupExists.Count() != 1 || userExists.Count() != 1) return;

                    Group group = groupExists.First();
                    User user = userExists.First();
                    var userInGroup = dbContext.UserGroup.Where(ug => ug.groupId == group.id && ug.userId == user.id);
                    if (userInGroup.Count() != 1) return;
                    UserGroup ugCaller = userInGroup.First();
                    dbContext.Entry(ugCaller).Reference("role").Load();
                    if (data.message.Length > 128) return;

                    data.username = user.nickname;
                    data.publicUserId = user.publicId;
                    data.role = ugCaller.role.name;
                    data.time = DateTime.Now;

                    dbContext.Add(new GroupChatMessage
                    {
                        Group = group,
                        username = data.username,
                        publicUserId = data.publicUserId,
                        role = data.role,
                        message = data.message,
                        time = data.time
                    });
                    dbContext.SaveChanges();

                    await Clients.All.SendAsync(data.group, data);
                }
            }
            catch(Exception)
            {
                return;
            }
        }
    }
}
