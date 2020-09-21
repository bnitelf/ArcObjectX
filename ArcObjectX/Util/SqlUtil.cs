using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcObjectX.Util
{
    public class SqlUtil
    {
        /// <summary>
        /// In second, 0 to use default.
        /// </summary>
        private static int sqlTimeout = 0;

        /// <summary>
        /// Reset to Default.
        /// </summary>
        public static void ResetSqlTimeout()
        {
            sqlTimeout = 0;
        }

        public static void SetSqlTimeout(int timeoutInSec)
        {
            if (timeoutInSec > 0)
                sqlTimeout = timeoutInSec;
            else
                ResetSqlTimeout();
        }


        /// <summary>
        /// Query data.
        /// </summary>
        /// <param name="connection">connection info to database</param>
        /// <param name="sql">sql statement</param>
        /// <param name="dictParams">the params to put in SqlCommand.Parameters for use with where clause</param>
        /// <returns></returns>
        public static SqlDataReader QueryData(SqlConnection connection, string sql, Dictionary<string, object> dictParams = null)
        {
            SqlCommand command = new SqlCommand(sql, connection);

            if (dictParams != null)
            {
                foreach (var item in dictParams)
                {
                    command.Parameters.AddWithValue(item.Key, item.Value);
                }
            }

            if (sqlTimeout > 0)
                command.CommandTimeout = sqlTimeout;

            if (connection.State != System.Data.ConnectionState.Open)
                connection.Open();

            SqlDataReader reader = command.ExecuteReader();
            return reader;
        }

        public static List<T> QueryData<T>(SqlConnection connection, string sql, Func<SqlDataReader, T> funcReaderToInfoMapper, Dictionary<string, object> dictParams = null)
        {
            List<T> listData = new List<T>();
            SqlDataReader reader = QueryData(connection, sql, dictParams);

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    //Console.WriteLine("{0}\t{1}", reader.GetInt32(0),
                    //    reader.GetString(1));

                    T data = funcReaderToInfoMapper(reader);
                    listData.Add(data);
                }
            }

            reader.Close();

            return listData;
        }

        /// <summary>
        /// Get count of sql query. Ex. SELECT COUNT(*) FROM tableName
        /// </summary>
        /// <param name="connection">connection info to database</param>
        /// <param name="sql">sql count statement. Ex. SELECT COUNT(*) FROM tableName </param>
        /// <param name="dictParams">the params to put in SqlCommand.Parameters for use with where clause</param>
        /// <returns></returns>
        public static int QueryCount(SqlConnection connection, string sql, Dictionary<string, object> dictParams = null)
        {
            string sqlCount = $"SELECT COUNT(*) FROM ({sql}) as src";
            SqlCommand command = new SqlCommand(sqlCount, connection);

            if (dictParams != null)
            {
                foreach (var item in dictParams)
                {
                    command.Parameters.AddWithValue(item.Key, item.Value);
                }
            }

            if (sqlTimeout > 0)
                command.CommandTimeout = sqlTimeout;

            if (connection.State != System.Data.ConnectionState.Open)
                connection.Open();

            int count = (int)command.ExecuteScalar();
            command.Dispose();
            return count;
        }

        /// <summary>
        /// To String (upper case) แบบมี {} ครอบ, ex. {B5A586C8-A22B-4E04-8F75-BB43A97C0551}
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static string ReadGuidValue(Guid guid)
        {
            return guid.ToString("B").ToUpper(); // B เพื่อให้ guid มี {} ครอบ
        }

        /// <summary>
        /// Output sqlWhereIn ex. "{field} IN (@1, @2)"
        /// <para>Output dictParams ex. { {"@1", 1} , ... }</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="listValue"></param>
        /// <param name="sqlWhereIn"></param>
        /// <param name="dictParams"></param>
        public static void CreateWhereIn<T>(string field, List<T> listValue, out string sqlWhereIn, out Dictionary<string, T> dictParams)
        {
            StringBuilder sbParamStr = new StringBuilder();
            string param;

            dictParams = new Dictionary<string, T>();

            for (int i = 0; i < listValue.Count; i++)
            {
                T data = listValue[i];

                param = "@" + (i + 1);

                if (i + 1 != listValue.Count)
                    sbParamStr.Append(param + ",");
                else
                    sbParamStr.Append(param);

                dictParams.Add(param, listValue[i]);
            }

            sqlWhereIn = $"{field} IN ({sbParamStr.ToString()})";
        }

        /// <summary>
        /// Output Sample (pend on type)
        /// <para>Case string, datetime, object. => {field} IN ('value1', ...) </para>
        /// <para>Case int, other number type => {field} IN (value1, ...)</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="listValue"></param>
        /// <param name="sqlWhereIn"></param>
        public static string CreateWhereIn<T>(string field, List<T> listValue)
        {
            StringBuilder sbParamStr = new StringBuilder();
            string sqlWhereIn = "";

            Type type = typeof(T);

            if (type == typeof(int)
                || type == typeof(short)
                || type == typeof(long)
                || type == typeof(float)
                || type == typeof(double))
            {
                sqlWhereIn = $"{field} IN ({string.Join(", ", listValue)})";
            }
            else if (type == typeof(DateTime))
            {
                string[] listDateTimeStr = listValue.Select(x => ((DateTime)(object)x).ToString("yyyy-MM-dd HH:mm:ss")).ToArray();
                sqlWhereIn = $"{field} IN ('{string.Join("', '", listDateTimeStr)}')";
            }
            else
            {
                // string 
                sqlWhereIn = $"{field} IN ('{string.Join("', '", listValue)}')";
            }

            return sqlWhereIn;
        }

        public static int ExecuteSql(SqlConnection connection, string sql, Dictionary<string, object> dictParams = null, SqlTransaction tran = null)
        {
            int affectedRow = 0;

            if (connection.State != System.Data.ConnectionState.Open)
                connection.Open();

            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                if (dictParams != null)
                {
                    foreach (var item in dictParams)
                    {
                        command.Parameters.AddWithValue(item.Key, item.Value);
                    }
                }

                if (tran != null)
                {
                    command.Transaction = tran;
                }

                if (sqlTimeout > 0)
                    command.CommandTimeout = sqlTimeout;

                affectedRow = command.ExecuteNonQuery();
            }

            return affectedRow;
        }
    }
}
