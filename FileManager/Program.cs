using System;
using System.IO;
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
            if (File.Exists(Setting.SettingFile))
            {
                Application.Run(new Form1_0());
            }
            else
            {
                Application.Run(new Form0_0());
                Application.Run(new Form0_1());
                
            }
            if (Setting.Flag == 1)
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + "\\temp");
                Setting.HidDir(Environment.CurrentDirectory + "\\temp");
                Application.Run(new Form1());
            }
                

        }
    }
}
