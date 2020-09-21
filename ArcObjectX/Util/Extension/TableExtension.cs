using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcObjectX.Util.Extension
{
    public static class TableExtension
    {
        /// <summary>
        /// Get "table {tableName}" message.
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static string GetPrintName(this ITable table)
        {
            string tableName = ((IDataset)table).Name;
            return $"table {tableName}";
        }

        /// <summary>
        /// Get table name.
        /// </summary>
        /// <param name="fclass"></param>
        /// <returns></returns>
        public static string GetName(this ITable table)
        {
            string tableName = ((IDataset)table).Name;
            return tableName;
        }
    }
}
