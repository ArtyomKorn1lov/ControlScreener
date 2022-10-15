using NLog;
using System.Drawing;

namespace ControlScreener
{
    class ScreenAnalyser
    {
        private Bitmap FirstScreen;
        private Bitmap SecondScreen;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public string NameConverter(string data)
        {
            data = data.Replace("\\", "#");
            return data;
        }
        public void AddImageToAnalyse(Bitmap Bmp_Obj, int counter, string date, double difference, string path)
        {
            if (counter == 1)
            {
                FirstScreen = Bmp_Obj;
                bool flagblack = true;
                Color PixelOld = FirstScreen.GetPixel(0, 0);
                for (int count1 = 0; count1 < FirstScreen.Width; count1++)
                {
                    for (int count2 = 0; count2 < FirstScreen.Height; count2++)
                    {
                        if ((FirstScreen.GetPixel(count1, count2) != PixelOld) && (!FirstScreen.GetPixel(count1, count2).IsEmpty))
                        {
                            flagblack = false;
                            break;
                        }
                    }
                    if (!flagblack) break;
                }
                if (!flagblack)
                {
                    SecondScreen = Bmp_Obj;
                    BitmapSave(date, path);
                } else
                {
                    logger.Warn("Screenshot black!");
                }
            }
            else
            {
                SecondScreen = Bmp_Obj;
                ImageAnalyse(date, difference, path);
            }
        }
        public void ImageAnalyse(string date, double difference, string path)
        {
            int counter = 0;
            int x1 = FirstScreen.Height;
            int y1 = FirstScreen.Width;
            int x2 = SecondScreen.Height;
            int y2 = SecondScreen.Width;
            bool flagblack = true;
            bool flagnew = false;
            Color PixelOld = SecondScreen.GetPixel(0, 0);

            if (x1 == x2 && y1 == y2)
            {
                for (int count1 = 0; count1 < SecondScreen.Width; count1++)
                {
                    for (int count2 = 0; count2 < SecondScreen.Height; count2++)
                    {
                        if (FirstScreen.GetPixel(count1, count2) != SecondScreen.GetPixel(count1, count2))
                        {
                            counter++;
                        }
                        if ((SecondScreen.GetPixel(count1, count2) != PixelOld) && (!SecondScreen.GetPixel(count1, count2).IsEmpty))
                        {
                            flagblack = false;
                        }
                        if (counter >= (SecondScreen.Width * SecondScreen.Height * difference))
                        {
                            flagnew = true;
                            break;
                        }
                    }
                    if (flagnew) break;
                }

                if (flagblack) {
                    counter = 0;
                    logger.Warn("Screenshot black!");
                    return;
                }

                logger.Trace("PixelNew " + counter + " % " + (counter * 100)/(SecondScreen.Width * SecondScreen.Height) );
                if (flagnew)
                {
                    BitmapSave(date, path);
                } else
                {
                    logger.Warn("Screenshot no changes!");
                }

            } else
            {
                BitmapSave(date, path);
            }
        }
        
        private void BitmapSave(string date, string path)
        {
            string name = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            name = name + "#" + date + ".bmp";
            name = NameConverter(name);
            logger.Trace("File name " + path + name);
            SecondScreen.Save(path + name);
            FirstScreen = SecondScreen;
        }
    }
}
