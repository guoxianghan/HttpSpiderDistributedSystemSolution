using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using Validate;
//using WebServiceLibrary;

namespace CommonHelper
{
    /// <summary>
    ///
    /// </summary>
    public abstract class DbHelperSQL
    {
        protected static string connString = ConfigurationManager.ConnectionStrings["ConnectionString42"].ToString();
        public DbHelperSQL()
        {
        }

        //private static string connString = Encryption.Decode(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString());
        //private static string connString = WebService._Db.ConnectionString();

        static object _LockObj = new object();
        public static string ConnString
        {
            get
            {
                lock (_LockObj)
                {
                    return DbHelperSQL.connString;
                }
            }
            set
            {
                DbHelperSQL.connString = value;
            }
        }

        #region 公用方法

        public static int GetMaxID(string FieldName, string TableName, out string err)
        {
            string strsql = "select max(" + FieldName + ")+1 from " + TableName;
            object obj = GetSingle(strsql, out err);
            if (obj == null)
            {
                return 1;
            }
            else
            {
                return int.Parse(obj.ToString());
            }
        }
        /// <summary>
        /// 将表添加到数据库中
        /// </summary>
        /// <param name="strsql"></param>
        /// <param name="dt"></param>
        /// <param name="errinfo"></param>
        /// <returns></returns>
        public static bool InsertTable(string strsql, DataTable dt, out string errinfo)
        {
            errinfo = "";
            using (SqlConnection connection = new SqlConnection(connString))
            {
                connection.Open();
                SqlTransaction trans = connection.BeginTransaction();
                try
                {
                    SqlDataAdapter sqlDA1 = new SqlDataAdapter(strsql, connString);
                    SqlCommandBuilder sqlCB1 = new SqlCommandBuilder(sqlDA1);
                    sqlDA1.Fill(dt);
                    sqlDA1.Update(dt);

                    dt.AcceptChanges();
                    trans.Commit();
                    connection.Close();
                }
                catch (SqlException ex)
                {
                    trans.Rollback();
                    errinfo = ex.Message;
                    connection.Close();
                    connection.Dispose();
                    return false;
                }
                trans.Dispose();
            }

            return true;

        }
        /// <summary>
        /// 返回一个DataTable对象
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static DataTable GetDataTable(string sql, out string err)
        {
            DataTable dt = new DataTable();
            SqlDataReader dr = ExecuteReader(sql, out err);
            while (dr.Read())
            {
                dt.Load(dr, LoadOption.Upsert);

            }
            return dt;
        }

        public static DataTable GetTableColumns(string tablename, out string err)
        {
            DataTable dt = new DataTable();
            SqlDataReader dr = ExecuteReader("SELECT * FROM " + tablename + " WHERE 1=0", out err);
            dt.Load(dr, LoadOption.Upsert);
            return dt;
        }

        public static DataTable GetWebTypes(out string err)
        {
            DataTable dt = new DataTable();
            SqlDataReader dr = ExecuteReader("SELECT * FROM [WebType] WHERE Status=1", out err);
            dt.Load(dr, LoadOption.Upsert);
            return dt;
        }
        public static bool Exists(string strSql, out string err, params SqlParameter[] cmdParms)
        {
            object obj = GetSingle(strSql, out err, cmdParms);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region  执行简单SQL语句

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSql(string SQLString, out string err)
        {
            err = "";
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (System.Data.SqlClient.SqlException E)
                    {
                        connection.Close();
                        err = E.Message;
                        throw E;
                    }
                }
            }
        }

        public static bool ExecuteSqlTran<T>(string ProcName, out string err, params T[] t)
        {
            err = "";
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                SqlTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    for (int n = 0; n < t.Length; n++)
                    {
                        //RunProcedure(
                    }
                    tx.Commit();
                }
                catch (System.Data.SqlClient.SqlException E)
                {
                    tx.Rollback();
                    err = E.Message;
                    throw E;
                }
            }
            return true;
        }
        static SqlTransaction tx;
        /// <summary>
        /// 开始一个事务
        /// </summary>
        public static void BeginSqlTran()
        {
            SqlConnection conn = new SqlConnection(connString);

            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            tx = conn.BeginTransaction();
            cmd.Transaction = tx;
            tx.Rollback();
        }

        /// <summary>
        /// 提交一个事务
        /// </summary>
        public static void CommitSqlTran()
        {
            tx.Commit();

        }

        public static void RollBackSqlTran()
        {
            tx.Rollback();
        }
        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">多条SQL语句</param>		
        public static void ExecuteSqlTran(ArrayList SQLStringList, out string err)
        {
            err = "";
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                SqlTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    for (int n = 0; n < SQLStringList.Count; n++)
                    {
                        string strsql = SQLStringList[n].ToString();
                        if (strsql.Trim().Length > 1)
                        {
                            cmd.CommandText = strsql;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();
                }
                catch (System.Data.SqlClient.SqlException E)
                {
                    tx.Rollback();
                    conn.Close();
                    err = E.Message;
                }
            }
        }
        /// <summary>
        /// 执行带一个存储过程参数的的SQL语句。
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <param name="content">参数内容,比如一个字段是格式复杂的文章，有特殊符号，可以通过这个方式添加</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSql(string SQLString, string content, out string err)
        {
            err = "";
            using (SqlConnection connection = new SqlConnection(connString))
            {
                SqlCommand cmd = new SqlCommand(SQLString, connection);
                System.Data.SqlClient.SqlParameter myParameter = new System.Data.SqlClient.SqlParameter("@content", SqlDbType.NText);
                myParameter.Value = content;
                cmd.Parameters.Add(myParameter);
                try
                {
                    connection.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (System.Data.SqlClient.SqlException E)
                {
                    err = E.Message;
                    throw E;
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }
        /// <summary>
        /// 向数据库里插入图像格式的字段(和上面情况类似的另一种实例)
        /// </summary>
        /// <param name="strSQL">SQL语句</param>
        /// <param name="fs">图像字节,数据库的字段类型为image的情况</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSqlInsertImg(string strSQL, byte[] fs)
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                SqlCommand cmd = new SqlCommand(strSQL, connection);
                System.Data.SqlClient.SqlParameter myParameter = new System.Data.SqlClient.SqlParameter("@fs", SqlDbType.Image);
                myParameter.Value = fs;
                cmd.Parameters.Add(myParameter);
                try
                {
                    connection.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (System.Data.SqlClient.SqlException E)
                {
                    throw new Exception(E.Message);
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public static object GetSingle(string SQLString, out string err)
        {
            err = "";
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        connection.Close();
                        err = e.Message;
                        throw e;
                    }
                }
            }
        }
        /// <summary>
        /// 执行查询语句，返回SqlDataReader
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns>SqlDataReader</returns>
        public static SqlDataReader ExecuteReader(string strSQL, out string err)
        {
            err = "";
            SqlConnection connection = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(strSQL, connection);
            try
            {
                connection.Open();
                SqlDataReader myReader = cmd.ExecuteReader();
                return myReader;
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                err = e.Message;
                cmd.Dispose();
                connection.Close();
                throw e;
            }

        }
        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataSet Query(string SQLString, out string err)
        {
            err = "";
            using (SqlConnection connection = new SqlConnection(connString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    SqlDataAdapter command = new SqlDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");
                }
                catch (System.Data.SqlClient.SqlException ex)
                 {
                    err = ex.Message;
                    throw ex;
                }
                return ds;
            }
        }

        /// <summary>
        /// 将表添加到数据库中
        /// </summary>
        /// <param name="strsql"></param>
        /// <param name="dt"></param>
        /// <param name="errinfo"></param>
        /// <returns></returns>
        public static bool ExprotTable(string strsql, DataTable dt, out string errinfo)
        {
            errinfo = "";
            using (SqlConnection connection = new SqlConnection(connString))
            {
                connection.Open();
                SqlTransaction trans = connection.BeginTransaction();
                try
                {
                    SqlDataAdapter sqlDA1 = new SqlDataAdapter(strsql, connString);
                    SqlCommandBuilder sqlCB1 = new SqlCommandBuilder(sqlDA1);
                    sqlDA1.Fill(dt);
                    sqlDA1.Update(dt);

                    dt.AcceptChanges();
                    trans.Commit();
                    connection.Close();
                }
                catch (SqlException ex)
                {
                    trans.Rollback();
                    errinfo = ex.Message;
                    connection.Close();
                    connection.Dispose();
                    throw ex;
                }
                trans.Dispose();
            }

            return true;

        }

        #endregion

        #region 执行带参数的SQL语句

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSql(string SQLString, out string err, params SqlParameter[] cmdParms)
        {
            err = "";
            for (int t = 0; t < cmdParms.Length; t++)
            {
                if (cmdParms[t].Value == null)
                    cmdParms[t].Value = DBNull.Value;
            }
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        return rows;
                    }
                    catch (System.Data.SqlClient.SqlException E)
                    {
                        err = E.Message;
                        throw E;
                    }
                }
            }
        }


        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的SqlParameter[]）</param>
        public static void ExecuteSqlTran(Hashtable SQLStringList)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    SqlCommand cmd = new SqlCommand();
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

                            trans.Commit();
                        }
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }


        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public static object GetSingle(string SQLString, out string err, params SqlParameter[] cmdParms)
        {
            err = "";
            for (int t = 0; t < cmdParms.Length; t++)
            {
                if (cmdParms[t].Value == null)
                    cmdParms[t].Value = DBNull.Value;
            }
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        object obj = cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        err = e.Message;
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// 执行查询语句，返回SqlDataReader
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns>SqlDataReader</returns>
        public static SqlDataReader ExecuteReader(string SQLString, out string err, params SqlParameter[] cmdParms)
        {
            err = "";
            for (int t = 0; t < cmdParms.Length; t++)
            {
                if (cmdParms[t].Value == null)
                    cmdParms[t].Value = DBNull.Value;
            }
            SqlConnection connection = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand();
            try
            {
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                SqlDataReader myReader = cmd.ExecuteReader();
                cmd.Parameters.Clear();
                return myReader;
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                err = e.Message;
                cmd.Dispose();
                connection.Close();
                return null;
            }

        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataSet Query(string SQLString, out string err, params SqlParameter[] cmdParms)
        {
            err = "";
            for (int t = 0; t < cmdParms.Length; t++)
            {
                if (cmdParms[t].Value == null)
                    cmdParms[t].Value = DBNull.Value;
            }
            using (SqlConnection connection = new SqlConnection(connString))
            {
                SqlCommand cmd = new SqlCommand();
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        da.Fill(ds, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (System.Data.SqlClient.SqlException ex)
                    {
                        err = ex.Message;
                        return null;
                    }
                    return ds;
                }
            }
        }


        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, string cmdText, SqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            for (int t = 0; t < cmdParms.Length; t++)
            {
                if (cmdParms[t].Value == null)
                    cmdParms[t].Value = DBNull.Value;
            }
            cmd.CommandType = CommandType.Text;//cmdType;
            if (cmdParms != null)
            {
                foreach (SqlParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }

        #endregion

        #region 存储过程操作
        static int i = 0;
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SqlDataReader</returns>
        public static SqlDataReader RunProcedure(string storedProcName, IDataParameter[] parameters)
        {

            for (int t = 0; t < parameters.Length; t++)
            {
                if (parameters[t].Value == null)
                    parameters[t].Value = DBNull.Value;
            }
            SqlConnection connection = new SqlConnection(connString);
            SqlDataReader returnReader;
            connection.Open();
            SqlCommand command = BuildQueryCommand(connection, storedProcName, parameters);
            command.CommandType = CommandType.StoredProcedure;
            returnReader = command.ExecuteReader();
            return returnReader;
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="tableName">DataSet结果中的表名</param>
        /// <returns>DataSet</returns>
        public static DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName)
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                DataSet dataSet = new DataSet();

                for (int t = 0; t < parameters.Length; t++)
                {
                    if (parameters[t].Value == null)
                        parameters[t].Value = DBNull.Value;
                }
                connection.Open();
                SqlDataAdapter sqlDA = new SqlDataAdapter();

                sqlDA.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
                sqlDA.Fill(dataSet, tableName);
                connection.Close();

                return dataSet;
            }
        }
        public static object ObjLock = new object();


        /// <summary>
        /// 构建 SqlCommand 对象(用来返回一个结果集，而不是一个整数值)
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SqlCommand</returns>
        private static SqlCommand BuildQueryCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            i++;
            IDataParameter[] tmp = (IDataParameter[])parameters.Clone();

            //parameters.CopyTo(tmp,0);
            for (int t = 0; t < parameters.Length; t++)
            {
                if (parameters[t].Value == null)
                    parameters[t].Value = DBNull.Value;
            }
            SqlCommand command = new SqlCommand(storedProcName, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Clear();


            foreach (SqlParameter parameter in tmp)
            {
                command.Parameters.Add(parameter);
            }
            tmp = null;
            return command;

        }


        /// <summary>
        /// 执行存储过程，返回影响的行数		
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="rowsAffected">影响的行数</param>
        /// <returns></returns>
        public static int RunProcedure(string storedProcName, IDataParameter[] parameters, out int rowsAffected)
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                int result;
                connection.Open();             
                SqlCommand command = BuildIntCommand(connection, storedProcName, parameters);
                rowsAffected = command.ExecuteNonQuery();
                result = (int)command.Parameters["ReturnValue"].Value;
                foreach (var i in parameters)
                {
                    if (i.Direction == ParameterDirection.InputOutput || i.Direction == ParameterDirection.Output)
                    {
                        i.Value = ((IDataParameter)(command.Parameters[i.ParameterName])).Value;
                    }
                }
                //Connection.Close();
                return result;
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
            foreach (var i in parameters)
            {
                if (i.Direction == ParameterDirection.InputOutput || i.Direction == ParameterDirection.Output)
                {
                    i.Value = ((IDataParameter)(command.Parameters[i.ParameterName])).Value;
                }
            }
            return command;
        }
        #endregion

        /// <summary>
        /// 根据分页查询返回DataReader 
        /// </summary>
        /// <param name="strWhere"></param>
        /// <param name="orderby"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public static SqlDataReader GetDataReader(string strWhere, string orderby, int startIndex, int endIndex, out string err)
        {
            return DbHelperSQL.ExecuteReader(CreateSqlWhere(strWhere, orderby, startIndex, endIndex).ToString(), out err);
        }
        /// <summary>
        /// 拼接分页查询的SQL语句
        /// </summary>
        /// <param name="strWhere"></param>
        /// <param name="orderby"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        private static StringBuilder CreateSqlWhere(string strWhere, string orderby, int startIndex, int endIndex)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM ( ");
            strSql.Append(" SELECT ROW_NUMBER() OVER (");
            if (!string.IsNullOrEmpty(orderby.Trim()))
            {
                strSql.Append("order by T." + orderby);
            }
            else
            {
                strSql.Append("order by T.ID desc");
            }
            strSql.Append(")AS Row, T.*  from PolicyData T ");
            if (!string.IsNullOrEmpty(strWhere.Trim()))
            {
                strSql.Append(" WHERE " + strWhere);
            }
            strSql.Append(" TT");
            strSql.AppendFormat(" WHERE TT.Row between {0} and {1}", startIndex, endIndex);
            return strSql.Replace("  ", " ");
        }

    }
}
