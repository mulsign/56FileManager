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
                SQLiteHelper initial = new SQLiteHelper();
                initial = new SQLiteHelper();
                SQLiteHelper.passwd = System.Text.Encoding.UTF8.GetBytes(Setting.MD5Encrypt(passWD.Text));
                initial.CreatNewDatabase();

                initial.DatabaseInitial(0, Setting.MD5Encrypt(passWD.Text));
                initial.DatabaseInitial(1);

                Close();
            }
            else
            {
                MessageBox.Show("两次密码不一致，请重新输入！");
                passWD.Clear();
                passWDC.Clear();
            }
        }

        private void passWDC_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Button1_Click(sender, e);
            }
        }
    }


}
