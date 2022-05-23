using ClientServer.Net.IO;
using System.Net.Sockets;

namespace ClientServer
{
    public class Client
    {
        public string UserName { get; set; }
        public Guid UID { get; set; }
        public TcpClient ClientSocket { get; set; }

        PacketReader _packetReader;

        public Client(TcpClient clientSocket)
        {
            ClientSocket = clientSocket;
            UID = Guid.NewGuid();

            _packetReader = new PacketReader(ClientSocket.GetStream());

            var opCode = _packetReader.ReadByte();

            UserName = _packetReader.ReadMessage();
            //Loga no console o evento  disparado
            Console.WriteLine($"[{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}] Cliente foi conectado com username: {UserName}");
            Task.Run(() => Process());
        }

        void Process()
        {
            while (true)
            {
                try
                {

                    //Loga no console o evento  disparado
                    var opcode = _packetReader.ReadByte();
                    switch (opcode)
                    {
                        case 5:
                            var msg = _packetReader.ReadMessage();
                            Console.WriteLine($"[{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}] Mensagem recebida  {msg}");
                            Startup.BroadCastMessage($"[{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}] : [{UserName}]: {msg}");
                            break;

                        default:
                            break;
                    }

                }
                catch (Exception)
                {
                    //Loga no console o evento  disparado e ebcessa a aplicação do cliente
                    Console.WriteLine($"[{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}] {UID.ToString()} Usuario desconectadp!!");
                    Startup.BroadCastDisconnect(UID.ToString());
                    ClientSocket.Close();
                    break;
                }
            }
        }
    }
}
