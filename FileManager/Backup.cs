﻿using System;
using System.Configuration;
using System.Data.SQLite;
using System.IO;
using DBUtility.SQLite;

class Bakuper
{
    private static string srcPath = ConfigurationManager.AppSettings["srcPath"];
    private static string destPath = ConfigurationManager.AppSettings["destPath"];
    private static int fileCount;
    private static int copyCount;
    private static SQLiteConnection conn;

    public static int BackupFile()
    {
        fileCount = 0;
        copyCount = 0;
        conn = SQLiteHelper.
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
            SQLiteCommand cmd = new SQLiteCommand("select lastWriteTime from " + SQLHelper.TB_NAME + " where fullPath='" + NextFile.FullName + "'", conn);
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
                SQLiteCommand cmdInsert = new SQLiteCommand(conn);//实例化SQL命令  
                cmdInsert.CommandText = "insert into " + SQLHelper.TB_NAME + " values(@fullPath, @lastWriteTime)";//设置带参SQL语句  
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
                    cmdUpdate.CommandText = "update " + SQLHelper.TB_NAME + " set lastWriteTime=@lastWriteTime where fullPath=@fullPath";
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