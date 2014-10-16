using System;
using System.IO;

namespace LeveledUp
{
    public class MessageServer
    {

        public event EventHandler<EventArgs> OnConnectionOpen;
        public event EventHandler<EventArgs> OnConnectionClosed;

        public bool IsRunning { get; private set; }


        public void Start(string url)
        {
            IsRunning = true;
        }

        public void Stop()
        {
            IsRunning = false;
        }


        public void SendNotificationMessage(string message)
        {
            OpenStandardStreamOut(message);

        }

        private static void OpenStandardStreamOut(string stringData)
        {
            String str = "{\"data\": \"" + stringData + "\"}";
            Stream stdout = Console.OpenStandardOutput();

            stdout.WriteByte((byte)str.Length);
            stdout.WriteByte((byte)'\0');
            stdout.WriteByte((byte)'\0');
            stdout.WriteByte((byte)'\0');
            Console.Write(str);
        }
    }
}
