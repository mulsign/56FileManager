using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace DBUtility.SQLite
{
    public interface ISQLiteHelper
    {
        int ExecuteNonQuery(string sql, params SQLiteParameter[] parameters);
        void ExecuteNonQueryBatch(List<KeyValuePair<string, SQLiteParameter[]>> list);
        DataTable ExecuteQuery(string sql, params SQLiteParameter[] parameters);
        SQLiteDataReader ExecuteReader(string sql, params SQLiteParameter[] parameters);
        object ExecuteScalar(string sql, params SQLiteParameter[] parameters);
        DataTable GetSchema();
    }
}