using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XSharp;

namespace MailBot
{
    class LinuxEnumConverter
    {
        public static XMouseButton E(MouseButtons b)
        {
            switch (b)
            {
                case MouseButtons.Left:
                    return XMouseButton.Button1;
                case MouseButtons.Right:
                    return XMouseButton.Button2;
                case MouseButtons.Button3:
                    return XMouseButton.Button3;
                case MouseButtons.Button4:
                    return XMouseButton.Button4;
                case MouseButtons.Button5:
                    return XMouseButton.Button5;
            }

            return XMouseButton.AnyButton;
        }
    }
}
