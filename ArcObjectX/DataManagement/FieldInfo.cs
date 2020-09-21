using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcObjectX.DataManagement
{
    /// <summary>
    /// General field info.
    /// </summary>
    public class FieldInfo
    {
        public string FieldName { get; set; }
        public string FieldType { get; set; }
        public int Width { get; set; }
        public int Precision { get; set; }
        public bool Nullable { get; set; }
        public object DefaultValue { get; set; }
        /// <summary>
        /// Is primary key
        /// </summary>
        public bool IsPK { get; set; }
    }
}
