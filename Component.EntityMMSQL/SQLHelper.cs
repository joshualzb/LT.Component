using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace LT.Component.EntityMSSQL
{
    /// <summary>
    /// SQLHelper 通用类
    /// </summary>
    public sealed class SQLHelper
    {
        public static DataSet DataSet(string connection, string sqlText, CommandType commandType, DbParameter[] parms)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                SqlCommand sqlCommand = new SqlCommand();
                PrepareCommand(conn, null, sqlCommand, sqlText, commandType, parms);

                DataSet ds = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(sqlText, conn);
                adapter.SelectCommand = sqlCommand;
                adapter.SelectCommand.CommandTimeout = 60;

                adapter.Fill(ds);
                adapter.Dispose();

                conn.Close();

                return ds;
            }
        }

        public static SqlDataReader DataReader(string connection, string sqlText, CommandType commandType, DbParameter[] parms)
        {
            SqlConnection conn = new SqlConnection(connection);

            SqlCommand sqlCommand = new SqlCommand();
            PrepareCommand(conn, null, sqlCommand, sqlText, commandType, parms);

            SqlDataReader reader = sqlCommand.ExecuteReader(CommandBehavior.CloseConnection);

            sqlCommand.Parameters.Clear();
            sqlCommand.Dispose();

            return reader;
        }

        public static SqlDataReader DataReader(SqlConnection connection, SqlTransaction trans, string sqlText, CommandType commandType, DbParameter[] parms)
        {
            SqlCommand sqlCommand = new SqlCommand();
            PrepareCommand(connection, trans, sqlCommand, sqlText, commandType, parms);

            SqlDataReader reader = sqlCommand.ExecuteReader(CommandBehavior.CloseConnection);

            sqlCommand.Parameters.Clear();
            sqlCommand.Dispose();

            return reader;
        }

        public static int NonQuery(string connection, string sqlText, CommandType commandType, DbParameter[] parms)
        {
            int reVal = 0;
            using (SqlConnection conn = new SqlConnection(connection))
            {
                SqlCommand sqlCommand = new SqlCommand();
                PrepareCommand(conn, null, sqlCommand, sqlText, commandType, parms);

                reVal = sqlCommand.ExecuteNonQuery();

                sqlCommand.Parameters.Clear();
                sqlCommand.Dispose();
            }

            return reVal;
        }

        public static int NonQuery(SqlConnection connection, string sqlText, CommandType commandType, params DbParameter[] parms)
        {
            SqlCommand sqlCommand = new SqlCommand();
            PrepareCommand(connection, null, sqlCommand, sqlText, commandType, parms);
            int reVal = sqlCommand.ExecuteNonQuery();

            sqlCommand.Parameters.Clear();
            sqlCommand.Dispose();

            return reVal;
        }

        /// <summary>
        /// Prepare a command for execution
        /// </summary>
        /// <param name="cmd">SqlCommand object</param>
        /// <param name="conn">SqlConnection object</param>
        /// <param name="trans">SqlTransaction object</param>
        /// <param name="cmdType">Cmd type e.g. stored procedure or text</param>
        /// <param name="cmdText">Command text, e.g. Select * from Products</param>
        /// <param name="cmdParms">SqlParameters to use in the command</param>
        private static void PrepareCommand(SqlConnection connection, SqlTransaction trans, SqlCommand sqlCommand, string sqlText, CommandType commandType, params DbParameter[] parms)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            sqlCommand.Connection = connection;
            sqlCommand.CommandText = sqlText;
            sqlCommand.CommandType = commandType;
            sqlCommand.CommandTimeout = 60;

            if (trans != null)
            {
                sqlCommand.Transaction = trans;
            }

            if (parms != null)
            {
                foreach (DbParameter parm in parms)
                {
                    sqlCommand.Parameters.Add((SqlParameter)parm);
                }
            }
        }

        /// <summary>
        /// Maker MySql Parameters
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="Size"></param>
        /// <returns></returns>
        public static DbParameter MakeParam(string paramName, SqlDbType type, int size)
        {
            SqlParameter param;

            if (size > 0)
            {
                param = new SqlParameter(paramName, type, size);
            }
            else
            {
                param = new SqlParameter(paramName, type);
            }

            return param;
        }
    }
}
