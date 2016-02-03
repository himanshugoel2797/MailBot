using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MailBot
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Bitmap bg;
        Graphics bgG;

        private void Form1_Load(object sender, EventArgs e)
        {
            //this.TransparencyKey = Color.Magenta;
            this.FormBorderStyle = FormBorderStyle.None;
            bg = new Bitmap(this.Width, this.Height);
            bgG = Graphics.FromImage(bg);
            this.BackgroundImageLayout = ImageLayout.None;

            Program.RegisterEvents();
        }

        public void SetColor()
        {
            this.Hide();
            bgG.CopyFromScreen(this.Location, Point.Empty, this.Size, CopyPixelOperation.SourceCopy);
            this.Show();
            this.BackgroundImage = bg;
        }

        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

        static public System.Drawing.Color GetPixelColor(int x, int y)
        {
            IntPtr hdc = GetDC(IntPtr.Zero);
            uint pixel = GetPixel(hdc, x, y);
            ReleaseDC(IntPtr.Zero, hdc);
            Color color = Color.FromArgb((int)(pixel & 0x000000FF),
                         (int)(pixel & 0x0000FF00) >> 8,
                         (int)(pixel & 0x00FF0000) >> 16);
            return color;
        }
    }
}
