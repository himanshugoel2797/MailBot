using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MailBot
{
    class Communicator
    {
        UdpClient client;
        Socket server;

        const int clientPort = 11000;   //Port used by client to talk to server
        const int serverPort = 11001;   //Port used by server to talk to client

        IPEndPoint targetEP;


        private static byte[] eString(string msg)
        {
            return Encoding.ASCII.GetBytes(msg);
        }

        private static string eString(byte[] msg)
        {
            return Encoding.ASCII.GetString(msg);
        }

        private void SendMessage(string msg)
        {
            server.SendTo(eString(msg), targetEP);
        }

        private string ReceiveMessage(out IPEndPoint ep)
        {
            IPEndPoint e = new IPEndPoint(IPAddress.Any, 0);
            var data = client.Receive(ref e);
            ep = e;
            return eString(data);
        }

        private string SendMessageAndWait(string msg, out IPEndPoint ep)
        {
            SendMessage(msg);
            return ReceiveMessage(out ep);
        }

        private Random GetIPRandom()
        {
            IPAddress srcAddr = Dns.GetHostAddresses(Dns.GetHostName())[0];
            return new Random(BitConverter.ToInt32(srcAddr.GetAddressBytes(), 0));
        }

        //Client constructor
        public Communicator(string dstAddr)
        {
            //Negotiate connection with server and initialize two way communication channels
            server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPAddress dstAddress = IPAddress.Parse(dstAddr);
            IPEndPoint ep = new IPEndPoint(dstAddress, clientPort);
            targetEP = ep;

            client = new UdpClient(serverPort);
            IPEndPoint servEP = new IPEndPoint(IPAddress.Any, serverPort);

            Random rng = GetIPRandom();
            int key = rng.Next();

            while (true)
            {
                string resp = SendMessageAndWait($"Shake:{key}", out servEP);
                if (resp.StartsWith("Shakeback:"))
                {
                    try
                    {
                        int cKey = int.Parse(resp.Split(':')[1].Split(',')[0]);
                        if (cKey != key) throw new Exception("Bad Handshake received, trying handshake again");

                        int sKey = int.Parse(resp.Split(':')[1].Split(',')[1]);
                        resp = SendMessageAndWait($"ShakebackAccepted:{sKey}", out servEP);

                        if (resp == "ConnectionAccepted") break;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }

            //At this point the client has successfully connected to the server
            Console.WriteLine("Connection Established");
        }

        //Server constructor
        public Communicator()
        {
            client = new UdpClient(clientPort);
            IPEndPoint clientEP = new IPEndPoint(IPAddress.Any, clientPort);

            while (true)
            {
                string msg = ReceiveMessage(out clientEP);
                if (msg.StartsWith("Shake:"))
                {
                    try
                    {
                        server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                        targetEP = clientEP;

                        int key = int.Parse(msg.Split(':')[1]);

                        Random rng = GetIPRandom();
                        msg = SendMessageAndWait($"Shakeback:{key},{rng.Next()}", out clientEP);

                        if (msg.StartsWith("ShakebackAccepted:"))
                        {
                            int sKey = int.Parse(msg.Split(':')[1]);
                            if (sKey != key) throw new Exception("Warning: Invalid Handshake received, Ignored");

                            SendMessage("ConnectionAccepted");
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }

            //At this point the server has successfully connected to the client
            Console.WriteLine("Connection Established");
        }

    }
}
