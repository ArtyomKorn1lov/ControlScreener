using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ControlScreener
{
    class FileClass
    {
        private string DelayMask = "Delay:";
        private string DifferenceMask = "Difference:";
        private string PathBmpMask = "PATHBmp:";
        private int del_out = 10;
        private int dif_out = 10;
        private string pathbmp = "";
        public int IntConventerFromString(string input_string)
        {
            int out_int = 0;
            try
            {
                out_int = Convert.ToInt32(input_string);
            }
            catch
            {
            }
            return out_int;
        }
        public string File_Reader(string input_name)
        {
            FileStream File = new FileStream(input_name, FileMode.Open, FileAccess.Read, FileShare.Read);
            StreamReader Reader = new StreamReader(File);
            string file_output = Reader.ReadToEnd();
            Reader.Close();
            return file_output;
        }
        public void ReadCFG(string input_string)
        {
            string[] list = input_string.Split('\n');
            int indexOf_var1 = 0;
            int indexOf_var2 = 0;
            int indexOf_var3 = 0;
            string temp = "";
            foreach (string s in list)
            {
                indexOf_var1 = s.IndexOf(DelayMask);
                indexOf_var2 = s.IndexOf(DifferenceMask);
                indexOf_var3 = s.IndexOf(PathBmpMask);
                if (indexOf_var1 != -1)
                {
                    temp = s.Substring(indexOf_var1 + DelayMask.Length);
                    temp = temp.Trim();
                    del_out = IntConventerFromString(temp);
                    if (del_out == 0)
                    {
                        del_out = 10;
                    }
                }
                if (indexOf_var2 != -1)
                {
                    temp = s.Substring(indexOf_var2 + DifferenceMask.Length);
                    temp = temp.Trim();
                    dif_out = IntConventerFromString(temp);
                    if (dif_out == 0)
                    {
                        dif_out = 10;
                    }
                }
                if (indexOf_var3 != -1)
                {
                    temp = s.Substring(indexOf_var3 + PathBmpMask.Length);
                    temp = temp.Trim();
                    pathbmp = temp;
                }
            }
        }
        public int GetDelay()
        {
            return del_out;
        }
        public double GetDifferent()
        {
            return (double)dif_out / 100;
        }
        public string GetPathBMP()
        {
            return pathbmp;
        }
    }
}
