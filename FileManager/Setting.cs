using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FileManager
{
    class Setting
    {
        private static string settingFile = Environment.CurrentDirectory + "//" + "56settings";

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
    }
}
