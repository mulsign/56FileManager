using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;

namespace FileManager
{
    public partial class Form1_0 : Form
    {
        public Form1_0()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            string confirmpswd = Setting.MD5Encrypt(textBox1.Text);
            StreamReader objReader = new StreamReader(Setting.SettingFile);
            string sLine = "";
            ArrayList LineList = new ArrayList();
            while (sLine != null)
            {
                sLine = objReader.ReadLine();
                if (sLine != null && !sLine.Equals(""))
                    LineList.Add(sLine);
            }
            objReader.Close();
            if (LineList[0].ToString() == confirmpswd)
            {
                MessageBox.Show(confirmpswd);
                Setting.Flag = 1;
                Close();
            }
            else
            {
                MessageBox.Show("密码错误！");
                textBox1.Clear();
            }
        }
    }
}
