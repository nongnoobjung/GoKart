using KartExtreme.Data;
using KartExtreme.IO;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace KartExtreme.Net
{
    public static class Server
    {
        private static Acceptor _acceptor;
        private static List<KartClient> _clients;

        public static void Initialize()
        {
            Server._clients = new List<KartClient>();

            Settings.Initialize();

            Database.Test();
            Database.Analyze();

            Server._acceptor = new Acceptor(Settings.GetUShort("Net/Port"));
            Server._acceptor.OnClientAccepted = Server.OnClientAccepted;
            Server._acceptor.Start();
        }

        private static void OnClientAccepted(Socket socket)
        {
            KartClient client = new KartClient(socket);

            Log.Inform("Accepted connection from {0}.", client.Label);

            Server.AddClient(client);
        }

        public static void AddClient(KartClient client)
        {
            Server._clients.Add(client);
        }

        public static void RemoveClient(KartClient client)
        {
            Server._clients.Remove(client);
        }
    }
}
