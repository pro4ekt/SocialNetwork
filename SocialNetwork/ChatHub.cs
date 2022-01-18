using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using BLL.DTO;
using BLL.Interfaces;
using BLL.Services;
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
                IServiceCreator service = new ServiceCreator();
                return service.CreateUserService(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename='|DataDirectory|\TestContext.mdf';Integrated Security=True;Persist Security Info = True");
            }
        }
        private static List<MessagesDTO> messages = new List<MessagesDTO>();
        private static ConcurrentDictionary<string, string> clients = new ConcurrentDictionary<string, string>();

        public async override Task OnConnected()
        {
            messages = await UserService.GetAllMessages();
            clients.TryAdd(Context.ConnectionId, Context.User.Identity.GetUserId());
            await base.OnConnected();
        }

        public async Task Send(string name ,string message)
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