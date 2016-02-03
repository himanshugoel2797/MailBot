using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using XSharp;

namespace MailBot
{
    static class LinuxController
    {
		[DllImport("libXtst.so")]
		static extern int XTestFakeButtonEvent(IntPtr dsp, uint button, bool isPress, ulong delay);

		[DllImport("libXtst.so")]
		static extern int XTestFakeMotionEvent(IntPtr dsp, int screen_number, int x, int y, ulong delay);

        static XDisplay dsp;
        static XWindow root_window;

		static LinuxController()
		{
			dsp = new XDisplay (null);
			root_window = new XWindow (dsp);
		}

        public static void MoveCursor(int x, int y)
        {
            root_window.SelectInput(XEventMask.KeyReleaseMask);

            XPointer ptr = new XPointer(dsp);

            Point p = GetCursor();
            ptr.Warp(root_window, root_window, 0, 0, 0, 0, -p.X, -p.Y);
            ptr.Warp(root_window, root_window, 0, 0, 0, 0, x, y);
            dsp.Flush();
        }

        public static Point GetCursor()
        {
            XPointer ptr = new XPointer(dsp);
            var d = ptr.Query(root_window);
            return new Point(d.root_x, d.root_y);
        }

		public static void SendMouseEvent(MouseButtons mouseButton, bool doubleClick)
        {
			XPointer p = new XPointer (dsp);
			var pQinfo = p.Query (root_window);

			XButtonEvent b = new XButtonEvent ();

			b.root = pQinfo.root;
			b.window = pQinfo.child;
			b.x_root = pQinfo.root_x;
			b.y_root = pQinfo.root_y;
			b.x = pQinfo.win_x;
			b.y = pQinfo.win_y;
			b.state = pQinfo.mask;

			uint i = (uint)LinuxEnumConverter.E(mouseButton);

			XTestFakeButtonEvent (dsp.Handle, i, true,  0);
			XTestFakeButtonEvent (dsp.Handle, i, false, 100);

			if (doubleClick) {
				XTestFakeButtonEvent (dsp.Handle, i, true,  200);
				XTestFakeButtonEvent (dsp.Handle, i, false, 300);
			}

			dsp.Flush ();
        }

    }
}
