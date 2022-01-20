using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
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
        private static bool f;

        public async override Task OnConnected()
        {
            messages = await UserService.GetAllMessages();
            messages = new List<MessagesDTO>(messages.OrderBy(ma=> ma.DateTime));
            clients.TryAdd(Context.ConnectionId, Context.User.Identity.GetUserId());
            f = false;
            await base.OnConnected();
        }

        public async Task Send(string name ,string message)
        {
            KeyValuePair<string, string> receiver = clients.FirstOrDefault(k => k.Value == name);
            UserDTO u = await UserService.FindById(receiver.Value);
            UserDTO u1 = await UserService.FindById(Context.User.Identity.GetUserId());
            if(u1.Banned)
                Clients.Caller.addNewMessageToPage("Server", "You Have been banned");
            else if (receiver.Equals(default(KeyValuePair<string, string>)))
                Clients.Caller.addNewMessageToPage("Server", "User offline");
            else
            {
                try
                {
                    AppendChat(receiver, f);
                    f = true;
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
                    Console.WriteLine(DateTime.Now + "Chat");
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

        public void AppendChat(KeyValuePair<string, string> receiver, bool b)
        {
            if (b == false)
            {
                var m = messages.Where(u => (u.Id == Context.User.Identity.GetUserId() && u.ReceiverId == receiver.Value) || (u.Id == receiver.Value && u.ReceiverId == Context.User.Identity.GetUserId())).OrderBy(a => a.DateTime);
                foreach (var mess in m)
                {
                    Clients.Client(receiver.Key).addNewMessageToPage(Context.User.Identity.Name, mess.Text);
                    Clients.Caller.addNewMessageToPage(Context.User.Identity.Name, mess.Text);
                }
            }
        }
    }
}