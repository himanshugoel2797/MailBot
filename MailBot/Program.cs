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
                Console.WriteLine("Arguments [client/server] [(Shared Edge):top/bottom/left/right] [gmail username]@gmail.com [gmail password]");
            }

            SMTPSys s = new SMTPSys(args[2], args[3]);

            if (PlatformInfo.RunningPlatform() == Platform.Linux)
            {
                LinuxController.MoveCursor(100, 100);
            }
        }
    }
}
