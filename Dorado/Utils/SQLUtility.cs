namespace Dorado.Utils
{
    using Dorado.Core;
    using Dorado.Core.Logger;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    /// <summary>
    /// 数据访问抽象基础类
    /// </summary>
    public class SQLUtility
    {
        private string connString;

        public SQLUtility(string conn)
        {
            connString = conn;
        }

        #region 公用方法

        /// <summary>
        /// 判断是否存在某表的某个字段
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="columnName">列名称</param>
        /// <returns>是否存在</returns>
        public bool IsExistsColumn(string tableName, string columnName)
        {
            string sql = "select count(1) from syscolumns where [id]=object_id('" + tableName + "') and [name]='" + columnName + "'";
            object res = GetSingle(sql);
            if (res == null)
            {
                return false;
            }
            return (int)res > 0;
        }

        public int GetMaxID(string fieldName, string tableName)
        {
            string strsql = "select max(" + fieldName + ")+1 from " + tableName;
            object obj = GetSingle(strsql);
            return obj == null ? 1 : (int)obj;
        }

        public bool IsExists(string strSql)
        {
            object obj = GetSingle(strSql);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = (int)obj;
            }
            return cmdresult == 0 ? false : true;
        }

        /// <summary>
        /// 表是否存在
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public bool IsExistsTable(string tableName)
        {
            string strsql = "select count(1) from sysobjects where id = object_id(N'[" + tableName + "]') and OBJECTPROPERTY(id, N'IsUserTable') = 1";

            //string strsql = "SELECT count(1) FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[" + tableName + "]') AND type in (N'U')";
            object obj = GetSingle(strsql);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = (int)obj;
            }
            return cmdresult == 0 ? false : true;
        }

        public bool IsExists(string strSql, params SqlParameter[] cmdParms)
        {
            object obj = GetSingle(strSql, cmdParms);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = (int)obj;
            }
            return cmdresult == 0 ? false : true;
        }

        public static string GetSafeSql(string rawString)
        {
            if (string.IsNullOrEmpty(rawString))
                return string.Empty;
            string returnValue = rawString;
            Dictionary<string, string> convertDict = new Dictionary<string, string>();
            convertDict.Add("'", "''");
            convertDict.Add("%", "[%]");
            convertDict.Add("[", "[[]");
            convertDict.Add("_", "[_]");
            convertDict.Add("^", "[^]");
            convertDict.Add("<", "[<]");
            convertDict.Add(">", "[>]");
            foreach (var v in convertDict)
            {
                returnValue = returnValue.Replace(v.Key, v.Value);
            }
            return returnValue;
        }

        public bool Test()
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                try
                {
                    connection.Open();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        #endregion 公用方法

        #region 执行简单SQL语句

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteSql(string sqlString)
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlString, connection))
                {
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (SqlException ex)
                    {
                        LoggerWrapper.Logger.Error(ex);
                        return 0;
                    }
                }
            }
        }

        public int ExecuteSqlByTime(string sqlString, int Times)
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlString, connection))
                {
                    try
                    {
                        connection.Open();
                        cmd.CommandTimeout = Times;
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (SqlException ex)
                    {
                        LoggerWrapper.Logger.Error(ex);
                        return 0;
                    }
                }
            }
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">多条SQL语句</param>
        public int ExecuteSqlTran(ICollection<String> sqlList)
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                connection.Open();
                using (SqlTransaction tx = connection.BeginTransaction())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connection;
                        cmd.Transaction = tx;
                        try
                        {
                            int count = 0;
                            foreach (string var in sqlList)
                            {
                                if (var.Trim().Length > 1)
                                {
                                    cmd.CommandText = var;
                                    count += cmd.ExecuteNonQuery();
                                }
                            }
                            tx.Commit();
                            return count;
                        }
                        catch (Exception ex)
                        {
                            LoggerWrapper.Logger.Error(ex);
                            tx.Rollback();
                            return 0;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 执行带一个存储过程参数的的SQL语句。
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <param name="content">参数内容,比如一个字段是格式复杂的文章，有特殊符号，可以通过这个方式添加</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteSql(string sqlString, string content)
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlString, connection))
                {
                    SqlParameter myParameter = new SqlParameter("@content", SqlDbType.NText);
                    myParameter.Value = content;
                    cmd.Parameters.Add(myParameter);
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (SqlException ex)
                    {
                        LoggerWrapper.Logger.Error(ex);
                        return 0;
                    }
                }
            }
        }

        /// <summary>
        /// 执行带一个存储过程参数的的SQL语句。
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <param name="content">参数内容,比如一个字段是格式复杂的文章，有特殊符号，可以通过这个方式添加</param>
        /// <returns>影响的记录数</returns>
        public object ExecuteSqlGet(string sqlString, string content)
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlString, connection))
                {
                    SqlParameter myParameter = new SqlParameter("@content", SqlDbType.NText);
                    myParameter.Value = content;
                    cmd.Parameters.Add(myParameter);
                    try
                    {
                        connection.Open();
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                            return null;
                        else
                            return obj;
                    }
                    catch (SqlException ex)
                    {
                        LoggerWrapper.Logger.Error(ex);
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// 向数据库里插入图像格式的字段(和上面情况类似的另一种实例)
        /// </summary>
        /// <param name="strSQL">SQL语句</param>
        /// <param name="fs">图像字节,数据库的字段类型为image的情况</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteSqlInsertImg(string strSQL, byte[] fs)
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand(strSQL, connection))
                {
                    SqlParameter myParameter = new SqlParameter("@fs", SqlDbType.Image);
                    myParameter.Value = fs;
                    cmd.Parameters.Add(myParameter);
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (SqlException ex)
                    {
                        LoggerWrapper.Logger.Error(ex);
                        return 0;
                    }
                }
            }
        }

        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public object GetSingle(string sqlString)
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlString, connection))
                {
                    try
                    {
                        connection.Open();
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                            return null;
                        else
                            return obj;
                    }
                    catch (SqlException ex)
                    {
                        LoggerWrapper.Logger.Error(ex);
                        return null;
                    }
                }
            }
        }

        public object GetSingle(string sqlString, int Times)
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlString, connection))
                {
                    try
                    {
                        connection.Open();
                        cmd.CommandTimeout = Times;
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                            return null;
                        else
                            return obj;
                    }
                    catch (SqlException ex)
                    {
                        LoggerWrapper.Logger.Error(ex);
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// 执行查询语句，返回SqlDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns>SqlDataReader</returns>
        public SqlDataReader ExecuteReader(string strSQL)
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand(strSQL, connection))
                {
                    try
                    {
                        connection.Open();
                        SqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        return myReader;
                    }
                    catch (SqlException ex)
                    {
                        LoggerWrapper.Logger.Error(ex);
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public DataSet Query(string sqlString)
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (SqlDataAdapter command = new SqlDataAdapter(sqlString, connection))
                {
                    using (DataSet ds = new DataSet())
                    {
                        try
                        {
                            connection.Open();
                            command.Fill(ds, "ds");
                        }
                        catch (SqlException ex)
                        {
                            LoggerWrapper.Logger.Error(ex);
                        }
                        return ds;
                    }
                }
            }
        }

        public DataSet Query(string sqlString, int Times)
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (SqlDataAdapter command = new SqlDataAdapter(sqlString, connection))
                {
                    using (DataSet ds = new DataSet())
                    {
                        try
                        {
                            connection.Open();
                            command.SelectCommand.CommandTimeout = Times;
                            command.Fill(ds, "ds");
                        }
                        catch (SqlException ex)
                        {
                            LoggerWrapper.Logger.Error(ex);
                        }
                        return ds;
                    }
                }
            }
        }

        #endregion 执行简单SQL语句

        #region 执行带参数的SQL语句

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteSql(string sqlString, params SqlParameter[] cmdParms)
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, sqlString, cmdParms);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        return rows;
                    }
                    catch (SqlException ex)
                    {
                        LoggerWrapper.Logger.Error(ex);
                        return 0;
                    }
                }
            }
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的SqlParameter[]）</param>
        public void ExecuteSqlTran(Hashtable SQLStringList)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        try
                        {
                            //循环
                            foreach (DictionaryEntry myDE in SQLStringList)
                            {
                                string cmdText = myDE.Key.ToString();
                                SqlParameter[] cmdParms = (SqlParameter[])myDE.Value;
                                PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                                int val = cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                            }
                            trans.Commit();
                        }
                        catch
                        {
                            trans.Rollback();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的SqlParameter[]）</param>
        public int ExecuteSqlTran(ICollection<CommandInfo> cmdList)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        try
                        {
                            int count = 0;

                            //循环
                            foreach (CommandInfo myDE in cmdList)
                            {
                                string cmdText = myDE.CommandText;
                                SqlParameter[] cmdParms = (SqlParameter[])myDE.Parameters;
                                PrepareCommand(cmd, conn, trans, cmdText, cmdParms);

                                if (myDE.EffentNextType == EffentNextType.WhenHaveContine || myDE.EffentNextType == EffentNextType.WhenNoHaveContine)
                                {
                                    if (myDE.CommandText.ToLower().IndexOf("count(") == -1)
                                    {
                                        trans.Rollback();
                                        return 0;
                                    }
                                    object obj = cmd.ExecuteScalar();
                                    bool isHave = false;
                                    if (obj == null && obj == DBNull.Value)
                                    {
                                        isHave = false;
                                    }
                                    isHave = Convert.ToInt32(obj) > 0;

                                    if (myDE.EffentNextType == EffentNextType.WhenHaveContine && !isHave)
                                    {
                                        trans.Rollback();
                                        return 0;
                                    }
                                    if (myDE.EffentNextType == EffentNextType.WhenNoHaveContine && isHave)
                                    {
                                        trans.Rollback();
                                        return 0;
                                    }
                                    continue;
                                }
                                int val = cmd.ExecuteNonQuery();
                                count += val;
                                if (myDE.EffentNextType == EffentNextType.ExcuteEffectRows && val == 0)
                                {
                                    trans.Rollback();
                                    return 0;
                                }
                                cmd.Parameters.Clear();
                            }
                            trans.Commit();
                            return count;
                        }
                        catch (Exception ex)
                        {
                            LoggerWrapper.Logger.Error(ex);
                            trans.Rollback();
                            return 0;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的SqlParameter[]）</param>
        public void ExecuteSqlTranWithIndentity(ICollection<CommandInfo> SQLStringList)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        try
                        {
                            int indentity = 0;

                            //循环
                            foreach (CommandInfo myDE in SQLStringList)
                            {
                                string cmdText = myDE.CommandText;
                                SqlParameter[] cmdParms = (SqlParameter[])myDE.Parameters;
                                foreach (SqlParameter q in cmdParms)
                                {
                                    if (q.Direction == ParameterDirection.InputOutput)
                                    {
                                        q.Value = indentity;
                                    }
                                }
                                PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                                int val = cmd.ExecuteNonQuery();
                                foreach (SqlParameter q in cmdParms)
                                {
                                    if (q.Direction == ParameterDirection.Output)
                                    {
                                        indentity = Convert.ToInt32(q.Value);
                                    }
                                }
                                cmd.Parameters.Clear();
                            }
                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            LoggerWrapper.Logger.Error(ex);
                            trans.Rollback();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的SqlParameter[]）</param>
        public void ExecuteSqlTranWithIndentity(Hashtable SQLStringList)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        try
                        {
                            int indentity = 0;

                            //循环
                            foreach (DictionaryEntry myDE in SQLStringList)
                            {
                                string cmdText = myDE.Key.ToString();
                                SqlParameter[] cmdParms = (SqlParameter[])myDE.Value;
                                foreach (SqlParameter q in cmdParms)
                                {
                                    if (q.Direction == ParameterDirection.InputOutput)
                                    {
                                        q.Value = indentity;
                                    }
                                }
                                PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                                int val = cmd.ExecuteNonQuery();
                                foreach (SqlParameter q in cmdParms)
                                {
                                    if (q.Direction == ParameterDirection.Output)
                                    {
                                        indentity = Convert.ToInt32(q.Value);
                                    }
                                }
                                cmd.Parameters.Clear();
                            }
                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            LoggerWrapper.Logger.Error(ex);
                            trans.Rollback();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public object GetSingle(string sqlString, params SqlParameter[] cmdParms)
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, sqlString, cmdParms);
                        object obj = cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                            return null;
                        else
                            return obj;
                    }
                    catch (SqlException ex)
                    {
                        LoggerWrapper.Logger.Error(ex);
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// 执行查询语句，返回SqlDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns>SqlDataReader</returns>
        public SqlDataReader ExecuteReader(string sqlString, params SqlParameter[] cmdParms)
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, sqlString, cmdParms);
                        SqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        cmd.Parameters.Clear();
                        return myReader;
                    }
                    catch (SqlException ex)
                    {
                        LoggerWrapper.Logger.Error(ex);
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public DataSet Query(string sqlString, params SqlParameter[] cmdParms)
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    PrepareCommand(cmd, connection, null, sqlString, cmdParms);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        using (DataSet ds = new DataSet())
                        {
                            try
                            {
                                da.Fill(ds, "ds");
                                cmd.Parameters.Clear();
                            }
                            catch (SqlException ex)
                            {
                                LoggerWrapper.Logger.Error(ex);
                            }
                            return ds;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Prepares the command.
        /// </summary>
        /// <param name="cmd">The CMD.</param>
        /// <param name="conn">The conn.</param>
        /// <param name="trans">The trans.</param>
        /// <param name="cmdText">The CMD text.</param>
        /// <param name="cmdParms">The CMD parms.</param>
        /// <Author>张江松(lietou1986@gmail.com) 2009-2-19</Author>
        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, string cmdText, SqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;//cmdType;
            if (cmdParms != null)
            {
                foreach (SqlParameter parameter in cmdParms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parameter);
                }
            }
        }

        #endregion 执行带参数的SQL语句

        #region 存储过程操作

        /// <summary>
        /// 执行存储过程，返回SqlDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SqlDataReader</returns>
        public SqlDataReader RunProcedure(IDataParameter[] parameters, string storedProcName)
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                SqlDataReader returnReader;
                connection.Open();
                SqlCommand command = BuildQueryCommand(connection, storedProcName, parameters);
                returnReader = command.ExecuteReader(CommandBehavior.CloseConnection);
                return returnReader;
            }
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="tableName">DataSet结果中的表名</param>
        /// <returns>DataSet</returns>
        public DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName)
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (SqlDataAdapter sqlDA = new SqlDataAdapter())
                {
                    using (DataSet ds = new DataSet())
                    {
                        connection.Open();
                        sqlDA.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
                        sqlDA.Fill(ds, tableName);
                        return ds;
                    }
                }
            }
        }

        /// <summary>
        /// Runs the procedure.
        /// </summary>
        /// <param name="storedProcName">Name of the stored proc.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="Times">The times.</param>
        /// <returns></returns>
        public DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName, int Times)
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (SqlDataAdapter sqlDA = new SqlDataAdapter())
                {
                    using (DataSet ds = new DataSet())
                    {
                        connection.Open();
                        sqlDA.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
                        sqlDA.SelectCommand.CommandTimeout = Times;
                        sqlDA.Fill(ds, tableName);
                        return ds;
                    }
                }
            }
        }

        /// <summary>
        /// 构建 SqlCommand 对象(用来返回一个结果集，而不是一个整数值)
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SqlCommand</returns>
        private static SqlCommand BuildQueryCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            using (SqlCommand command = new SqlCommand(storedProcName, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter parameter in parameters)
                {
                    if (parameter != null)
                    {
                        // 检查未分配值的输出参数,将其分配以DBNull.Value.
                        if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                            (parameter.Value == null))
                        {
                            parameter.Value = DBNull.Value;
                        }
                        command.Parameters.Add(parameter);
                    }
                }
                return command;
            }
        }

        /// <summary>
        /// 执行存储过程，返回影响的行数
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="rowsAffected">影响的行数</param>
        /// <returns></returns>
        public int RunProcedure(string storedProcName, IDataParameter[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                try
                {
                    int result;
                    connection.Open();
                    SqlCommand command = BuildIntCommand(connection, storedProcName, parameters);
                    result = Convert.ToInt32(command.ExecuteScalar());
                    return result;
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// 创建 SqlCommand 对象实例(用来返回一个整数值)
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SqlCommand 对象实例</returns>
        private static SqlCommand BuildIntCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            SqlCommand command = BuildQueryCommand(connection, storedProcName, parameters);
            command.Parameters.Add(new SqlParameter("ReturnValue",
                SqlDbType.Int, 4, ParameterDirection.ReturnValue,
                false, 0, 0, string.Empty, DataRowVersion.Default, null));
            return command;
        }

        #endregion 存储过程操作
    }

    public enum EffentNextType
    {
        /// <summary>
        /// 对其他语句无任何影响
        /// </summary>
        None,

        /// <summary>
        /// 当前语句必须为"select count(1) from .."格式，如果存在则继续执行，不存在回滚事务
        /// </summary>
        WhenHaveContine,

        /// <summary>
        /// 当前语句必须为"select count(1) from .."格式，如果不存在则继续执行，存在回滚事务
        /// </summary>
        WhenNoHaveContine,

        /// <summary>
        /// 当前语句影响到的行数必须大于0，否则回滚事务
        /// </summary>
        ExcuteEffectRows,

        /// <summary>
        /// 引发事件-当前语句必须为"select count(1) from .."格式，如果不存在则继续执行，存在回滚事务
        /// </summary>
        SolicitationEvent
    }

    public class CommandInfo
    {
        public object ShareObject = null;
        public object OriginalData = null;

        private event EventHandler _solicitationEvent;

        public event EventHandler SolicitationEvent
        {
            add
            {
                _solicitationEvent += value;
            }
            remove
            {
                _solicitationEvent -= value;
            }
        }

        public void OnSolicitationEvent()
        {
            if (_solicitationEvent != null)
            {
                _solicitationEvent(this, new EventArgs());
            }
        }

        public string CommandText;
        public System.Data.Common.DbParameter[] Parameters;
        public EffentNextType EffentNextType = EffentNextType.None;

        public CommandInfo()
        {
        }

        public CommandInfo(string sqlText, SqlParameter[] para)
        {
            this.CommandText = sqlText;
            this.Parameters = para;
        }

        public CommandInfo(string sqlText, SqlParameter[] para, EffentNextType type)
        {
            this.CommandText = sqlText;
            this.Parameters = para;
            this.EffentNextType = type;
        }
    }
}