﻿using ChatClient.Net.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.Net
{
    class Server
    {
        TcpClient _client;
        public PacketReader PacketReader;
        PacketBuilder  _packetBuilder;

        public event Action connectedEvent;
        public event Action msgReceivedEvent;
        public event Action UserDisconnectEvent;
        public Server()
        {
            _client = new TcpClient();
        }

        public void ConnectToServer(string username)
        {
            if (!_client.Connected)
            {
                _client.Connect("127.0.0.1", 80);
                PacketReader = new PacketReader(_client.GetStream());

                if (!string.IsNullOrEmpty(username))
                {

                    var connectPacket = new PacketBuilder();
                    connectPacket.WriteOpCode(0);
                    connectPacket.WriteMessage(username);
                    _client.Client.Send(connectPacket.GetPacketBytes());
                }
                ReadPackets();
            }

        }
        private void ReadPackets()
        {
            Task.Run(() =>
                {
                    while (true)
                    {
                        var opcode = PacketReader.ReadByte();
                        switch (opcode)
                        {

                            case 1:
                                connectedEvent?.Invoke();
                                break;

                                case 5:
                                    msgReceivedEvent?.Invoke();
                                break;
                            case 10:
                                UserDisconnectEvent?.Invoke();
                                break;

                            default:
                                Console.WriteLine("Unknown opcode");
                                break;
                        }
                    }
                });
        }
        public void SendMessageToServer(string message)
        {
            var Messagepacket = new PacketBuilder();
            Messagepacket.WriteOpCode(5);
            Messagepacket.WriteMessage(message);
            _client.Client.Send(Messagepacket.GetPacketBytes());
        }
        }
    }