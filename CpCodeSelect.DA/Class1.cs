using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpCodeSelect.DA
{
    public class SQLiteHelper
    {
        private SQLiteConnection dbConnection;

        public SQLiteHelper(string connectionString)
        {
            try
            {
                dbConnection = new SQLiteConnection(connectionString);
            }
            catch (Exception e)
            {
                // MessageBox.Show(e.ToString());
            }
        }

        /// <summary>
        /// 执行非查询SQL语句（创建表、插入、更新、删除）
        /// </summary>
        public void ExecuteNonQuery(string queryString)
        {
            try
            {
                dbConnection.Open(); // 打开连接
                using (SQLiteCommand dbCommand = new SQLiteCommand(queryString, dbConnection))
                {
                    dbCommand.ExecuteNonQuery(); // 执行命令
                }
            }
            catch (Exception e)
            {
                // MessageBox.Show(e.Message);
            }
            finally
            {
                if (dbConnection.State == ConnectionState.Open)
                    dbConnection.Close(); // 确保连接关闭
            }
        }

        /// <summary>
        /// 执行查询语句，返回DataTable
        /// </summary>
        public DataTable ExecuteQuery(string queryString)
        {
            DataTable dataTable = new DataTable();
            try
            {
                dbConnection.Open();
                using (SQLiteCommand dbCommand = new SQLiteCommand(queryString, dbConnection))
                {
                    using (SQLiteDataReader dataReader = dbCommand.ExecuteReader())
                    {
                        dataTable.Load(dataReader); // 将数据加载到DataTable
                    }
                }
            }
            catch (Exception e)
            {
                // MessageBox.Show(e.Message);
            }
            finally
            {
                if (dbConnection.State == ConnectionState.Open)
                    dbConnection.Close();
            }
            return dataTable;
        }
    }
}
