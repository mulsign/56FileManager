using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager
{
    class Log
    {
        static SQLiteHelper log = new SQLiteHelper();

        public static void InsertLog(string path, DateTime time, string op)
        {
            log.ConnectToDatabase(1);
            log.InsertLogs(path, time.ToString(), op);
            log.CloseDatabase(1);
        }
    }
}
