using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MailBot
{
    class WindowsController
    {
        public static void MoveCursor(int x, int y)
        {
            Cursor.Clip = Screen.PrimaryScreen.Bounds;
            Cursor.Position = new System.Drawing.Point(x, y);
        }

        public static Point GetCursor()
        {
            return Cursor.Position;
        }
    }
}
