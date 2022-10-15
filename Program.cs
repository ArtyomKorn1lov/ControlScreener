
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using NLog;
using System.Runtime.InteropServices;

namespace ControlScreener
{
    public class main : ConsoleClass
    {
        public static bool flag = true;
        static string TimeConverter(string data)
        {
            data = data.Replace(".", "#");
            data = data.Replace(" ", "#");
            data = data.Replace(":", "#");
            return data;
        }
        private static Logger logger = LogManager.GetCurrentClassLogger();
        
        static void Main()
        {
            logger.Info("ControlScreener start v.1.0.7");
            int counter = 0;
            FileClass F = new FileClass();
            string input_string = F.File_Reader("config.cfg");
            F.ReadCFG(input_string);
            logger.Trace("Config load");
            int delay = F.GetDelay();
            double difference = F.GetDifferent();
            string pathbmp = F.GetPathBMP();
            ScreenAnalyser Analys = new ScreenAnalyser();
            while (true)
            {
                counter++;
                if (!IsWorkstationLocked())
                {
                    logger.Trace("Session not blokked");
                    Bitmap Bmp_Obj = null;
                    try
                    {
                        Bmp_Obj = AutoScreener.ScreenCompliter();
                        logger.Trace("Bitmap AutoScreener create");
                    }
                    catch
                    {
                        logger.Error("{0} Exception caught AutoScreener");

                        try
                        {
                            Bmp_Obj = CaptureScreen.GetDesktopImage();
                            logger.Trace("Bitmap CaptureScreen create");
                        }
                        catch
                        {
                            logger.Error("{0} Exception caught CaptureScreen");
                            Bmp_Obj = null;
                        }
                    }
                    
                    if (Bmp_Obj != null)
                    {
                        logger.Trace("Bitmap create");
                        Analys.AddImageToAnalyse(Bmp_Obj, counter, TimeConverter(Convert.ToString(DateTime.Now)), difference, pathbmp);
                        logger.Trace("Bitmap analyse");
                    }
                    else
                    {
                        logger.Warn("Bitmap not create");
                    }

                }
                else 
                {
                    logger.Warn("Session blokked"); 
                }
                GC.Collect(4);
                System.Threading.Thread.Sleep(delay * 1000);
                logger.Trace("Delay " + delay + " sec");
            }
            logger.Info("ControlScreener close");

        }


        [DllImport("user32", EntryPoint = "OpenDesktopA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern Int32 OpenDesktop(string lpszDesktop, Int32 dwFlags, bool fInherit, Int32 dwDesiredAccess);

        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern Int32 CloseDesktop(Int32 hDesktop);

        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern Int32 SwitchDesktop(Int32 hDesktop);

        public static bool IsWorkstationLocked()
        {
            const int DESKTOP_SWITCHDESKTOP = 256;
            int hwnd = -1;
            int rtn = -1;

            hwnd = OpenDesktop("Default", 0, false, DESKTOP_SWITCHDESKTOP);

            if (hwnd != 0)
            {
                rtn = SwitchDesktop(hwnd);
                if (rtn == 0)
                {
                    // Locked
                    CloseDesktop(hwnd);
                    return true;
                }
                else
                {
                    // Not locked
                    CloseDesktop(hwnd);
                }
            }
            else
            {
                // Error: "Could not access the desktop..."
            }

            return false;
        }
    }
}
