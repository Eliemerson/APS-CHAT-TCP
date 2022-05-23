using APS_2022_1.MVVM.Core;
using APS_2022_1.MVVM.Model;
using APS_2022_1.Net;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace APS_2022_1.MVVM.ViewModel
{
    public class MainViewModel
    {
        //Variavel que contem todos usuarios conectado
        public ObservableCollection<UserModel> Users { get; set; }
        //Variavel que contem todas as mensagens enviadas
        public ObservableCollection<string> Messages { get; set; }
        //Variavel que pega o click pelo usuario na UI
        public RelyCommand ConnectToServerCommand { get; set; }
        //Variavel que pega o click pelo usuario na UI
        public RelyCommand SendMessageCommand { get; set; }

        public string UserName { get; set; }
        public string Message { get; set; }

        private Server _server;


        public MainViewModel()
        {
            //Construtor principal, faz a inicialização, criando instancias e conetando o usuario ao servidor principal
            Users = new ObservableCollection<UserModel>();
            Messages = new ObservableCollection<string>();
            _server = new Server();
            _server.connectEvent += UserConnected;
            _server.msgReceivedEvent += MessageReceived;
            _server.disonnectEvent += UserDesconnect;
            ConnectToServerCommand = new RelyCommand(e => _server.ConnectToServer(UserName), o => !string.IsNullOrEmpty(UserName));
            SendMessageCommand = new RelyCommand(e => _server.SendMessageToServer(Message), o => !string.IsNullOrEmpty(Message));
        }

        // Metodo que pega evento de conectar disparado pelo usuario.
        private void UserConnected()
        {
            var user = new UserModel
            {
                UserName = _server.PacketReader.ReadMessage(),
                UID = _server.PacketReader.ReadMessage()
            };
            // valida de uma UID ja esta inserida no servidor.
            if (!Users.Any(x => x.UID == user.UID))
            {
                Application.Current.Dispatcher.Invoke(() => Users.Add(user));
            }
        }

        //Evento que detecta uma mensagem disparada pelo servidor
        private void MessageReceived()
        {
            var msg = _server.PacketReader.ReadMessage();
            Application.Current.Dispatcher.Invoke(() => Messages.Add(msg));
        }

        //Evento que recebe o click do usuario a desconectar um client e informa as demais interface conectada
        private void UserDesconnect()
        {
            var uid = _server.PacketReader.ReadMessage();
            var user = Users.Where(x => x.UID == uid).FirstOrDefault();
            Application.Current.Dispatcher.Invoke(() => Users.Remove(user));

        }
    }
}

