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
using Microsoft.VisualBasic;

namespace FileManager
{
    public partial class Form1 : Form
    {
        //全局静态数据成员，指的是当前所处目录路径
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
            TreeNode node1 = new TreeNode("备份文件夹");
            TreeNode node2 = new TreeNode("加密文件夹");
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

        [StructLayout(LayoutKind.Sequential)]
        public struct SHELLEXECUTEINFO
        {
            public int cbSize;
            public uint fMask;
            public IntPtr hwnd;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpVerb;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpFile;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpParameters;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpDirectory;
            public int nShow;
            public IntPtr hInstApp;
            public IntPtr lpIDList;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpClass;
            public IntPtr hkeyClass;
            public uint dwHotKey;
            public IntPtr hIcon;
            public IntPtr hProcess;
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
                    if (newtype == "sys" || newtype == "ini" || newtype == "log" || newtype == "com" || newtype == "db") 
                    { }
                    else
                    {
                        SHGetFileInfo(files[i], (uint)0x80, ref shfi, (uint)Marshal.SizeOf(shfi), (uint)(0x100 | 0x400));
                        imglist.Images.Add(fi.Name, (Icon)Icon.FromHandle(shfi.hIcon).Clone());
                        info[0] = fi.Name;
                        double kbLength = fi.Length / 1024;
                        double mbLength;
                        if (fi.Length < 1024)
                            info[1] = fi.Length.ToString("0.00") + "B";
                        else if (kbLength < 1024)
                            info[1] = kbLength.ToString("0.00") + "KB";
                        else if ((mbLength = kbLength / 1024) < 1024)
                            info[1] = Convert.ToDouble(mbLength).ToString("0.00") + "MB";
                        else
                            info[1] = Convert.ToDouble(mbLength / 1024).ToString("0.00") + "GB";
                        info[2] = fi.Extension.ToString();
                        info[3] = fi.LastWriteTime.ToString();
                        ListViewItem item = new ListViewItem(info, fi.Name);
                        lv.Items.Add(item);

                        DestoryIcon(shfi.hIcon);
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
        private void BackPath(ListView lv, ImageList imagelist)
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
        private void ShowAddress()
        {
            toolStripTextBox2.Text = AllPath;
        }



        //树状视图控件中选择一项后的事件，选择不同磁盘，在列表中显示该磁盘文件及文件夹信息
        private void TreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            GetPath(e.Node.FullPath.PadLeft(5).Remove(0,5), imageList1, listView1, 0);
            ShowAddress();
        }

        //双击listview列表控件中项目，打开其或其子文件夹
        private void ListView1_DoubleClick(object sender, EventArgs e)
        {
            GetPath(listView1.SelectedItems[0].Text, imageList1, listView1, 1);
            ShowAddress();
        }

        //单击向上按钮事件
        private void ToolStripButton2_Click(object sender, EventArgs e)
        {
            BackPath(listView1, imageList1);
            ShowAddress();
        }

        //通过输入地址栏信息点击进入按钮或回车键来查看文件和文件夹信息
        private void ToolStripButton3_Click(object sender, EventArgs e)
        {
            try
            {
                AllPath = toolStripTextBox2.Text;
                if (AllPath.Substring(AllPath.Length - 1) != "\\")
                    AllPath = AllPath + "\\";
                GetListViewItem(AllPath, imageList1, listView1);
                ShowAddress();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ToolStripTextBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
                ToolStripButton3_Click(sender, e);               
        }

        public void NewFileDir(ListView lv, ImageList imagelist, string strName, int intflag)
        {
            string strPath = AllPath + strName;
            if (intflag == 0)
            {
                try
                {
                    File.Create(strPath).Close();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else if(intflag == 1)
            {
                try
                {
                    Directory.CreateDirectory(strPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            GetListViewItem(AllPath, imagelist, listView1);
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

        private void 新建文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewFile();
        }

        private void NewFile()
        {
            string filename = Interaction.InputBox("请输入新建文件名", "新建文件", DateTime.Now.ToString("yyyyMMddhhmmss") + ".txt", -1, -1);
            NewFileDir(listView1, imageList1, filename, 0);
        }

        private void 新建文件夹ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewDir();
        }

        private void NewDir()
        {
            string dirname = Interaction.InputBox("请输入新建文件夹名", "新建文件夹", DateTime.Now.ToString("yyyyMMddhhmmss"), -1, -1);
            NewFileDir(listView1, imageList1, dirname, 1);
        }

        private void 属性ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if(listView1.SelectedItems.Count == 1)
                ShowFileProperties(AllPath + listView1.SelectedItems[0].Text);
            else
                ShowFileProperties(AllPath);
        }

        private void ToolStripMenuItem2_Click_1(object sender, EventArgs e)
        {
            Renamefile();
        }

        private void Renamefile()
        {
            if (listView1.SelectedItems.Count == 1)
            {
                try
                {
                    string FileName = AllPath + listView1.SelectedItems[0].Text;
                    string newFileName = Interaction.InputBox("请输入新文件(夹)名", "重命名", listView1.SelectedItems[0].Text, -1, -1);
                    FileSystem.Rename(FileName, AllPath + newFileName);
                    GetListViewItem(AllPath, imageList1, listView1);
                }
                catch { }

            }
            else
            {
                MessageBox.Show("请先选择一个文件或文件夹");
            }
        }

        private const int SW_SHOW = 5;
        private const uint SEE_MASK_INVOKEIDLIST = 12;

        [DllImport("shell32.dll")]
        static extern bool ShellExecuteEx(ref SHELLEXECUTEINFO lpExecInfo);

        public static void ShowFileProperties(string Filename)
        {
            SHELLEXECUTEINFO info = new SHELLEXECUTEINFO();
            info.cbSize = Marshal.SizeOf(info);
            info.lpVerb = "properties";
            info.lpFile = Filename;
            info.nShow = SW_SHOW;
            info.fMask = SEE_MASK_INVOKEIDLIST;
            ShellExecuteEx(ref info);
        }

        private void ListView1_MouseClick(object sender, MouseEventArgs e)
        {
             
            listView1.MultiSelect = false;
            //鼠标右键
            
            if (e.Button == MouseButtons.Right)
            {
                Point p = new Point(e.X, e.Y);
                if (listView1.SelectedItems.Count == 1)
                {
                    string fileName = listView1.SelectedItems[0].Text;
                    contextMenuStrip1.Show(listView1, p);
                }
                else if(listView1.SelectedItems.Count == 0)
                {
                    contextMenuStrip2.Show(listView1, p);
                }
            }

        }

        public void DeleteFile(string dir)
        {
            if (Directory.Exists(dir)) //如果存在这个文件夹删除之 
            {
                foreach (string d in Directory.GetFileSystemEntries(dir))
                {
                    if (File.Exists(d))
                        File.Delete(d); //直接删除其中的文件 
                    else
                        DeleteFile(d); //递归删除子文件夹 
                }
                Directory.Delete(dir); //删除已空文件夹 
            }
            else if (File.Exists(dir))
                File.Delete(dir);
            else
                MessageBox.Show(dir + " 该文件夹不存在"); //如果文件夹不存在则提示 
        }

        private void DeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count >= 1)
            {
                try
                {
                    for (int i = 0; i < listView1.SelectedItems.Count; i++)
                    {
                        string fullName = AllPath + listView1.SelectedItems[i].Text;
                        DeleteFile(fullName);
                    }
                    GetListViewItem(AllPath, imageList1, listView1);
                }
                catch { }

            }
            else
            {
                MessageBox.Show("请先选择一个文件或文件夹");
            }
        }

        private void ListView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                DeleteToolStripMenuItem_Click(sender, e);
            else if (e.KeyCode == Keys.Enter)
                ListView1_DoubleClick(sender, e);
        }

        private void CloseForm1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        
    }
}
