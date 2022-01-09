using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using BLL.DTO;
using BLL.Interfaces;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace SocialNetwork
{
    [HubName("chatHub")]
    public class ChatHub : Hub
    {
        private IUserService UserService
        {
            get
            {
                return HttpContext.Current.GetOwinContext().GetUserManager<IUserService>();
            }
        }
        private static ConcurrentDictionary<string, string> clients = new ConcurrentDictionary<string, string>();

        public override Task OnConnected()
        {
            clients.TryAdd(Context.ConnectionId, Context.User.Identity.GetUserId());
            return base.OnConnected();
        }

        public async Task Send(string name ,string message)
        {
            if (name == "")
            {
                try
                {
                    MessagesDTO messageDTO = new MessagesDTO
                    {
                        MessageId = RandomString(5),
                        Id = Context.User.Identity.GetUserId(),
                        ReceiverId = "All",
                        ReceiverName = "All",
                        DateTime = DateTime.Now,
                        Text = message
                    };
                    await UserService.SaveMessage(messageDTO);
                    Clients.All.addNewMessageToPage(Context.User.Identity.Name, message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            else
            {
                KeyValuePair<string, string> receiver = clients.FirstOrDefault(k => k.Value == name);
                if (receiver.Equals(default(KeyValuePair<string, string>)))
                    Clients.Caller.addNewMessageToPage("Server", "User offline");
                else
                {
                    try
                    {
                        UserDTO u = await UserService.FindById(receiver.Value);
                        MessagesDTO messageDTO = new MessagesDTO
                        {
                            MessageId = RandomString(6),
                            Id = Context.User.Identity.GetUserId(),
                            ReceiverId = receiver.Value,
                            ReceiverName = u.UserName,
                            DateTime = DateTime.Now,
                            Text = message
                        };
                        await UserService.SaveMessage(messageDTO);
                        Clients.Client(receiver.Key).addNewMessageToPage(Context.User.Identity.Name, message);
                        Clients.Caller.addNewMessageToPage(Context.User.Identity.Name, message);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
            }
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var userId = Context.User.Identity.GetUserId();
            clients.TryRemove(Context.ConnectionId, out userId);
            return base.OnDisconnected(stopCalled);
        }

        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}