using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcObjectX.Util.Extension
{
    public static class SqlDataReaderExtension
    {
        /// <summary>
        /// Get int value of the specified fieldName, if field not exist in reader or is null value return default;
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="fieldName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int GetIntIfAny(this SqlDataReader reader, string fieldName, int defaultValue = 0)
        {
            int value = defaultValue;
            int index = reader.GetOrdinal(fieldName);
            if (index != -1)
            {
                if (!reader.IsDBNull(index))
                    value = reader.GetInt32(index);
            }
            return value;
        }

        /// <summary>
        /// Get short value of the specified fieldName, if field not exist in reader or is null value return default;
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="fieldName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static short GetShortIfAny(this SqlDataReader reader, string fieldName, short defaultValue = 0)
        {
            short value = defaultValue;
            int index = reader.GetOrdinal(fieldName);
            if (index != -1)
            {
                if (!reader.IsDBNull(index))
                    value = reader.GetInt16(index);
            }
            return value;
        }

        /// <summary>
        /// Get string value of the specified fieldName, if field not exist in reader or is null value return default;
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="fieldName"></param>
        /// <param name="defaultValue"></param>
        public static string GetStringIfAny(this SqlDataReader reader, string fieldName, string defaultValue = null)
        {
            string value = defaultValue;
            int index = reader.GetOrdinal(fieldName);
            if (index != -1)
            {
                if (!reader.IsDBNull(index))
                    value = reader.GetString(index);
            }
            return value;
        }

        /// <summary>
        /// Get Guid string value (upper case) of the specified fieldName, if field not exist in reader or is null value return default
        /// <para>ex. {B5A586C8-A22B-4E04-8F75-BB43A97C0551}</para>
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="fieldName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string GetGuidStringIfAny(this SqlDataReader reader, string fieldName, string defaultValue = null)
        {
            string value = defaultValue;
            int index = reader.GetOrdinal(fieldName);
            if (index != -1)
            {
                if (!reader.IsDBNull(index))
                    value = reader.GetGuid(index).ToString("B").ToUpper(); // B เพื่อให้ guid มี {} ครอบ
            }
            return value;
        }

    }
}
