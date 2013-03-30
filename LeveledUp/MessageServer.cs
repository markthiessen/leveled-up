using System;
using System.Collections.Generic;
using Fleck;

namespace LeveledUp
{
    public class MessageServer : IDisposable
    {
        private WebSocketServer _server;
        private readonly List<IWebSocketConnection> _allSockets;

        public event EventHandler<EventArgs> OnConnectionOpen;
        public event EventHandler<EventArgs> OnConnectionClosed;

        public bool IsRunning { get; private set; }

        public MessageServer()
        {
            _allSockets = new List<IWebSocketConnection>();
        }

        public void Start(string url)
        {
            _server = new WebSocketServer(url);
            _server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    if (OnConnectionOpen != null) OnConnectionOpen(this, EventArgs.Empty);
                    _allSockets.Add(socket);
                };
                socket.OnClose = () =>
                {
                    if (OnConnectionClosed != null) OnConnectionClosed(this, EventArgs.Empty);
                    _allSockets.Remove(socket);
                };

                socket.OnMessage = message => socket.Send(message);
            });
            IsRunning = true;
        }

        public void Stop()
        {
            _server.Dispose();
            _server = null;
            IsRunning = false;
        }


        public void SendNotificationMessage(string message)
        {
            if (_server != null)
                foreach (var socket in _allSockets)
                {
                    socket.Send(message);
                }
        }

        public void Dispose()
        {
            if (_server != null)
                _server.Dispose();
        }
    }
}
