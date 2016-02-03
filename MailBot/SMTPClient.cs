using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AE.Net.Mail;
using System.Net;
using System.Net.Mail;

namespace MailBot
{
    class SMTPSys
    {
        class SMTPCreds : ICredentialsByHost
        {
            string username, password;

            public SMTPCreds(string username, string password)
            {
                this.username = username;
                this.password = password;
            }

            public NetworkCredential GetCredential(string host, int port, string authenticationType)
            {
                return new NetworkCredential(username, password);
            }
        }

        ImapClient ic;
        SmtpClient smtp;
        string username;
        string password;

        public SMTPSys(string username, string password)
        {
            this.username = username;
            this.password = password;

            ImapClient ic = new ImapClient("imap.gmail.com", username, password, AuthMethods.Login, 993, true);
            ic.SelectMailbox("INBOX");

            ic.NewMessage += Ic_NewMessage;

            Console.WriteLine("Monitoring for commands...");
        }

        private void SendMessage(string body, string destAddr)
        {
            smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.EnableSsl = true;
            smtp.Credentials = new SMTPCreds(username, password);
            smtp.Send(username, destAddr, "BOT-REPLY", body);
            smtp.Dispose();
        }

        private void Ic_NewMessage(object sender, AE.Net.Mail.Imap.MessageEventArgs e)
        {
            //Check the message and send a reply
            if (ic == null)
            {
                ic = new ImapClient("imap.gmail.com", username, password, AuthMethods.Login, 993, true);
                ic.NewMessage += Ic_NewMessage;
            }
            AE.Net.Mail.MailMessage msg = ic.GetMessage(e.MessageCount - 1);   //Get the newest email

            switch (msg.Subject)
            {
                case "GetIP":  //Retrieve VNC info and send reply
                    {
                        Console.WriteLine($"GetIP command received from {msg.From.Address}");
                        string hostName = Dns.GetHostName();
                        var addrs = Dns.GetHostEntry(hostName);

                        string body = $"Host Name: {hostName}\n";

                        for (int i = 0; i < addrs.AddressList.Length; i++)
                        {
                            body += $"IPAddress #{i + 1}: {addrs.AddressList[i].ToString()}\n";
                        }

                        SendMessage(body, msg.From.Address);
                    }
                    break;
                case "Help":
                    {
                        Console.WriteLine($"Help command received from {msg.From.Address}");

                        string body = "Commands:\n";
                        body += "GetIP - Get IP Addresses\n";

                        SendMessage(body, msg.From.Address);
                    }
                    break;
            }

            ic.DeleteMessage(msg);
        }
    }
}
