using System;
using System.Collections.Generic;
using System.Drawing;
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

        public Point targetRes = new Point(0, 0);

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

        private void SendMessageAsync(string msg)
        {
            server.BeginSendTo(eString(msg), 0, msg.Length, SocketFlags.None, targetEP, new AsyncCallback((IAsyncResult ar) =>
            {
                server.EndSendTo(ar);
            }), null);
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


        public void SendClick(MouseButtons button)
        {
            SendMessageAsync($"Click:{button}");
        }

        public void SendMove(int x, int y)
        {
            SendMessageAsync($"Move: {x},{y}");
        }

        public void SendDown(MouseButtons button)
        {
            SendMessageAsync($"Down:{button}");
        }

        public void SendUp(MouseButtons button)
        {
            SendMessageAsync($"Up:{button}");
        }

        public void ListenAsync()
        {
            AsyncCallback callback = null;
            callback = new AsyncCallback((IAsyncResult e) =>
            {
                if (e.IsCompleted)
                {
                    IPEndPoint ep = new IPEndPoint(IPAddress.Any, clientPort);
                    string msg = eString(client.EndReceive(e, ref ep));

                    while (true)
                    {
                        switch (msg.Split(':')[0])
                        {
                            case "Click":
                                Controller.SendMouseEvent((MouseButtons)Enum.Parse(typeof(MouseButtons), msg.Split(':')[1]), true);
                                System.Threading.Thread.Sleep(1);
                                Controller.SendMouseEvent((MouseButtons)Enum.Parse(typeof(MouseButtons), msg.Split(':')[1]), false);
                                break;
                            case "Down":
                                Controller.SendMouseEvent((MouseButtons)Enum.Parse(typeof(MouseButtons), msg.Split(':')[1]), true);
                                break;
                            case "Up":
                                Controller.SendMouseEvent((MouseButtons)Enum.Parse(typeof(MouseButtons), msg.Split(':')[1]), false);
                                break;
                            case "Move":
                                {
                                    int x = int.Parse(msg.Split(':')[1].Split(',')[0]);
                                    int y = int.Parse(msg.Split(':')[1].Split(',')[1]);
                                    Controller.MoveCursor(x, y);
                                }
                                break;
                        }

                        msg = ReceiveMessage(out ep);
                    }
                }
            });

            client.BeginReceive(callback, null);
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

            string resp = "";

            while (true)
            {
                resp = SendMessageAndWait($"Shake:{key}", out servEP);
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

            bool commence = false;
            while (!commence)
            {
                resp = ReceiveMessage(out servEP);

                string cmd = resp.Split(':')[0];
                switch (cmd)
                {
                    case "Commence":
                        commence = true;
                        break;
                    case "GetResolution?":
                        {
                            Point p = Controller.GetResolution();
                            SendMessage($"GetResolution:{p.X},{p.Y}");

                            int x = int.Parse(resp.Split(':')[1].Split(',')[0]);
                            int y = int.Parse(resp.Split(':')[1].Split(',')[1]);
                            targetRes = new Point(x, y);
                        }
                        break;
                }
            }
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
                        targetEP = new IPEndPoint(clientEP.Address, serverPort);

                        int cKey = int.Parse(msg.Split(':')[1]);

                        Random rng = GetIPRandom();
                        int key = rng.Next();

                        msg = SendMessageAndWait($"Shakeback:{cKey},{key}", out clientEP);

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

            //Set cursor positions
            IPEndPoint dummy = new IPEndPoint(IPAddress.Any, 0);

            SendMove(0, 0);
            Controller.MoveCursor(0, 0);
            string cRes = "";

            while (true)
            {
                Point res = Controller.GetResolution();
                cRes = SendMessageAndWait($"GetResolution?:{res.X},{res.Y}", out dummy);
                if (cRes.StartsWith("GetResolution:"))
                {
                    int x = int.Parse(cRes.Split(':')[1].Split(',')[0]);
                    int y = int.Parse(cRes.Split(':')[1].Split(',')[1]);
                    targetRes = new Point(x, y);

                    break;
                }
                else
                {
                    Console.WriteLine("Invalid Response, Retrying");
                }
            }

            SendMessage("Commence");

            //At this point the server has successfully connected to the client
            Console.WriteLine("Connection Established");
        }

    }
}
