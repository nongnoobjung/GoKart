using KartExtreme.IO;
using System;
using System.Net;
using System.Net.Sockets;

namespace KartExtreme.Net
{
    public sealed class Acceptor
    {
        public const int Backlog = 25;

        public ushort Port { get; private set; }

        private readonly TcpListener m_listener;

        private bool m_disposed;

        public Action<Socket> OnClientAccepted;

        public Acceptor(ushort port) : this(IPAddress.Any, port) { }

        public Acceptor(IPAddress ip, ushort port)
        {
            Port = port;
            m_listener = new TcpListener(IPAddress.Any, port);
            OnClientAccepted = null;
            m_disposed = false;
        }

        public void Start()
        {
            m_listener.Start(Backlog);
            m_listener.BeginAcceptSocket(EndAccept, null);

            Log.Inform("Initialized clients listener on {0}.", this.Port);
        }

        public void Stop()
        {
            Dispose();
        }

        private void EndAccept(IAsyncResult iar)
        {
            if (m_disposed) { return; }

            Socket client = m_listener.EndAcceptSocket(iar);

            if (OnClientAccepted != null)
                OnClientAccepted(client);

            if (!m_disposed)
                m_listener.BeginAcceptSocket(EndAccept, null);
        }

        public void Dispose()
        {
            if (!m_disposed)
            {
                m_disposed = true;
                m_listener.Server.Close();
            }
        }
    }
}
