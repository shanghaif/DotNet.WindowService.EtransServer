using System;
using System.Collections.Generic;
using System.Data;

using System.Data.SqlClient;
using System.Configuration;

namespace ET.WinService.AreaStatistics.common
{
    public class SqlServerHelper
    {
        private static string connectionString = string.Empty;
        static SqlServerHelper()
        {

            connectionString = ConfigurationManager.ConnectionStrings["OtherConnectionString"].ToString();
        }

        #region AddInParameter 添加输入参数
        /// <summary>
        /// 添加In参数
        /// </summary>
        /// <param name="paramName">参数名</param>
        /// <param name="value">值</param>
        /// <returns>返回一个SqlParameter对象</returns>
        public static SqlParameter AddInParameter(string paramName, object value)
        {
            SqlParameter param = new SqlParameter(paramName, value);
            return param;
        }

        /// <summary>
        /// 添加In参数
        /// </summary>
        /// <param name="paramName">参数名</param>
        /// <param name="dbType">数据类型</param>
        /// <param name="value">值</param>
        /// <returns>返回一个SqlParameter对象</returns>
        public static SqlParameter AddInParameter(string paramName, SqlDbType dbType, object value)
        {
            return AddInParameter(paramName, dbType, 0, value);
        }
        /// <summary>
        /// 添加In参数
        /// </summary>
        /// <param name="paramName">参数名</param>
        /// <param name="dbType">数据类型</param>
        /// <param name="size">字段大小</param>
        /// <param name="value">值</param>
        /// <returns>返回一个SqlParameter对象</returns>
        public static SqlParameter AddInParameter(string paramName, SqlDbType dbType, int size, object value)
        {
            SqlParameter param;
            if (size > 0)
                param = new SqlParameter(paramName, dbType, size);
            else
                param = new SqlParameter(paramName, dbType);
            param.Value = value;

            return param;
        }
        #endregion

        #region AddOutParameter 添加输出参数
        /// <summary>
        /// 添加Out参数
        /// </summary>
        /// <param name="paramName">参数名</param>
        /// <param name="dbType">数据类型</param>
        /// <returns>返回一个SqlParameter对象</returns>
        public static SqlParameter AddOutParameter(string paramName, SqlDbType dbType)
        {
            return AddOutParameter(paramName, dbType, 0, null);
        }

        /// <summary>
        /// 添加Out参数
        /// </summary>
        /// <param name="paramName">参数名</param>
        /// <param name="dbType">数据类型</param>
        /// <param name="size">字段大小</param>
        /// <returns>返回一个SqlParameter对象</returns>
        public static SqlParameter AddOutParameter(string paramName, SqlDbType dbType, int size)
        {
            return AddOutParameter(paramName, dbType, size, null);
        }
        public static SqlParameter AddOutParameter(string paramName, SqlDbType dbType, int size, object value)
        {
            SqlParameter param;
            if (size > 0)
            {
                param = new SqlParameter(paramName, dbType, size);
            }
            else
            {
                param = new SqlParameter(paramName, dbType);
            }
            if (value != null)
            {
                param.Value = value;
            }
            param.Direction = ParameterDirection.Output;

            return param;
        }
        #endregion

        #region PrepareCommand
        private static void PrepareCommand(SqlConnection conn, SqlCommand cmd, CommandType cmdType, string cmdText, int timeout, SqlParameter[] cmdParms)
        {
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            cmd.CommandType = cmdType;
            if (timeout > 30) cmd.CommandTimeout = timeout;
            if (cmdParms != null)
            {
                foreach (SqlParameter parm in cmdParms)
                {
                    if ((parm.Direction == ParameterDirection.InputOutput || parm.Direction == ParameterDirection.Input) && (parm.Value == null))
                    {
                        parm.Value = DBNull.Value;
                    }

                    cmd.Parameters.Add(parm);
                }
            }
            if (conn.State != ConnectionState.Open)
            {
                try
                {
                    conn.Open();
                }
                catch { }
            }

        }
        private static void PrepareCommand(SqlConnection conn, SqlCommand cmd, CommandType cmdType, string cmdText, int timeout, List<SqlParameter> cmdParms)
        {
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            cmd.CommandType = cmdType;
            if (timeout > 30) cmd.CommandTimeout = timeout;
            if (cmdParms != null)
            {
                foreach (SqlParameter parm in cmdParms)
                {
                    if ((parm.Direction == ParameterDirection.InputOutput || parm.Direction == ParameterDirection.Input) && (parm.Value == null))
                    {
                        parm.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parm);
                }
            }
            if (conn.State != ConnectionState.Open)
            {
                try
                {
                    conn.Open();
                }
                catch { }
            }

        }
        #endregion

        #region ConnClose 关闭数据库连接
        private static void ConnClose(SqlConnection conn)
        {
            if (conn != null && conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
        }
        #endregion

        #region 返回DataTable,DataSet,执行更新

        /// <summary>
        /// 标量查询，返回查询结果集中第一行的第一列。
        /// </summary>
        /// <param name="cmdType">该值指示如何解释 CommandText 属性</param>
        /// <param name="cmdText">Transact-SQL 语句或存储过程。</param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="cmdParms">参数列表，params变长数组的形式</param>
        /// <returns>返回值</returns>
        public static object ExecuteScalar(string cmdText, CommandType cmdType, int timeOut, params  SqlParameter[] cmdParms)
        {
            using (SqlConnection myConn = new SqlConnection(connectionString))
            {
                using (SqlCommand myCmd = new SqlCommand())
                {
                    object retval = null;
                    try
                    {
                        PrepareCommand(myConn, myCmd, cmdType, cmdText, timeOut, cmdParms);
                        retval = myCmd.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {

                        return null;
                    }
                    finally
                    {
                        if (cmdParms != null)
                        {
                            myCmd.Parameters.Clear();
                        }
                        myCmd.Dispose();
                        ConnClose(myConn);
                    }
                    return retval;
                }
            }
        }
        /// <summary>
        /// 创建DataTable
        /// </summary>
        /// <param name="cmdText">Transact-SQL 语句或存储过程。</param>
        /// <param name="cmdType">该值指示如何解释 CommandText 属性</param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="cmdParms">参数列表，params变长数组的形式</param>
        /// <returns>DataTable 对象。</returns>
        public static DataTable ExecuteDataTable(string cmdText, CommandType cmdType, int timeOut, params  SqlParameter[] cmdParms)
        {
            using (SqlConnection myConn = new SqlConnection(connectionString))
            {
                using (SqlCommand myCmd = new SqlCommand())
                {
                    SqlDataAdapter myda = null;
                    DataTable dt = new DataTable();
                    try
                    {
                        PrepareCommand(myConn, myCmd, cmdType, cmdText, timeOut, cmdParms);
                        myda = new SqlDataAdapter(myCmd);
                        myda.Fill(dt);
                    }
                    catch (Exception ex)
                    {


                        return new DataTable();
                    }
                    finally
                    {
                        if (cmdParms != null)
                        {
                            myCmd.Parameters.Clear();
                        }
                        myCmd.Dispose();
                        myda.Dispose();
                        ConnClose(myConn);
                    }
                    return dt;
                }
            }
        }
        /// <summary>
        /// 创建DataSet
        /// </summary>
        /// <param name="cmdType">该值指示如何解释 CommandText 属性</param>
        /// <param name="cmdText">Transact-SQL 语句或存储过程。</param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="cmdParms">参数列表，params变长数组的形式</param>
        /// <returns>DataSet 对象。</returns>
        public static DataSet ExecuteDataSet(string cmdText, CommandType cmdType, int timeOut, params  SqlParameter[] cmdParms)
        {
            using (SqlConnection myConn = new SqlConnection(connectionString))
            {
                using (SqlCommand myCmd = new SqlCommand())
                {
                    SqlDataAdapter myda = null;
                    DataSet ds = new DataSet();
                    try
                    {
                        PrepareCommand(myConn, myCmd, cmdType, cmdText, timeOut, cmdParms);
                        myda = new SqlDataAdapter(myCmd);
                        myda.Fill(ds);
                    }
                    catch (Exception ex)
                    {

                        return new DataSet();
                    }
                    finally
                    {
                        if (cmdParms != null)
                        {
                            myCmd.Parameters.Clear();
                        }
                        myCmd.Dispose();
                        myda.Dispose();
                        ConnClose(myConn);
                    }
                    return ds;
                }
            }
        }
        /// <summary>
        /// 创建DataTable
        /// </summary>
        /// <param name="cmdText">Transact-SQL 语句或存储过程。</param>
        /// <param name="cmdType">该值指示如何解释 CommandText 属性</param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="cmdParms">参数列表，params变长数组的形式</param>
        /// <returns>DataTable 对象。</returns>
        public static DataTable ExecuteDataTable(string cmdText, CommandType cmdType, int timeOut, List<SqlParameter> cmdParms)
        {
            using (SqlConnection myConn = new SqlConnection(connectionString))
            {
                using (SqlCommand myCmd = new SqlCommand())
                {
                    SqlDataAdapter myda = null;
                    DataTable dt = new DataTable();
                    try
                    {
                        PrepareCommand(myConn, myCmd, cmdType, cmdText, timeOut, cmdParms);
                        myda = new SqlDataAdapter(myCmd);
                        myda.Fill(dt);
                    }
                    catch (Exception ex)
                    {

                        return new DataTable();
                    }
                    finally
                    {
                        if (cmdParms != null)
                        {
                            myCmd.Parameters.Clear();
                        }
                        myCmd.Dispose();
                        myda.Dispose();
                        ConnClose(myConn);
                    }
                    return dt;
                }
            }
        }

        #endregion

        #region 快捷方法

        #region ExecuteDataTable 创建DataTable
        public static DataTable ExecuteDataTable(string cmdText)
        {
            return ExecuteDataTable(cmdText, CommandType.Text, 30, new List<SqlParameter>());
        }
        public static DataTable ExecuteDataTable(string cmdText, List<SqlParameter> cmdParms)
        {
            return ExecuteDataTable(cmdText, CommandType.Text, 30, cmdParms);
        }

        /// <summary>
        /// 创建DataTable
        /// </summary>
        /// <param name="cmdText">Transact-SQL 语句或存储过程</param>
        /// <param name="cmdParms">参数列表，params变长数组的形式</param>
        /// <returns>DataTable 对象。</returns>
        public static DataTable ExecuteDataTable(string cmdText, params SqlParameter[] cmdParms)
        {
            return ExecuteDataTable(cmdText, CommandType.Text, 60, cmdParms);
        }

        /// <summary>
        /// 创建DataTable
        /// </summary>
        /// <param name="cmdText">Transact-SQL 语句或存储过程</param>
        /// <param name="cmdType">该值指示如何解释 CommandText 属性</param>
        /// <param name="cmdParms">参数列表，params变长数组的形式</param>
        /// <returns> DataTable 对象。</returns>
        public static DataTable ExecuteDataTable(string cmdText, CommandType cmdType, List<SqlParameter> cmdParms)
        {
            return ExecuteDataTable(cmdText, cmdType, 60, cmdParms);
        }
        /// <summary>
        /// 创建DataTable
        /// </summary>
        /// <param name="cmdText">Transact-SQL 语句或存储过程</param>
        /// <param name="cmdType">该值指示如何解释 CommandText 属性</param>
        /// <param name="cmdParms">参数列表，params变长数组的形式</param>
        /// <returns> DataTable 对象。</returns>
        public static DataTable ExecuteDataTable(string cmdText, CommandType cmdType, params SqlParameter[] cmdParms)
        {
            return ExecuteDataTable(cmdText, cmdType, 60, cmdParms);
        }


        /// <summary>
        /// 创建DataTable
        /// </summary>
        /// <param name="ConnectionString">数据库连接字符串</param>
        /// <param name="cmdText">Transact-SQL 语句或存储过程。</param>
        /// <param name="cmdParms">参数列表，params变长数组的形式</param>
        /// <returns>DataTable 对象。</returns>
        public static DataTable ExecuteDataTable(string ConnectionString, string cmdText, params SqlParameter[] cmdParms)
        {
            return ExecuteDataTable(ConnectionString, cmdText, CommandType.Text, cmdParms);
        }

        /// <summary>
        /// 创建DataTable
        /// </summary>
        /// <param name="ConnectionString">数据库连接字符串</param>
        /// <param name="cmdText">Transact-SQL 语句或存储过程。</param>
        /// <param name="cmdType">该值指示如何解释 CommandText 属性</param>
        /// <param name="cmdParms">参数列表，params变长数组的形式</param>
        /// <returns>DataTable 对象。</returns>
        public static DataTable ExecuteDataTable(string ConnectionString, string cmdText, CommandType cmdType, params SqlParameter[] cmdParms)
        {
            using (SqlConnection myConn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand myCmd = new SqlCommand())
                {
                    SqlDataAdapter myda = null;
                    DataTable dt = new DataTable();
                    try
                    {
                        PrepareCommand(myConn, myCmd, cmdType, cmdText, 60, cmdParms);
                        myda = new SqlDataAdapter(myCmd);
                        myda.Fill(dt);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        if (cmdParms != null)
                        {
                            myCmd.Parameters.Clear();
                        }
                        myCmd.Dispose();
                        myda.Dispose();
                        ConnClose(myConn);
                    }
                    return dt;
                }
            }
        }
        #endregion

        #region ExecuteDataRow 返回一行数据
        public static DataRow ExecuteDataRow(string cmdText)
        {
            return ExecuteDataRow(cmdText, CommandType.Text, 30, new List<SqlParameter>());
        }
        public static DataRow ExecuteDataRow(string cmdText, List<SqlParameter> cmdParms)
        {
            return ExecuteDataRow(cmdText, CommandType.Text, 30, cmdParms);
        }

        /// <summary>
        /// DataRow
        /// </summary>
        /// <param name="cmdText">Transact-SQL 语句或存储过程</param>
        /// <param name="cmdParms">参数列表，params变长数组的形式</param>
        /// <returns>DataTable 对象。</returns>
        public static DataRow ExecuteDataRow(string cmdText, params SqlParameter[] cmdParms)
        {
            return ExecuteDataRow(cmdText, CommandType.Text, 60, cmdParms);
        }
        /// <summary>
        /// 创建DataTable
        /// </summary>
        /// <param name="cmdText">Transact-SQL 语句或存储过程</param>
        /// <param name="cmdType">该值指示如何解释 CommandText 属性</param>
        /// <param name="cmdParms">参数列表，params变长数组的形式</param>
        /// <returns> DataTable 对象。</returns>
        public static DataRow ExecuteDataRow(string cmdText, CommandType cmdType, params SqlParameter[] cmdParms)
        {
            return ExecuteDataRow(cmdText, cmdType, 60, cmdParms);
        }


        /// <summary>
        /// DataRow
        /// </summary>
        /// <param name="ConnectionString">数据库连接字符串</param>
        /// <param name="cmdText">Transact-SQL 语句或存储过程。</param>
        /// <param name="cmdParms">参数列表，params变长数组的形式</param>
        /// <returns>DataTable 对象。</returns>
        public static DataRow ExecuteDataRow(string ConnectionString, string cmdText, params SqlParameter[] cmdParms)
        {
            return ExecuteDataRow(ConnectionString, cmdText, CommandType.Text, cmdParms);
        }

        /// <summary>
        /// DataRow
        /// </summary>
        /// <param name="ConnectionString">数据库连接字符串</param>
        /// <param name="cmdText">Transact-SQL 语句或存储过程。</param>
        /// <param name="cmdType">该值指示如何解释 CommandText 属性</param>
        /// <param name="cmdParms">参数列表，params变长数组的形式</param>
        /// <returns>DataTable 对象。</returns>
        public static DataRow ExecuteDataRow(string ConnectionString, string cmdText, CommandType cmdType, params SqlParameter[] cmdParms)
        {
            DataTable table = ExecuteDataTable(ConnectionString, cmdText, cmdType, cmdParms);
            if (table != null)
            {
                if (table.Rows.Count > 0)
                {
                    return table.Rows[0];
                }
            }
            return null;
        }

        /// <summary>
        /// 返回DataRow
        /// </summary>
        /// <param name="cmdText">Transact-SQL 语句或存储过程。</param>
        /// <param name="cmdType">该值指示如何解释 CommandText 属性</param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="cmdParms">参数列表，params变长数组的形式</param>
        /// <returns>DataTable 对象。</returns>
        public static DataRow ExecuteDataRow(string cmdText, CommandType cmdType, int timeOut, List<SqlParameter> cmdParms)
        {
            DataTable table = ExecuteDataTable(cmdText, cmdType, timeOut, cmdParms);
            if (table != null)
            {
                if (table.Rows.Count > 0)
                {
                    return table.Rows[0];
                }
            }
            return null;
        }
        /// <summary>
        /// DataRow
        /// </summary>
        /// <param name="cmdText">Transact-SQL 语句或存储过程。</param>
        /// <param name="cmdType">该值指示如何解释 CommandText 属性</param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="cmdParms">参数列表，params变长数组的形式</param>
        /// <returns>DataTable 对象。</returns>
        public static DataRow ExecuteDataRow(string cmdText, CommandType cmdType, int timeOut, params SqlParameter[] cmdParms)
        {
            DataTable table = ExecuteDataTable(cmdText, cmdType, timeOut, cmdParms);
            if (table != null)
            {
                if (table.Rows.Count > 0)
                {
                    return table.Rows[0];
                }
            }
            return null;
        }
        #endregion

        #region ExecuteNonQuery
        public static int ExecuteNonQueryStoredProcedure(string cmdText, params  SqlParameter[] cmdParms)
        {
            return ExecuteNonQuery(cmdText, CommandType.StoredProcedure, 30, cmdParms);
        }

        public static int ExecuteNonQuery(string cmdText, List<SqlParameter> cmdParms)
        {
            return ExecuteNonQuery(cmdText, CommandType.Text, 30, cmdParms);
        }
        public static int ExecuteNonQuery(string cmdText)
        {
            return ExecuteNonQuery(cmdText, CommandType.Text, 30, new List<SqlParameter>());
        }
        /// <summary>
        /// 对连接对象执行 SQL 语句。
        /// </summary>
        /// <param name="cmdText">Transact-SQL 语句或存储过程。</param>
        /// <param name="cmdParms">参数列表，params变长数组的形式</param>
        /// <returns>返回受影响的行数。</returns>
        public static int ExecuteNonQuery(string cmdText, params SqlParameter[] cmdParms)
        {
            return ExecuteNonQuery(cmdText, CommandType.Text, 60, cmdParms);
        }
        /// <summary>
        /// 对连接对象执行 SQL 语句。
        /// </summary>
        /// <param name="cmdType">该值指示如何解释 CommandText 属性</param>
        /// <param name="cmdText">Transact-SQL 语句或存储过程。</param>
        /// <param name="cmdParms">参数列表，params变长数组的形式</param>
        /// <returns>返回受影响的行数。</returns>
        public static int ExecuteNonQuery(string cmdText, CommandType cmdType, params SqlParameter[] cmdParms)
        {
            return ExecuteNonQuery(cmdText, cmdType, 60, cmdParms);
        }
        /// <summary>
        /// 对连接对象执行 SQL 语句。
        /// </summary>
        /// <param name="cmdType">该值指示如何解释 CommandText 属性</param>
        /// <param name="cmdText">Transact-SQL 语句或存储过程。</param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="cmdParms">参数列表，params变长数组的形式</param>
        /// <returns>返回受影响的行数。</returns>
        public static int ExecuteNonQuery(string cmdText, CommandType cmdType, int timeOut, List<SqlParameter> cmdParms)
        {
            using (SqlConnection myConn = new SqlConnection(connectionString))
            {
                using (SqlCommand myCmd = new SqlCommand())
                {
                    int retval = 0;
                    try
                    {
                        PrepareCommand(myConn, myCmd, cmdType, cmdText, timeOut, cmdParms);
                        retval = myCmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {

                        return 0;
                    }
                    finally
                    {
                        if (cmdParms != null)
                        {
                            myCmd.Parameters.Clear();
                        }
                        myCmd.Dispose();
                        ConnClose(myConn);
                    }
                    return retval;
                }
            }
        }
        /// <summary>
        /// 对连接对象执行 SQL 语句。
        /// </summary>
        /// <param name="cmdType">该值指示如何解释 CommandText 属性</param>
        /// <param name="cmdText">Transact-SQL 语句或存储过程。</param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="cmdParms">参数列表，params变长数组的形式</param>
        /// <returns>返回受影响的行数。</returns>
        public static int ExecuteNonQuery(string cmdText, CommandType cmdType, int timeOut, params  SqlParameter[] cmdParms)
        {
            using (SqlConnection myConn = new SqlConnection(connectionString))
            {
                using (SqlCommand myCmd = new SqlCommand())
                {
                    int retval = 0;
                    try
                    {
                        PrepareCommand(myConn, myCmd, cmdType, cmdText, timeOut, cmdParms);
                        retval = myCmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw ex;

                        return 0;
                    }
                    finally
                    {
                        if (cmdParms != null)
                        {
                            myCmd.Parameters.Clear();
                        }
                        myCmd.Dispose();
                        ConnClose(myConn);
                    }
                    return retval;
                }
            }
        }
        #endregion

        #region ExecuteDataSet 创建DataSet
        /// <summary>
        /// 创建DataSet
        /// </summary>
        /// <param name="cmdText">Transact-SQL 语句或存储过程。</param>
        /// <param name="cmdParms">参数列表，params变长数组的形式</param>
        /// <returns> DataSet 对象。</returns>
        public static DataSet ExecuteDataSet(string cmdText, params SqlParameter[] cmdParms)
        {
            return ExecuteDataSet(cmdText, CommandType.Text, 60, cmdParms);
        }

        /// <summary>
        /// 创建DataSet
        /// </summary>
        /// <param name="cmdType">该值指示如何解释 CommandText 属性</param>
        /// <param name="cmdText">设Transact-SQL 语句或存储过程。</param>
        /// <param name="cmdParms">参数列表，params变长数组的形式</param>
        /// <returns> DataSet 对象。</returns>
        public static DataSet ExecuteDataSet(string cmdText, CommandType cmdType, params SqlParameter[] cmdParms)
        {
            return ExecuteDataSet(cmdText, cmdType, 60, cmdParms);
        }
        #endregion

        #region ExecuteDataReader 创建SqlDataReader
        /// <summary>
        /// 创建 SqlDataReader。
        /// </summary>
        /// <param name="cmdText">Transact-SQL 语句或存储过程。</param>
        /// <param name="cmdParms">参数列表，params变长数组的形式</param>
        /// <returns> SqlDataReader 对象。</returns>
        public static SqlDataReader ExecuteDataReader(string cmdText, params SqlParameter[] cmdParms)
        {
            return ExecuteDataReader(cmdText, CommandType.Text, 60, cmdParms);
        }
        /// <summary>
        /// 创建 SqlDataReader。
        /// </summary>
        /// <param name="cmdType">该值指示如何解释 CommandText 属性</param>
        /// <param name="cmdText">Transact-SQL 语句或存储过程。</param>
        /// <param name="cmdParms">参数列表，params变长数组的形式</param>
        /// <returns>一个 SqlDataReader 对象。</returns>
        public static SqlDataReader ExecuteDataReader(string cmdText, CommandType cmdType, params SqlParameter[] cmdParms)
        {
            return ExecuteDataReader(cmdText, cmdType, 60, cmdParms);
        }
        /// <summary>
        /// 创建 SqlDataReader。
        /// </summary>
        /// <param name="cmdType">该值指示如何解释 CommandText 属性</param>
        /// <param name="cmdText">Transact-SQL 语句或存储过程。</param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="cmdParms">参数列表，params变长数组的形式</param>
        /// <returns>一个 SqlDataReader 对象。</returns>
        public static SqlDataReader ExecuteDataReader(string cmdText, CommandType cmdType, int timeOut, params SqlParameter[] cmdParms)
        {
            SqlConnection myConn = new SqlConnection(connectionString);
            SqlCommand myCmd = new SqlCommand();
            SqlDataReader dr = null;
            try
            {
                PrepareCommand(myConn, myCmd, cmdType, cmdText, timeOut, cmdParms);
                dr = myCmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                ConnClose(myConn);
                throw new Exception(ex.Message);
            }
            finally
            {
                if (cmdParms != null)
                {
                    myCmd.Parameters.Clear();
                }
            }
            return dr;
        }
        #endregion

        #region  ExecuteNonQueryTran 执行带事务的批量SQL语句
        public static int ExecuteNonQueryTranStoredProcedure(string cmdText, params  SqlParameter[] cmdParms)
        {
            return ExecuteNonQueryTran(cmdText, CommandType.StoredProcedure, 30, cmdParms);
        }

        public static int ExecuteNonQueryTran(string cmdText, List<SqlParameter> cmdParms)
        {
            return ExecuteNonQueryTran(cmdText, CommandType.Text, 30, cmdParms);
        }
        public static int ExecuteNonQueryTran(string cmdText)
        {
            return ExecuteNonQueryTran(cmdText, CommandType.Text, 30, new List<SqlParameter>());
        }
        /// <summary>
        /// 对连接对象执行 SQL 语句。
        /// </summary>
        /// <param name="cmdText">Transact-SQL 语句或存储过程。</param>
        /// <param name="cmdParms">参数列表，params变长数组的形式</param>
        /// <returns>返回受影响的行数。</returns>
        public static int ExecuteNonQueryTran(string cmdText, params SqlParameter[] cmdParms)
        {
            return ExecuteNonQueryTran(cmdText, CommandType.Text, 60, cmdParms);
        }
        /// <summary>
        /// 对连接对象执行 SQL 语句。
        /// </summary>
        /// <param name="cmdType">该值指示如何解释 CommandText 属性</param>
        /// <param name="cmdText">Transact-SQL 语句或存储过程。</param>
        /// <param name="cmdParms">参数列表，params变长数组的形式</param>
        /// <returns>返回受影响的行数。</returns>
        public static int ExecuteNonQueryTran(string cmdText, CommandType cmdType, params SqlParameter[] cmdParms)
        {
            return ExecuteNonQueryTran(cmdText, cmdType, 60, cmdParms);
        }
        /// <summary>
        /// 对连接对象执行 SQL 语句。
        /// </summary>
        /// <param name="cmdType">该值指示如何解释 CommandText 属性</param>
        /// <param name="cmdText">Transact-SQL 语句或存储过程。</param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="cmdParms">参数列表，params变长数组的形式</param>
        /// <returns>返回受影响的行数。</returns>
        public static int ExecuteNonQueryTran(string cmdText, CommandType cmdType, int timeOut, List<SqlParameter> cmdParms)
        {
            using (SqlConnection myConn = new SqlConnection(connectionString))
            {
                myConn.Open();
                SqlTransaction tran = myConn.BeginTransaction();
                using (SqlCommand myCmd = new SqlCommand())
                {
                    int retval = 0;
                    try
                    {
                        PrepareCommand(myConn, myCmd, cmdType, cmdText, timeOut, cmdParms);

                        myCmd.Transaction = tran;
                        retval = myCmd.ExecuteNonQuery();
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return 0;
                    }
                    finally
                    {
                        if (cmdParms != null)
                        {
                            myCmd.Parameters.Clear();
                        }
                        tran.Dispose();
                        myCmd.Dispose();
                        ConnClose(myConn);
                    }
                    return retval;
                }
            }
        }
        /// <summary>
        /// 对连接对象执行 SQL 语句。
        /// </summary>
        /// <param name="cmdType">该值指示如何解释 CommandText 属性</param>
        /// <param name="cmdText">Transact-SQL 语句或存储过程。</param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="cmdParms">参数列表，params变长数组的形式</param>
        /// <returns>返回受影响的行数。</returns>
        public static int ExecuteNonQueryTran(string cmdText, CommandType cmdType, int timeOut, params  SqlParameter[] cmdParms)
        {
            using (SqlConnection myConn = new SqlConnection(connectionString))
            {
                myConn.Open();
                SqlTransaction tran = myConn.BeginTransaction();
                using (SqlCommand myCmd = new SqlCommand())
                {
                    int retval = 0;
                    try
                    {
                        PrepareCommand(myConn, myCmd, cmdType, cmdText, timeOut, cmdParms);

                        myCmd.Transaction = tran;
                        retval = myCmd.ExecuteNonQuery();
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return 0;
                    }
                    finally
                    {
                        if (cmdParms != null)
                        {
                            myCmd.Parameters.Clear();
                        }
                        tran.Dispose();
                        myCmd.Dispose();
                        ConnClose(myConn);

                    }
                    return retval;
                }
            }
        }

        #endregion

        #region ExecuteScalar 执行标量查询
        /// <summary>
        /// 标量查询，返回查询结果集中第一行的第一列。
        /// </summary>
        /// <param name="cmdText">Transact-SQL 语句或存储过程。</param>
        /// <param name="cmdParms">参数列表，params变长数组的形式</param>
        /// <returns>返回值</returns>
        public static Object ExecuteScalar(string cmdText, params SqlParameter[] cmdParms)
        {
            return ExecuteScalar(cmdText, CommandType.Text, 60, cmdParms);
        }
        /// <summary>
        /// 标量查询，返回查询结果集中第一行的第一列。
        /// </summary>
        /// <param name="cmdText">Transact-SQL 语句或存储过程。</param>
        /// <param name="cmdParms">参数列表，params变长数组的形式</param>
        /// <returns>返回值</returns>
        public static int ExecuteScalar<Int32>(string cmdText, params SqlParameter[] cmdParms)
        {

            object obj = ExecuteScalar(cmdText, CommandType.Text, 60, cmdParms);
            int result = -1;
            if (obj != null)
            {

                int.TryParse(obj.ToString(), out result);
            }
            return result;
        }

        /// <summary>
        /// 标量查询，返回查询结果集中第一行的第一列。
        /// </summary>
        /// <param name="cmdType">该值指示如何解释 CommandText 属性</param>
        /// <param name="cmdText">Transact-SQL 语句或存储过程。</param>
        /// <param name="cmdParms">参数列表，params变长数组的形式</param>
        /// <returns>返回值</returns>
        public static Object ExecuteScalar(string cmdText, CommandType cmdType, params SqlParameter[] cmdParms)
        {
            return ExecuteScalar(cmdText, cmdType, 60, cmdParms);
        }
        #endregion

        #region ExecuteBulkInsert 批量插入数据
        public static int ExecuteBulkInsert(DataTable t)
        {
            try
            {
                SqlBulkCopy bulk = new SqlBulkCopy(connectionString);
                bulk.BatchSize = t.Rows.Count;
                bulk.DestinationTableName = t.TableName;
                bulk.WriteToServer(t);
                return t.Rows.Count;
            }
            catch
            {
                return 0;
            }
        }

        #endregion

        #region getMaxID
        public static int getMaxID(string fieldName, string tableName)
        {
            string sql = "select max(" + fieldName + ") from " + tableName;
            object obj = ExecuteScalar(sql);
            int maxId = -1;
            if (obj != null)
            {
                int.TryParse(obj.ToString(), out maxId);
            }
            return maxId;
        }
        #endregion

        #endregion
    }
}