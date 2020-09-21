using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcObjectX.DataManagement
{
    /// <summary>
    /// Supported DB Client list. For get string use with IProperty "DBClient" see DBClientExtension.ToIPropertyString() />
    /// </summary>
    public enum DBClient
    {
        ALTIBASE,
        DB2,
        DB2_for_zOS,
        Dameng,
        Informix,
        Netezza,
        Oracle,
        PostgreSQL,
        SAP_HANA,
        SQL_Server,
        Teradata,
    }

    public static class DBClientExtension
    {
        /// <summary>
        /// Get string for use with IProperty "DBClient"
        /// </summary>
        /// <param name="dbClient"></param>
        /// <returns></returns>
        public static string ToIPropertyString(this DBClient dbClient)
        {
            switch (dbClient)
            {
                case DBClient.Oracle:
                    return "Oracle";

                case DBClient.SQL_Server:
                    return "SQLServer";

                default:
                    return dbClient.ToString();
            }
        }
    }
}
