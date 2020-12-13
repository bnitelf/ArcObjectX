using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcObjectX.DataManagement
{
    /// <summary>
    /// Class contain info for connect sde.
    /// </summary>
    public class SdeConnectionInfo
    {
        public string Server { get; set; }
        public string DBName { get; set; }
        public string User { get; set; }
        public string Pass { get; set; }
        /// <summary>
        /// SDE Version
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// Default table owner that use to open layer or table.
        /// </summary>
        public string DefaultTableOwner { get; set; }

        /// <summary>
        /// DB Client text. Ex. SQLServer, Oracle, etc.
        /// </summary>
        public string DbClientString { get; set; }

        public DBClient DbClient { get; set; }

        /// <summary>
        /// Default DB Client use to connect.
        /// </summary>
        public static DBClient DefaultDbClient = DBClient.SQL_Server;

        public override string ToString()
        {
            return $"Server={Server}, DBName={DBName}, User={User}, Version={Version}";
        }

        public string ToLongString()
        {
            return ToString() + $", DefaultTableOwner={DefaultTableOwner}, DbClient = {DbClient.ToIPropertyString()}";
        }

        /// <summary>
        /// Get Datasource path liked string. Ex. \\Server\DBName\User\Version
        /// </summary>
        /// <returns></returns>
        public string ToDsPathLikedString()
        {
            return $"\\\\{Server}\\{DBName}\\{User}\\{Version}";
        }

        /// <summary>
        /// Get Datasource path liked string. Ex. \\Server\DBName\User\Version\DefaultTableOwner
        /// </summary>
        /// <returns></returns>
        public string ToDsPathLikedStringWithDefaultTableOwner()
        {
            return $"\\\\{Server}\\{DBName}\\{User}\\{Version}\\{DefaultTableOwner}";
        }
    }
}
