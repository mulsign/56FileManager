using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileManager
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            int flag = 0;
            string[] CurrentDirFile = Directory.GetFiles(Environment.CurrentDirectory);
            for (int i = 0; i < CurrentDirFile.Length; i++)
            {
                FileInfo fi = new FileInfo(CurrentDirFile[i]);
                if (fi.Name == "56settings")
                    flag = 1;
            }
            if (flag == 1)
                Application.Run(new Form1());
            else
                Application.Run(new Form());
        }
    }
}
