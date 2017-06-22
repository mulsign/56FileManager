using System;
using System.Windows.Forms;
using System.IO;

namespace FileManager
{
    public partial class Form0_1 : Form
    {
        public Form0_1()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            //System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient(address.Text, 21);
            //bool state = client.Connected;
            //System.Net.Sockets.NetworkStream s = client.GetStream();//分析这个s应该就可以知道ftp的状态了

            label5.Visible = true;
            button2.Enabled = true;
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            SQLiteHelper FTPsetting = new SQLiteHelper();
            FTPsetting.ConnectToDatabase(0);
            FTPsetting.InsertSettings(1, "FTPserver", address.Text);
            FTPsetting.InsertSettings(2, "USER", user.Text);
            FTPsetting.InsertSettings(3, "PASS", passwd.Text);
            FTPsetting.CloseDatabase(0);
            Directory.CreateDirectory(Environment.CurrentDirectory + "\\Backup");
            Directory.CreateDirectory(Environment.CurrentDirectory + "\\Crypt");
            Setting.Flag = 1;
            Close();
        }
    }
}
