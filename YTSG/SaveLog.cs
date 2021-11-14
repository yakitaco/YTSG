using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YTSG
{
    static class SaveLog
    {
        public static void saveNewFile(string str, int teban)
        {
            DateTime dt = DateTime.Now;
            string result = dt.ToString("yyyyMMdd_HHmmss");
            if (!Directory.Exists("log")) {
                Directory.CreateDirectory("log");
            }
            StreamWriter sw = new StreamWriter("log/" + result + "_" + teban + ".txt", false, Encoding.GetEncoding("shift_jis"));
            sw.Write(str);
            sw.Close();

        }



    }
}
