using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailBot
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 4)
            {
                args = new string[] { "", "", "", "" };

                while (!new string[] { "client", "server" }.Contains(args[0]))
                {
                    if (args[0] != "") Console.WriteLine("Invalid Input.");
                    Console.WriteLine("client or server?");
                    args[0] = Console.ReadLine();
                }

                while (!new string[] { "top", "bottom", "left", "right" }.Contains(args[1]))
                {
                    if (args[1] != "") Console.WriteLine("Invalid Input.");
                    Console.WriteLine("active area on top/bottom/left/right?");
                    args[1] = Console.ReadLine();
                }

                if (args[0] == "server")
                {
                    Console.WriteLine("Enter bot gmail address:");
                    args[2] = Console.ReadLine();

                    Console.WriteLine("Enter bot gmail password:");

                    string pass = "";
                    ConsoleKeyInfo c = new ConsoleKeyInfo(' ', ConsoleKey.Spacebar, false, false, false);
                    while (c.KeyChar != '\n')
                    {
                        c = Console.ReadKey();
                        if (c.Key == ConsoleKey.Enter)
                        {
                            break;
                        }
                        else if (c.Key == ConsoleKey.Backspace)
                        {
                            if (pass.Length > 0)
                            {
                                pass.Remove(pass.Length - 1);
                            }
                        }
                        else
                        {
                            Console.Write("\b \b");
                            pass += c.KeyChar;
                        }
                    }

                    args[3] = pass;
                }
                else if (args[0] == "client")
                {
                    Console.WriteLine("Enter server IP:");
                    args[2] = Console.ReadLine();
                }

            }

            System.Threading.Thread.Sleep(1000);
            Controller.MoveCursor(25, 110);
            Controller.SendMouseEvent(MouseButtons.Left, true);
            System.Threading.Thread.Sleep(1);
            Controller.SendMouseEvent(MouseButtons.Left, false);

            if (args[0] == "server") new SMTPSys(args[2], args[3]);

            Communicator com;

            if (args[0] == "server") com = new Communicator();
            else if (args[0] == "client") com = new Communicator(args[2]);

            while (true) ;

        }
    }
}
