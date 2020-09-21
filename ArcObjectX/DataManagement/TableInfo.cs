using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcObjectX.DataManagement
{
    public class TableInfo
    {
        /// <summary>
        /// Table / Layer name
        /// </summary>
        public string Name { get; set; }
        
        public List<FieldInfo> Fields { get; set; }

        public string ObjectIDFieldName { get; set; }
    }
}
