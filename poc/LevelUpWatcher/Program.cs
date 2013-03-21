using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.Owin.Hosting;
using Owin;

namespace LevelUpWatcher
{
    class Program
    {
        private static FileSystemWatcher watcher;

        static void Main(string[] args)
        {
            string url = "http://localhost:9797";
            string directory = @"C:\Users\mark.thiessen\Documents\visual studio 2012\Projects\LevelUp\BasicWebApp";
            using (WebApplication.Start<Startup>(url))
            {
                Console.WriteLine("Server running on {0}", url);
                ConfigureWatcher(directory);
                Console.WriteLine("Watching directory: {0}", directory);
                Console.ReadLine();
            }
        }

        private static void ConfigureWatcher(string directory)
        {
            watcher = new FileSystemWatcher();
            watcher.Path = directory;
            watcher.IncludeSubdirectories = true;
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
           | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            // Only watch text files.
            watcher.Filter = "*.js";

            watcher.Changed += new FileSystemEventHandler(OnChanged);
            // Begin watching.
            watcher.EnableRaisingEvents = true;

        }
        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("Change detected");
            LevelUpHub.SendMessage("LeveledUp");
        }
    }

    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Turn cross domain on 
            var config = new HubConfiguration { EnableCrossDomain = true };

            // This will map out to http://localhost:8080/signalr by default
            app.MapHubs(config);
        }
    }
    
    public class LevelUpHub : Hub
    {
        

        public LevelUpHub()
        {
           
        }

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
