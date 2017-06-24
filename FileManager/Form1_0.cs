using System;
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
            SQLiteHelper login = new SQLiteHelper();

            if (login.CheckPasswd(textBox1.Text))
            {
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
