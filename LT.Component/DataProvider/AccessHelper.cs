using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Web;

namespace LT.Component.DataProvider
{
    /// <summary>
    /// AccessHelper 通用类
    /// </summary>
    public abstract class AccessHelper
    {
        /// <summary>
        /// Database connection strings
        /// </summary>
        public static readonly string connectionString = "provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + HttpContext.Current.Request.PhysicalApplicationPath + ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;

        /// <summary>
        /// 通过连接字符串获取连接对象
        /// </summary>
        /// <returns></returns>
        public static OleDbConnection GetSqlConnection()
        {
            return (new OleDbConnection(connectionString));
        }

        /// <summary>
        /// 通过指定的物理文件读取数据库（本网站目录下）
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static OleDbConnection GetSqlConnection(string file)
        {
            return (new OleDbConnection("provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + HttpContext.Current.Request.PhysicalApplicationPath + file));
        }

        /// <summary>
        /// 执行SQL语句，返回数据集
        /// </summary>
        /// <param name="sqlText"></param>
        /// <param name="cmdType"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        public static DataSet DataSet(string sqlText, OleDbParameter[] parms)
        {
            var ds = new DataSet();
            var sda = new OleDbDataAdapter(sqlText, connectionString);
            sda.SelectCommand.CommandType = CommandType.Text;
            if (parms != null)
            {
                foreach (OleDbParameter parm in parms)
                {
                    sda.SelectCommand.Parameters.Add(parm);
                }
            }
            sda.Fill(ds, "DataList");
            sda.SelectCommand.Parameters.Clear();
            sda.Dispose();

            return ds;
        }

        /// <summary>
        /// 执行SQL语句，返回数据集
        /// </summary>
        /// <param name="sqlText"></param>
        /// <param name="commandType"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        public static OleDbDataReader DataReader(string sqlText, OleDbParameter[] parms)
        {
            OleDbConnection conn = new OleDbConnection(connectionString);
            OleDbCommand sqlCommand = new OleDbCommand();
            PrepareCommand(conn, sqlCommand, sqlText, parms);

            OleDbDataReader reader = sqlCommand.ExecuteReader(CommandBehavior.CloseConnection);

            sqlCommand.Dispose();
            return reader;
        }

        /// <summary>
        /// 执行SQL语句，返回数据集（含有事务机制）
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <param name="sqlText"></param>
        /// <param name="commandType"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        public static OleDbDataReader DataReader(OleDbConnection conn, string sqlText, OleDbParameter[] parms)
        {
            OleDbCommand sqlCommand = new OleDbCommand();
            PrepareCommand(conn, sqlCommand, sqlText, parms);

            OleDbDataReader reader = sqlCommand.ExecuteReader(CommandBehavior.CloseConnection);

            sqlCommand.Dispose();
            return reader;
        }

        /// <summary>
        /// 执行SQL语句，并返回影响行数
        /// </summary>
        /// <param name="sqlText"></param>
        /// <param name="commandType"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        public static int NonQuery(string sqlText, OleDbParameter[] parms)
        {
            int reVal;
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                OleDbCommand sqlCommand = new OleDbCommand();
                PrepareCommand(conn, sqlCommand, sqlText, parms);

                reVal = sqlCommand.ExecuteNonQuery();

                sqlCommand.Parameters.Clear();
                sqlCommand.Dispose();
            }

            return reVal;
        }

        /// <summary>
        /// 执行SQL语句，并返回影响行数（含有事务机制）
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <param name="sqlText"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        public static int NonQuery(OleDbConnection conn, string sqlText, OleDbParameter[] parms)
        {
            OleDbCommand sqlCommand = new OleDbCommand();
            PrepareCommand(conn, sqlCommand, sqlText, parms);

            int reVal = sqlCommand.ExecuteNonQuery();

            sqlCommand.Parameters.Clear();
            sqlCommand.Dispose();

            return reVal;
        }

        /// <summary>
        /// 配置字符串参数
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <param name="sqlCommand"></param>
        /// <param name="sqlText"></param>
        /// <param name="parms"></param>
        private static void PrepareCommand(OleDbConnection conn, OleDbCommand sqlCommand, string sqlText, OleDbParameter[] parms)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            sqlCommand.Connection = conn;
            sqlCommand.CommandText = sqlText;
            sqlCommand.CommandType = CommandType.Text;

            if (parms != null)
            {
                foreach (OleDbParameter parm in parms)
                {
                    sqlCommand.Parameters.Add(parm);
                }
            }
        }
    }
}
