using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNet.SignalR;

namespace LeveledUp
{
    public class LevelUpHub : Hub
    {
        internal static void SendMessage(string message)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<LevelUpHub>();
            context.Clients.All.broadcastMessage(message);
        }

        public void Send(string message)
        {
            Clients.All.addMessage(message);
        }
    }
}
