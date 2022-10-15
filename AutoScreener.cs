using NLog;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace ControlScreener
{
    static class AutoScreener
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public static Bitmap ScreenCompliter()
        {

            Graphics graph = null;

            ScreenScalingFactor scf = new ScreenScalingFactor();

            //           var bmp = new Bitmap(((Screen.PrimaryScreen.Bounds.Width * scf.ScreenDPI) / scf.dpiBase), (Screen.PrimaryScreen.Bounds.Height * scf.ScreenDPI) / scf.dpiBase);
                       var bmp = new Bitmap(((SystemInformation.VirtualScreen.Width * scf.ScreenDPI) / scf.dpiBase), (SystemInformation.VirtualScreen.Height * scf.ScreenDPI) / scf.dpiBase);

            if (bmp != null)
            {
                logger.Trace("Create bitmap");
                graph = Graphics.FromImage(bmp);
                logger.Trace(" Bmp with - " + bmp.Width + " height - " + bmp.Height);

                if (graph != null)
                {
                    logger.Trace("Create graphics");
                    graph.CopyFromScreen(0, 0, 0, 0, bmp.Size);
                    GC.Collect();
                    logger.Trace("Create bmp");
                    return bmp;
                } else
                {
                    GC.Collect();
                    logger.Warn("Graphics not create");
                    return null;
                }
            } else
            {
                GC.Collect();
                logger.Warn("Bitmap not create");
                return null;
            }
        }

    }
}
