using System;
using System.Windows.Forms;
using System.IO;

namespace FileManager
{
    public partial class Form0_0: Form
    {
        public Form0_0()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if(passWD.Text == passWDC.Text)
            {
                string password = Setting.MD5Encrypt(passWD.Text);
                FileStream myFs = new FileStream(Setting.SettingFile, FileMode.CreateNew);
                StreamWriter mySw = new StreamWriter(myFs);
                mySw.Write(password + "\n");
                mySw.Close();
                myFs.Close();
                Setting.Flag = 1;
                Close();
            }
            else
            {
                MessageBox.Show("两次密码不一致，请重新输入！");
                passWD.Clear();
                passWDC.Clear();
            }
        }
    }


}
