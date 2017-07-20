using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;


namespace Common
{
    /// <summary>
    /// DBExecute 的摘要说明。
    /// </summary>
    public class DBExecute
    {
        public static void ExecuteNonQuery(SqlCommand cmd, string sql)
        {
            SqlConnection connection = new SqlConnection(GetConnString());
            try
            {
                cmd.CommandText = sql;
                cmd.Connection = connection;
                cmd.CommandTimeout = 240;
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            finally
            {
                connection.Close();
            }
        }

        public static string ExecuteScalar(SqlCommand cmd, string sql)
        {
            string str2;
            SqlConnection connection = new SqlConnection(GetConnString());
            connection.Open();
            try
            {
                cmd.CommandText = sql;
                cmd.Connection = connection;
                cmd.CommandTimeout = 240;
                object obj2 = cmd.ExecuteScalar();
                str2 = (obj2 == null) ? "" : obj2.ToString();
            }
            catch (Exception exception)
            {
                str2 = exception.ToString();
            }
            finally
            {
                connection.Close();
            }
            return str2;
        }

        public static int ExecuteSQL(string SqlString)
        {
            int num = -1;
            SqlConnection connection = new SqlConnection(GetConnString());
            connection.Open();
            try
            {
                num = new SqlCommand(SqlString, connection).ExecuteNonQuery();
            }
            catch
            {
                num = -1;
            }
            finally
            {
                connection.Close();
            }
            return num;
        }



        public static string GetConnString()
        {
            return ConfigurationSettings.AppSettings["ConnString"];
        }

        public static string GetSWUConnString()
        {
            return ConfigurationSettings.AppSettings["SWUConnString"];
        }

        public static DataTable GetSWUData(SqlCommand cmd, string strSQL)
        {
            SqlConnection connection = new SqlConnection(GetSWUConnString());
            DataTable dt = new DataTable();
            try
            {
                cmd.CommandText = strSQL;
                cmd.Connection = connection;
                cmd.CommandTimeout = 240;
                connection.Open();
                SqlDataAdapter myAdapter = new SqlDataAdapter(cmd);
                myAdapter.Fill(dt);
                return dt;
            }
            catch (Exception exception)
            {
                string message = exception.Message;
                return dt;
            }
            finally
            {
                connection.Close();
            }
        }

        public static void GetData(SqlCommand cmd, string strSQL, DataTable myTable)
        {
            SqlConnection connection = new SqlConnection(GetConnString());
            try
            {
                cmd.CommandText = strSQL;
                cmd.Connection = connection;
                cmd.CommandTimeout = 240;
                connection.Open();
                new SqlDataAdapter { SelectCommand = cmd }.Fill(myTable);
            }
            catch (Exception exception)
            {
                string message = exception.Message;
            }
            finally
            {
                connection.Close();
            }
        }

        public static DataTable GetData(SqlCommand cmd, string strSQL)
        {
            SqlConnection connection = new SqlConnection(GetConnString());
            DataTable dt = new DataTable();
            try
            {
                cmd.CommandText = strSQL;
                cmd.Connection = connection;
                cmd.CommandTimeout = 240;
                connection.Open();
                SqlDataAdapter myAdapter = new SqlDataAdapter(cmd);
                myAdapter.Fill(dt);
                return dt;
            }
            catch (Exception exception)
            {
                string message = exception.Message;
                return dt;
            }
            finally
            {
                connection.Close();
            }
        }

        public static SqlDataReader GetDataReader(string sql)
        {
            SqlDataReader reader;
            try
            {
                SqlCommand command = new SqlCommand();
                SqlConnection connection = new SqlConnection(GetConnString());
                connection.Open();
                command.CommandText = sql;
                command.Connection = connection;
                command.CommandTimeout = 240;
                reader = command.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (SqlException exception)
            {
                throw new Exception(exception.Message);
            }
            return reader;
        }

        public static void UpdateData(DataTable myTable)
        {
            SqlDataAdapter adapter = new SqlDataAdapter("select * from [" + myTable.TableName + "]", GetConnString());
            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
            adapter.InsertCommand = builder.GetInsertCommand();
            adapter.UpdateCommand = builder.GetUpdateCommand();
            adapter.DeleteCommand = builder.GetDeleteCommand();
            adapter.Update(myTable);
        }


        public static DataSet ExecuteQuery(CommandType cmdType, string cmdText, out int RecordCount, out int PageCount, params SqlParameter[] cmdParms)
        {
            bool mustCloseConnection = false;
            DataSet mySet = new DataSet();
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection myConnection = new SqlConnection(GetConnString()))
            {
                PrepareCommand(cmd, myConnection, null, cmdType, cmdText, cmdParms, out mustCloseConnection);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(mySet);
                myConnection.Close();
            }
            RecordCount = (int)cmd.Parameters[7].Value;
            PageCount = (int)cmd.Parameters[8].Value;
            return mySet;
        }

        public static DataSet ExecuteQuery(CommandType cmdType, string cmdText, out int RecordCount, params SqlParameter[] cmdParms)
        {
            bool mustCloseConnection = false;
            DataSet mySet = new DataSet();
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection myConnection = new SqlConnection(GetConnString()))
            {
                PrepareCommand(cmd, myConnection, null, cmdType, cmdText, cmdParms, out mustCloseConnection);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(mySet);
                myConnection.Close();
            }
            RecordCount = (int)cmd.Parameters[5].Value;
            return mySet;
        }

        private static void PrepareCommand(SqlCommand command, SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters, out bool mustCloseConnection)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("commandText");
            // If the provided connection is not open, we will open it
            if (connection.State != ConnectionState.Open)
            {
                mustCloseConnection = true;
                connection.Open();
            }
            else
            {
                mustCloseConnection = false;
            }
            // Associate the connection with the command
            command.Connection = connection;
            // Set the command text (stored procedure name or SQL statement)
            command.CommandText = commandText;
            // If we were provided a transaction, assign it
            if (transaction != null)
            {
                if (transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
                command.Transaction = transaction;
            }
            // Set the command type
            command.CommandType = commandType;
            // Attach the command parameters if they are provided
            if (commandParameters != null)
            {
                AttachParameters(command, commandParameters);
            }
            return;
        }
        private static void AttachParameters(SqlCommand command, SqlParameter[] commandParameters)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (commandParameters != null)
            {
                foreach (SqlParameter p in commandParameters)
                {
                    if (p != null)
                    {
                        // Check for derived output value with no value assigned
                        if ((p.Direction == ParameterDirection.InputOutput ||
                            p.Direction == ParameterDirection.Input) &&
                            (p.Value == null))
                        {
                            p.Value = DBNull.Value;
                        }
                        command.Parameters.Add(p);
                    }
                }
            }
        }

    }
}
