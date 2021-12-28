using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using Microsoft.AspNet.SignalR.Hosting;

namespace SocialNetwork
{
    public class ChatHub : Hub
    {
        public void Send(string message)
        {
            Clients.Caller.message("You~You: " + message);
            Clients.Others.message("others~" + Context.User.Identity.Name + ": " + message);
        }
    }
}