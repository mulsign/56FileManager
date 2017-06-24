using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace FileManager
{
    public partial class LogViewer : Form
    {
        public LogViewer()
        {
            InitializeComponent();
        }

        private void LogViewer_Load(object sender, EventArgs e)
        {

            //创建数据库连接类的对象
            SQLiteConnection con = new SQLiteConnection("Data Source=" + Setting.dbFile + ";Version=3;");
            con.SetPassword(SQLiteHelper.passwd);
            con.Open();
            //执行con对象的函数，返回一个SqlCommand类型的对象
            SQLiteCommand cmd = con.CreateCommand();
            //把输入的数据拼接成sql语句，并交给cmd对象
            cmd.CommandText = "select * from logs_table";

            //用cmd的函数执行语句，返回SqlDataReader类型的结果dr,dr就是返回的结果集（也就是数据库中查询到的表数据）
            SQLiteDataReader dr = cmd.ExecuteReader();
            //用dr的read函数，每执行一次，返回一个包含下一行数据的集合dr
            while (dr.Read())
            {
                //构建一个ListView的数据，存入数据库数据，以便添加到listView1的行数据中
                ListViewItem lt = new ListViewItem();
                //将数据库数据转变成ListView类型的一行数据
                lt.Text = dr[0].ToString();
                lt.SubItems.Add(dr[1].ToString());              
                lt.SubItems.Add(dr[2].ToString());
                //将lt数据添加到listView1控件中
                listView1.Items.Add(lt);
            }

            con.Close();
        }
    }
}
