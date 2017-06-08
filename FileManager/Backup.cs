using System;
using System.Configuration;
using System.Data.SQLite;
using System.IO;

class Bakuper
{
    private static string srcPath = ConfigurationManager.AppSettings["srcPath"];
    private static string destPath = ConfigurationManager.AppSettings["destPath"];
    private static int fileCount;
    private static int copyCount;
    private static string connectionString = string.Empty;
    private static SQLiteConnection conn;
    private static string TB_NAME = "dc";

    public static void SetConnectionString(string datasource, string password, int version = 3)
    {
        connectionString = string.Format("Data Source={0};Version={1};password={2}",
            datasource, version, password);
    }

    public static int BackupFile()
    {
        fileCount = 0;
        copyCount = 0;
        SetConnectionString("C:/user/AppData/Local/56FM/backup.db", "123456");
        conn = new SQLiteConnection(connectionString);
        DirectoryInfo theFolder = new DirectoryInfo(srcPath);
        ReadFolderList(theFolder);
        ReadFileList(theFolder);
        Console.WriteLine("共备份了" + copyCount + "个文件");
        return copyCount;
    }

    static void ReadFolderList(DirectoryInfo folder)
    {
        DirectoryInfo[] dirInfo = folder.GetDirectories();
        //遍历文件夹
        foreach (DirectoryInfo NextFolder in dirInfo)
        {
            ReadFolderList(NextFolder);
            ReadFileList(NextFolder);
        }
    }

    static void ReadFileList(DirectoryInfo folder)
    {
        FileInfo[] fileInfo = folder.GetFiles();
        foreach (FileInfo NextFile in fileInfo)  //遍历文件
        {
            SQLiteCommand cmd = new SQLiteCommand("select lastWriteTime from " + TB_NAME + " where fullPath='" + NextFile.FullName + "'", conn);
            object obj = cmd.ExecuteScalar();
            if (obj == null)//如果是新增的文件
            {
                String fullname = folder.FullName;
                string newpath = fullname.Replace(srcPath, destPath + "\\" + DateTime.Now.ToString("yyyyMMdd"));
                DirectoryInfo newFolder = new DirectoryInfo(newpath);
                if (!newFolder.Exists)
                {
                    newFolder.Create();
                }
                NextFile.CopyTo(newpath + "\\" + NextFile.Name, true);
                SQLiteCommand cmdInsert = new SQLiteCommand(conn)
                {
                    CommandText = "insert into " + TB_NAME + " values(@fullPath, @lastWriteTime)"//设置带参SQL语句  
                };//实例化SQL命令  
                cmdInsert.Parameters.AddRange(new[] {//添加参数  
                           new SQLiteParameter("@fullPath", NextFile.FullName),
                           new SQLiteParameter("@lastWriteTime", NextFile.LastWriteTime)
                       });
                cmdInsert.ExecuteNonQuery();
                copyCount++;
            }
            else
            {
                DateTime lastWriteTime = DateTime.Parse(obj.ToString());
                if (!DateTime.Parse(NextFile.LastWriteTime.ToString()).Equals(lastWriteTime))
                {
                    String fullname = folder.FullName;
                    string newpath = fullname.Replace(srcPath, destPath + "\\" + DateTime.Now.ToString("yyyyMMdd"));
                    DirectoryInfo newFolder = new DirectoryInfo(newpath);
                    if (!newFolder.Exists)
                    {
                        newFolder.Create();
                    }
                    NextFile.CopyTo(newpath + "\\" + NextFile.Name, true);
                    SQLiteCommand cmdUpdate = new SQLiteCommand(conn);//实例化SQL命令  
                    cmdUpdate.CommandText = "update " + TB_NAME + " set lastWriteTime=@lastWriteTime where fullPath=@fullPath";
                    cmdUpdate.Parameters.AddRange(new[] {//添加参数  
                           new SQLiteParameter("@fullPath", NextFile.FullName),
                           new SQLiteParameter("@lastWriteTime", NextFile.LastWriteTime)
                       });
                    cmdUpdate.ExecuteNonQuery();

                    copyCount++;
                }
            }

            Console.WriteLine("已遍历第" + (++fileCount) + "个文件");
        }
    }
}