﻿using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FileManager
{
    public static class  Setting
    {
        private static string settingFile = Environment.CurrentDirectory + "//" + "56settings.db";

        public static string SettingFile { get => settingFile; set => settingFile = value; }
        public static int Flag { get => flag; set => flag = value; }

        private static int flag = 0;

        ///   <summary>
        ///   给一个字符串进行MD5加密
        ///   </summary>
        ///   <param   name="strText">待加密字符串</param>
        ///   <returns>加密后的字符串</returns>
        public static string MD5Encrypt(string strText)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(Encoding.Default.GetBytes(strText));
            return Encoding.Default.GetString(result);
        }
        /// <summary> 隐藏文件夹 </summary>  
        /// <param name="path">文件名(包含路径)</param>  
        public static void HidDir(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            File.SetAttributes(path, dir.Attributes | FileAttributes.Hidden);
        }
        /// <summary> 设置文件隐藏 </summary>  
        /// <param name="path">文件名(包含路径)</param>  
        public static void HidFile(string path)
        {
            FileInfo fi = new FileInfo(path);
            File.SetAttributes(path, fi.Attributes | FileAttributes.Hidden);
        }
    }
}
