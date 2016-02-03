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

            string resp = SendMessageAndWait("Shake", out servEP);

        }

        //Server constructor
        public Communicator()
        {
            client = new UdpClient(clientPort);
            IPEndPoint clientEP = new IPEndPoint(IPAddress.Any, clientPort);

            string msg = ReceiveMessage(out clientEP);
            Console.WriteLine(msg);
        }

    }
}
