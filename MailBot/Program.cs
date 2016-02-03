using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MailBot
{
    class Program
    {
        static IKeyboardMouseEvents m_events;
        static bool transmitMouse = false;
        static Communicator com = null;
        static Point srcMousePos = new Point(0, 0);
        static Point dstMousePos = new Point(0, 0);
        static Form f;

        internal static void RegisterEvents()
        {
            m_events = Hook.GlobalEvents();
            m_events.MouseDownExt += M_events_MouseDownExt;
            m_events.MouseUpExt += M_events_MouseDownExt;
            m_events.MouseMoveExt += M_events_MouseMove;
            m_events.MouseClick += M_events_MouseClick;
            m_events.MouseWheelExt += M_events_MouseDownExt;
        }

        private static void M_events_MouseClick(object sender, MouseEventArgs e)
        {
            if (transmitMouse)
            {
                if (e.Button.HasFlag(System.Windows.Forms.MouseButtons.Left)) com.SendClick(MouseButtons.Left);

                if (e.Button.HasFlag(System.Windows.Forms.MouseButtons.Right)) com.SendClick(MouseButtons.Right);

                if (e.Button.HasFlag(System.Windows.Forms.MouseButtons.Middle)) com.SendClick(MouseButtons.Middle);

            }
        }

        static void M_events_MouseMove(object sender, MouseEventExtArgs e)
        {
            if (transmitMouse)
            {
                com.SendMove(e.X, e.Y);
                f.Location = new Point(e.X - f.Width / 2, e.Y - f.Height / 2);
            }
        }

        static void M_events_MouseDownExt(object sender, MouseEventExtArgs e)
        {
            if (e.Button == (System.Windows.Forms.MouseButtons.XButton2) && e.IsMouseButtonDown)
            {
                e.Handled = true;
                transmitMouse = !transmitMouse;
                if (transmitMouse)
                {
                    Cursor.Hide();
                    Controller.MoveCursor(dstMousePos.X, dstMousePos.Y);
                    srcMousePos = e.Location;
                }
                else
                {
                    Cursor.Show();
                    Controller.MoveCursor(srcMousePos.X, srcMousePos.Y);
                    dstMousePos = e.Location;
                }
                return;
            }

            if (transmitMouse)
            {
                e.Handled = true;
                //if (e.Button.HasFlag(System.Windows.Forms.MouseButtons.Left) && e.Clicked) com.SendClick(MouseButtons.Left);
                //else 
                if (e.Button.HasFlag(System.Windows.Forms.MouseButtons.Left) && e.IsMouseButtonDown) com.SendDown(MouseButtons.Left);
                if (e.Button.HasFlag(System.Windows.Forms.MouseButtons.Left) && (e.IsMouseButtonUp | e.Clicked)) com.SendUp(MouseButtons.Left);

                //if (e.Button.HasFlag(System.Windows.Forms.MouseButtons.Right) && e.Clicked) com.SendClick(MouseButtons.Right);
                //else 
                if (e.Button.HasFlag(System.Windows.Forms.MouseButtons.Right) && e.IsMouseButtonDown) com.SendDown(MouseButtons.Right);
                if (e.Button.HasFlag(System.Windows.Forms.MouseButtons.Right) && (e.IsMouseButtonUp | e.Clicked)) com.SendUp(MouseButtons.Right);

                //if (e.Button.HasFlag(System.Windows.Forms.MouseButtons.Middle) && e.Clicked) com.SendClick(MouseButtons.Middle);
                //else 
                if (e.Button.HasFlag(System.Windows.Forms.MouseButtons.Middle) && e.IsMouseButtonDown) com.SendDown(MouseButtons.Middle);
                if (e.Button.HasFlag(System.Windows.Forms.MouseButtons.Middle) && (e.IsMouseButtonUp | e.Clicked)) com.SendUp(MouseButtons.Middle);


            }
        }

        static void Main(string[] args)
        {
            if (args.Length < 3)
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

            if (args[0] == "server") new SMTPSys(args[2], args[3]);

            if (args[0] == "server") com = new Communicator();
            else if (args[0] == "client") com = new Communicator(args[2]);

            if (com == null)
            {
                Console.WriteLine("Invalid Arguments");
                return;
            }

            com.ListenAsync();  //Start listening

            if (PlatformInfo.RunningPlatform() == Platform.Windows)
            {
                f = new Form1();
                Application.Run(f);
            }

            while (true)
            {
                Console.ReadLine();
                com.SendClick(MouseButtons.Right);
            }

        }
    }
}
