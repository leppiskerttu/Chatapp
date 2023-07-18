using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using ChatServer.Net.IO;
using System.Linq.Expressions;

namespace ChatServer
{
    class Client
    {
        public string Username { get; set; }
        public Guid UID { get; set; }
        public TcpClient ClientSocket { get; set; }

        PacketReader _packetReader;

        public Client(TcpClient client)
        {
            ClientSocket = client;
            UID = Guid.NewGuid();
            _packetReader = new PacketReader(ClientSocket.GetStream());

            var opcode = _packetReader.ReadByte();
            Username = _packetReader.ReadMessage();

            Console.WriteLine($"[{DateTime.Now}]:Client Has Connected With the Username:{Username}");

            Task.Run(() => Process());
        }

        void Process()
        {
            try
            {
                while (true)
                {
                    var opcode = _packetReader.ReadByte();
                    switch (opcode)
                    {
                        case 5:
                            var message = _packetReader.ReadMessage();
                            Console.WriteLine($"[{DateTime.Now}]:{Username} says: {message}");
                            Program.BroadcastMessage($"[{DateTime.Now}] {Username}: {message}", UID);

                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine($"[{DateTime.Now}]:{Username} has disconnected");
                Program.BroadcastDisconnect(Username, UID);
                ClientSocket.Close();
                return;
            }
        }
    }
}