using NLog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ControlScreener
{
    class ScreenScalingFactor
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        public enum DeviceCap
        {
            VERTRES = 10,
            DESKTOPVERTRES = 117,

            // http://pinvoke.net/default.aspx/gdi32/GetDeviceCaps.html
        }

        public int ScreenDPI;
        public int dpiBase = 96;

        public ScreenScalingFactor()
        {
            Graphics g = Graphics.FromHwnd(IntPtr.Zero);

            IntPtr desktop = g.GetHdc();
            int LogicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.VERTRES);
            int PhysicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPVERTRES);

            float ScreenScalingFactor = (float)PhysicalScreenHeight / (float)LogicalScreenHeight;

            ScreenDPI = (int)(dpiBase * ScreenScalingFactor);

            logger.Trace("Load Screen DPI: " + ScreenDPI);
        }

    }
}
