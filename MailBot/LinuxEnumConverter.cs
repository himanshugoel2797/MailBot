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
                case MouseButtons.Middle:
                    return XMouseButton.Button2;
                case MouseButtons.Right:
                    return XMouseButton.Button3;
                case MouseButtons.ScrollDown:
                    return XMouseButton.Button5;
                case MouseButtons.ScrollUp:
                    return XMouseButton.Button4;
                case MouseButtons.Back:
                    return (XMouseButton)8;
                case MouseButtons.Forward:
                    return (XMouseButton)9;
            }

            return XMouseButton.AnyButton;
        }
    }
}
