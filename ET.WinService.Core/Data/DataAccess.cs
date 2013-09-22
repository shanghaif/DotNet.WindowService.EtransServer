using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace ET.WinService.Core.Data
{
    public class DataAccess : IDisposable
    {
        private string _strConn;
        private bool _tranSucc = false;

        /// <summary>
        /// 使用默认方法构造数据库实例
        /// 必须在appSettings配制节点下配置defaultDatabase配置项
        /// </summary>
        public DataAccess()
        {
            SetConnectString("DataConnectionString");
        }

        /// <summary>
        /// 构造新实例。
        /// </summary>
        /// <param name="connStringName">连接串名字。即在config文件中connectionStrings节里连接串的name。</param>
        public DataAccess(string connStringName)
        {
            SetConnectString(connStringName);
        }

        void SetConnectString(string connStringName)
        {
            if (ConfigurationManager.ConnectionStrings[connStringName] != null)
                this._strConn = ConfigurationManager.ConnectionStrings[connStringName].ConnectionString;
            else
                this._strConn = ConfigurationManager.ConnectionStrings[ConfigurationManager.ConnectionStrings.Count - 1].ConnectionString;
        }


        ~DataAccess()
        {
            Dispose(false);
        }

        #region ExecuteScalar
        /// <summary>
        /// 执行SQL查询语句，并返回查询所返回的结果集中第一行的第一列。所有其他的列和行将被忽略。 
        /// </summary>
        /// <param name="commandText">SQL语句。</param>
        /// <returns>结果集中第一行的第一列。</returns>
        public object ExecuteScalar(string commandText)
        {
            return ExecuteScalar(CommandType.Text, commandText);
        }

        /// <summary>
        /// 执行带参数的SQL查询语句，并返回查询所返回的结果集中第一行的第一列。所有其他的列和行将被忽略。 
        /// </summary>
        /// <param name="commandText">SQL语句。</param>
        /// <param name="commandParameters">参数。</param>
        /// <returns>结果集中第一行的第一列。</returns>
        public object ExecuteScalar(string commandText, params SqlParameter[] commandParameters)
        {
            return ExecuteScalar(CommandType.Text, commandText, commandParameters);
        }

        /// <summary>
        /// 执行带参数的SQL查询语句或存储过程，并返回查询所返回的结果集中第一行的第一列。所有其他的列和行将被忽略。 
        /// </summary>
        /// <param name="commandType">查询语句的类型，SQL语句或是存储过程。</param>
        /// <param name="commandText">SQL语句或存储过程名字。</param>
        /// <param name="commandParameters">参数。</param>
        /// <returns>结果集中第一行的第一列。</returns>
        public object ExecuteScalar(CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (_trans == null)
                return SqlHelper.ExecuteScalar(this._strConn, commandType, commandText, commandParameters);
            else
                return SqlHelper.ExecuteScalar(_trans, commandType, commandText, commandParameters);
        }

        /// <summary>
        /// 执行SQL查询语句或存储过程，并返回查询所返回的结果集中第一行的第一列。所有其他的列和行将被忽略。 
        /// </summary>
        /// <param name="commandType">查询语句的类型，SQL语句或是存储过程。</param>
        /// <param name="commandText">SQL语句或存储过程名字。</param>
        /// <returns>结果集中第一行的第一列。</returns>
        public object ExecuteScalar(CommandType commandType, string commandText)
        {
            if (_trans == null)
                return SqlHelper.ExecuteScalar(this._strConn, commandType, commandText);
            else
                return SqlHelper.ExecuteScalar(_trans, commandType, commandText);
        }
        #endregion

        #region ExecuteNonQuery

        /// <summary>
        /// 执行SQL语句。 
        /// </summary>
        /// <param name="commandText">SQL语句。</param>
        /// <returns>受影响的行数。</returns>
        public int ExecuteNonQuery(string commandText)
        {
            return ExecuteNonQuery(CommandType.Text, commandText);
        }

        /// <summary>
        /// 执行带参数的SQL语句。 
        /// </summary>
        /// <param name="commandText">SQL语句。</param>
        /// <param name="commandParameters">参数。</param>
        /// <returns>受影响的行数。</returns>
        public int ExecuteNonQuery(string commandText, params SqlParameter[] commandParameters)
        {
            return ExecuteNonQuery(CommandType.Text, commandText, commandParameters);
        }

        /// <summary>
        /// 执行SQL语句或存储过程。 
        /// </summary>
        /// <param name="commandType">查询的类型，SQL语句或是存储过程。</param>
        /// <param name="commandText">SQL语句或存储过程名字。</param>
        /// <returns>受影响的行数。</returns>
        public int ExecuteNonQuery(CommandType commandType, string commandText)
        {
            if (_trans == null)
                return SqlHelper.ExecuteNonQuery(_strConn, commandType, commandText);
            else
                return SqlHelper.ExecuteNonQuery(_trans, commandType, commandText);
        }

        /// <summary>
        /// 执行SQL语句或存储过程。 
        /// </summary>
        /// <param name="commandType">查询的类型，SQL语句或是存储过程。</param>
        /// <param name="commandText">SQL语句或存储过程名字。</param>
        /// <returns>受影响的行数。</returns>
        public int ExecuteNonQuery(SqlTransaction trans, CommandType cmdType, string commandText, params SqlParameter[] commandParameters)
        {
            return SqlHelper.ExecuteNonQuery(trans, cmdType, commandText, commandParameters);
        }

        /// <summary>
        /// 执行带参数的SQL语句或存储过程。 
        /// </summary>
        /// <param name="commandType">查询的类型，SQL语句或是存储过程。</param>
        /// <param name="commandText">SQL语句或存储过程名字。</param>
        /// <param name="commandParameters">参数。</param>
        /// <returns>受影响的行数。</returns>
        public int ExecuteNonQuery(CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (_trans == null)
                return SqlHelper.ExecuteNonQuery(_strConn, commandType, commandText, commandParameters);
            else
                return SqlHelper.ExecuteNonQuery(_trans, commandType, commandText, commandParameters);
        }
        #endregion

        #region ExecuteReader

        /// <summary>
        /// 执行SQL查询语句，并返回 SqlDataReader。 
        /// </summary>
        /// <param name="commandText">查询SQL语句。</param>
        /// <returns>一个 SqlDataReader 对象。</returns>
        public SqlDataReader ExecuteReader(string commandText)
        {
            return ExecuteReader(CommandType.Text, commandText);
        }

        /// <summary>
        /// 执行带参数的SQL查询语句，并返回 SqlDataReader结果。 
        /// </summary>
        /// <param name="commandText">查询SQL语句。</param>
        /// <param name="commandParameters">参数。</param>
        /// <returns>一个 SqlDataReader 对象。</returns>
        public SqlDataReader ExecuteReader(string commandText, params SqlParameter[] commandParameters)
        {
            return ExecuteReader(CommandType.Text, commandText, commandParameters);
        }

        /// <summary>
        /// 执行SQL查询语句或存储过程，并返回 SqlDataReader结果。 
        /// </summary>
        /// <param name="commandType">查询语句的类型，SQL语句或是存储过程。</param>
        /// <param name="commandText">查询SQL语句或是存储过程名字。</param>
        /// <returns>一个 SqlDataReader 对象。</returns>
        public SqlDataReader ExecuteReader(CommandType commandType, string commandText)
        {
            if (_transConn == null)
            {
                _transConn = new SqlConnection(_strConn);
                _transConn.Open();

                return SqlHelper.ExecuteReader(_transConn, commandType, commandText);
            }
            else if (_trans != null)
            {
                return SqlHelper.ExecuteReader(_trans, commandType, commandText);
            }
            else
                return SqlHelper.ExecuteReader(_transConn, commandType, commandText);
        }

        /// <summary>
        /// 执行带参数的SQL查询语句或存储过程，并返回 SqlDataReader结果。 
        /// </summary>
        /// <param name="commandType">查询语句的类型，SQL语句或是存储过程。</param>
        /// <param name="commandText">SQL语句或存储过程名字。</param>
        /// <param name="commandParameters">参数。</param>
        /// <returns>一个 SqlDataReader 对象。</returns>
        public SqlDataReader ExecuteReader(CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (_transConn == null)
            {
                _transConn = new SqlConnection(_strConn);
                _transConn.Open();
                return SqlHelper.ExecuteReader(_transConn, commandType, commandText, commandParameters);
            }
            else if (_trans != null)
            {
                return SqlHelper.ExecuteReader(_trans, commandType, commandText, commandParameters);
            }
            else
                return SqlHelper.ExecuteReader(_transConn, commandType, commandText, commandParameters);

        }
        #endregion

        #region ExecuteDataSet

        /// <summary>
        /// 执行SQL查询语句，并返回结果 DataSet。 
        /// </summary>
        /// <param name="commandText">查询SQL语句。</param>
        /// <returns>一个 DataSet 对象。</returns>
        public DataSet ExecuteDataSet(string commandText)
        {
            return ExecuteDataSet(CommandType.Text, commandText);
        }

        /// <summary>
        /// 执行带参数的SQL查询语句，并返回结果 DataSet。 
        /// </summary>
        /// <param name="commandText">查询SQL语句。</param>
        /// <param name="commandParameters">参数。</param>
        /// <returns>一个 DataSet 对象。</returns>
        public DataSet ExecuteDataSet(string commandText, params SqlParameter[] commandParameters)
        {
            return ExecuteDataSet(CommandType.Text, commandText, commandParameters);
        }

        /// <summary>
        /// 执行带参数的SQL查询语句，并返回结果 DataSet。 
        /// </summary>
        /// <param name="commandText">查询SQL语句。</param>
        /// <param name="commandParameters">参数。</param>
        /// <returns>一个 DataTable 对象。</returns>
        public DataTable ExecuteDataTable(string commandText, params SqlParameter[] commandParameters)
        {
            DataSet ds = ExecuteDataSet(CommandType.Text, commandText, commandParameters);

            return ds.Tables[0] ?? null;
        }

        /// <summary>
        /// 执行SQL查询语句或存储过程，并返回结果 DataSet。 
        /// </summary>
        /// <param name="commandType">查询语句的类型，SQL语句或是存储过程。</param>
        /// <param name="commandText">查询SQL语句或是存储过程名字。</param>
        /// <returns>一个 DataSet 对象。</returns>
        public DataSet ExecuteDataSet(CommandType commandType, string commandText)
        {
            if (_trans == null)
                return SqlHelper.ExecuteDataset(_strConn, commandType, commandText);
            else
                return SqlHelper.ExecuteDataset(_trans, commandType, commandText);
        }

        /// <summary>
        /// 执行带参数的SQL查询语句或存储过程，并返回结果 DataSet。 
        /// </summary>
        /// <param name="commandType">查询语句的类型，SQL语句或是存储过程。</param>
        /// <param name="commandText">SQL语句或存储过程名字。</param>
        /// <param name="commandParameters">参数。</param>
        /// <returns>一个 DataSet 对象。</returns>
        public DataSet ExecuteDataSet(CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (_trans == null)
                return SqlHelper.ExecuteDataset(_strConn, commandType, commandText, commandParameters);
            else
                return SqlHelper.ExecuteDataset(_trans, commandType, commandText, commandParameters);

        }
        #endregion

        #region Transaction
        private SqlTransaction _trans = null;
        private SqlConnection _transConn = null;

        /// <summary>
        /// 事务是否成功的标记。一般在成功操作之后设为true，然后调用EndTransaction()。
        /// </summary>
        public bool TransSuccess
        {
            get { return _tranSucc; }
            set { _tranSucc = value; }
        }

        /// <summary>
        /// 开始一个事务。
        /// </summary>
        public SqlTransaction BeginTransaction()
        {
            if (_transConn == null)
            {
                _transConn = new SqlConnection(_strConn);
                _transConn.Open();
            }
            if (_trans == null)
            {
                _trans = _transConn.BeginTransaction();
            }

            return _trans;
        }


        /// <summary>
        /// 事务结束。根据事务是否成功的参数自动提交或回滚事务。
        /// </summary>
        /// <param name="isTranSucc">事务是否成功</param>
        public void EndTransaction(bool isTranSucc)
        {
            if (_trans == null && _transConn == null)
                return;

            _tranSucc = isTranSucc;
            EndTransaction();

        }

        /// <summary>
        /// 事务结束。根据TransSuccess属性的值决定提交或回滚事务。
        /// </summary>
        public void EndTransaction()
        {
            if (_trans == null && _transConn == null)
                return;

            try
            {
                if (_trans != null)
                {
                    if (_tranSucc)
                        _trans.Commit();
                    else
                        _trans.Rollback();
                }

            }
            finally
            {
                if (_trans != null)
                {
                    _trans.Dispose();
                    _trans = null;
                }
                if (_transConn != null)
                {
                    _transConn.Close();
                    _transConn.Dispose();
                    _transConn = null;
                }
            }
        }
        #endregion

        #region 实现IDisposable

        private bool _disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);

        }

        /// <summary>
        /// 是否释放资源
        /// </summary>
        /// <param name="disposing">释放资源标志</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    //cleanup mamanged objects
                    EndTransaction();
                }
                //Cleanup unmamanged objects
            }
            _disposed = true;

        }

        #endregion

    }
}
