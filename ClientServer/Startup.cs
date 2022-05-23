using ClientServer.Net.IO;
using System.Net;
using System.Net.Sockets;

namespace ClientServer
{
    public static class Startup
    {
        static List<Client> _user;
        static TcpListener _listener;
        public static void SetServer()
        {
            _user = new List<Client>();
            _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 7891);
            _listener.Start();

            while (true)
            {
                var client = new Client(_listener.AcceptTcpClient());
                _user.Add(client);
                BroadCastConnection();
            }
        }

        public static void BroadCastConnection()
        {
            foreach (var user in _user)
            {
                foreach (var usr in _user)
                {
                    var broadCastPacket = new PacketBuilder();
                    //Codigo 1 : Codigo de uma nova conexão de usuario ao servidor
                    broadCastPacket.WriteOpCode(1);
                    broadCastPacket.WriteMessage(usr.UserName);
                    broadCastPacket.WriteMessage(usr.UID.ToString());
                    user.ClientSocket.Client.Send(broadCastPacket.GetPacketBytes());
                }
            }
        }

        public static void BroadCastMessage(string message)
        {
            foreach (var user in _user)
            {
                var msgPacket = new PacketBuilder();
                //Codigo 5 : Uma nova mensagem foi disparada
                msgPacket.WriteOpCode(5);
                msgPacket.WriteMessage(message);
                user.ClientSocket.Client.Send(msgPacket.GetPacketBytes());

            }
        }

        public static void BroadCastDisconnect(string uid)
        {
            var disconnectedUser = _user.Where(x => x.UID.ToString() == uid).FirstOrDefault();
            _user.Remove(disconnectedUser);

            foreach (var user in _user)
            {
                var msgPacket = new PacketBuilder();
                //Codigo 10 : Uma interface (usuario) foi desconectado
                msgPacket.WriteOpCode(10);
                msgPacket.WriteMessage(uid);
                user.ClientSocket.Client.Send(msgPacket.GetPacketBytes());

            }

            BroadCastMessage($" Usuario {disconnectedUser?.UserName} desconectado as [{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}]!!");

        }
    }
}
