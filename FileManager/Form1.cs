using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;

namespace FileManager
{
    public partial class Form1 : Form
    {
        //全局静态数据成员，指的是当前所处路径
        public static string AllPath = "";

        public Form1()
        {
            InitializeComponent();
        }

        //窗口导入时初始化根文件夹，包括物理磁盘和备份、加密文件夹
        private void Form1_Load(object sender, EventArgs e)
        {
            string[] strDrivers = Environment.GetLogicalDrives();

            treeView1.BeginUpdate();
            TreeNode node0 = new TreeNode("我的电脑");
            treeView1.Nodes.Add(node0);
            foreach(string strDrive in strDrivers)
            {
                TreeNode tnMyDrives = new TreeNode(strDrive);
                node0.Nodes.Add(tnMyDrives);
            }
            TreeNode node1 = new TreeNode("备份文件");
            TreeNode node2 = new TreeNode("加密文件");
            treeView1.Nodes.Add(node1);
            treeView1.Nodes.Add(node2);
            treeView1.EndUpdate();

        }
        
        //调用API函数所需参数结构体
        public struct SHFILEINFO
        {
            public IntPtr hIcon;                                    //文件图标句柄
            public IntPtr iIcon;                                    //图标系统索引号
            public uint dwAttributes;                               //文件属性值
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;                            //文件显示名
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;                               //文件类型名
        }

        
        //声明API函数SHGetFileInfo，利用该函数可以获取文件或文件夹图标
        [DllImport("shell32.dll", EntryPoint = "SHGetFileInfo")]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint Flags);
        //API函数，清楚文件或文件夹图标
        [DllImport("User32.dll", EntryPoint = "DestroyIcon")]
        public static extern int DestoryIcon(IntPtr hIcon);

        //动态获取path路径下所有文件及其图标
        public void GetListViewItem(string path, ImageList imglist, ListView lv)
        {
            lv.Items.Clear();
            SHFILEINFO shfi = new SHFILEINFO();
            try
            {
                string[] dirs = Directory.GetDirectories(path);
                string[] files = Directory.GetFiles(path);
                for(int i = 0; i < dirs.Length; i++)
                {
                    string[] info = new string[4];
                    DirectoryInfo dir = new DirectoryInfo(dirs[i]);
                    if(dir.Name == "$RECYCLE.BIN" || dir.Name == "RECYCLED" || dir.Name == "Recycled" || dir.Name == "System Volume Information")
                    { }
                    else
                    {
                        SHGetFileInfo(dirs[i], 0x80, ref shfi, (uint)Marshal.SizeOf(shfi), 0x100 | 0x400);
                        imglist.Images.Add(dir.Name, (Icon)Icon.FromHandle(shfi.hIcon).Clone());
                        info[0] = dir.Name;
                        info[1] = "";
                        info[2] = "文件夹";
                        info[3] = dir.LastWriteTime.ToString();
                        ListViewItem item = new ListViewItem(info, dir.Name);
                        lv.Items.Add(item);

                        DestoryIcon(shfi.hIcon);
                    }
                }
                for(int i = 0; i < files.Length; i++)
                {
                    string[] info = new string[4];
                    FileInfo fi = new FileInfo(files[i]);
                    string Filetype = fi.Name.Substring(fi.Name.LastIndexOf(".") + 1, fi.Name.Length - fi.Name.LastIndexOf(".") - 1);
                    string newtype = Filetype.ToLower();
                    if (newtype == "sys" || newtype == "ini" || newtype == "bin" || newtype == "log" || newtype == "com" || newtype == "db") 
                    { }
                    else
                    {
                        SHGetFileInfo(files[i], (uint)0x80, ref shfi, (uint)Marshal.SizeOf(shfi), (uint)(0x100 | 0x400));
                        imglist.Images.Add(fi.Name, (Icon)Icon.FromHandle(shfi.hIcon).Clone());
                        info[0] = fi.Name;
                        double dbLength = fi.Length / 1024;
                        if (dbLength < 1024)
                            info[1] = dbLength.ToString("0.00") + "KB";
                        else
                            info[1] = Convert.ToDouble(dbLength / 1024).ToString("0.00") + "MB";
                        info[2] = fi.Extension.ToString();
                        info[3] = fi.LastWriteTime.ToString();
                        ListViewItem item = new ListViewItem(info, fi.Name);
                        lv.Items.Add(item);

                        DestoryIcon(shfi.hIcon);
                    }
                }
            }
            catch { }
        }

        //将指定路径下文件或文件夹显示在列表视图控件listview中
        public void GetPath(string path, ImageList imglist, ListView lv, int intflag)
        {
            string pp = "";
            string uu = "";
            try
            {
                if (intflag == 0)
                {
                    if (AllPath != path)
                    {
                        lv.Items.Clear();
                        AllPath = path;
                        GetListViewItem(AllPath, imglist, lv);
                    }
                }
                else
                {
                    uu = AllPath + path;
                    if (Directory.Exists(uu))
                    {
                        AllPath = AllPath + path + "\\";
                        pp = AllPath.Substring(0, AllPath.Length - 1);
                        lv.Items.Clear();
                        GetListViewItem(pp, imglist, lv);
                    }
                    else
                    {
                        if (path.IndexOf("\\") == -1)
                        {
                            uu = AllPath + path;
                            System.Diagnostics.Process.Start(uu);
                        }
                        else
                            System.Diagnostics.Process.Start(path);
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        
        //向上一层文件夹目录路径
        private void backPath(ListView lv, ImageList imagelist)
        {
            if (AllPath.Length != 3 && AllPath.Length != 0)
            {
                string NewPath = AllPath.Remove(AllPath.LastIndexOf("\\")).Remove(AllPath.Remove(AllPath.LastIndexOf("\\")).LastIndexOf("\\")) + "\\";
                lv.Items.Clear();
                GetListViewItem(NewPath, imagelist, lv);

                AllPath = NewPath;
                
            }
        }

        //将当前路径显示在地址栏
        private void showAddress()
        {
            toolStripTextBox2.Text = AllPath;
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {

        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {

        }

        //树状视图控件中选择一项后的事件，选择不同磁盘，在列表中显示该磁盘文件及文件夹信息
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            GetPath(e.Node.FullPath.PadLeft(5).Remove(0,5), imageList1, listView1, 0);
            showAddress();
        }

        //双击listview列表控件中项目，打开其或其子文件夹
        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            GetPath(listView1.SelectedItems[0].Text, imageList1, listView1, 1);
            showAddress();
        }

        //单击向上按钮事件
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            backPath(listView1, imageList1);
            showAddress();
        }

        //通过输入地址栏信息点击进入按钮或回车键来查看文件和文件夹信息
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            try
            {
                AllPath = toolStripTextBox2.Text;
                if (AllPath.Substring(AllPath.Length - 1) != "\\")
                    AllPath = AllPath + "\\";
                GetListViewItem(AllPath, imageList1, listView1);
                showAddress();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void toolStripTextBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
                toolStripButton3_Click(sender, e);               
        }


        //设定文件和文件夹的显示效果，分别为平铺、图标、列表和详细信息。
        private void 平铺ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.View = View.Tile;
        }

        private void 图标ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.View = View.LargeIcon;
        }

        private void 列表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.View = View.List;
        }

        private void 详细信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.View = View.Details;
        }
    }
}
