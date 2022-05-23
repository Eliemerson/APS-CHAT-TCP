using APS_2022_1.Net.IO;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace APS_2022_1.Net
{
    public class Server
    {
        TcpClient _client;
        public PacketReader PacketReader;

        //Codigo 1 Disparado pelo servidor: Novo usuario
        public event Action connectEvent;
        //Codigo 5 Disparado pelo servidor: Nova Mensagem
        public event Action msgReceivedEvent;
        //Codigo 10 Disparado pelo servidor: Usuario Descontado
        public event Action disonnectEvent;
        public Server()
        {
            //Injeção de dependencia
            _client = new TcpClient();
        }


        //Conecta usuario no servidor
        public void ConnectToServer(string userName)
        {
            if (!_client.Connected)
            {
                _client.Connect("127.0.0.1", 7891);
                PacketReader = new PacketReader(_client.GetStream());

                if (!string.IsNullOrEmpty(userName))
                {
                    var connectPacket = new PacketBuilder();
                    connectPacket.WriteOpCode(0);
                    connectPacket.WriteMessage(userName);
                    _client.Client.Send(connectPacket.GetPacketBytes());
                }

                ReadPackets();

            }
        }

        private void ReadPackets()
        {
            //Apos usuario conectado.
            //Uma TAsk fica em run time, ouvindo algum disparo pelo servidor
            //para entar chamar os eventos abaixo e descrito acima para cada função
            Task.Run(() =>
            {
                while (true)
                {
                    var opcode = PacketReader.ReadByte();
                    switch (opcode)
                    {
                        case 1:
                            connectEvent?.Invoke();
                            break;
                        case 5:
                            msgReceivedEvent?.Invoke();
                            break;
                        case 10:
                            disonnectEvent?.Invoke();
                            break;
                        default:
                            Console.WriteLine("...");
                            break;
                    }
                }
            });
        }


        //Metodo responsavel por mandar mensagem ao servidor
        public void SendMessageToServer(string message)
        { 
            var packetBuilder = new PacketBuilder();
            packetBuilder.WriteOpCode(5);
            packetBuilder.WriteMessage(message);
            _client.Client.Send(packetBuilder.GetPacketBytes());
        }
    }
}
