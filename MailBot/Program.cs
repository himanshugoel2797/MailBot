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
            SMTPSys s = new SMTPSys(args[0], args[1]);

            if(PlatformInfo.RunningPlatform() == Platform.Linux)
            {
                LinuxMouseController.MoveCursor(100, 100);
            }
        }
    }
}
