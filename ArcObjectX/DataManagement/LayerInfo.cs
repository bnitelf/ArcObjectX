using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcObjectX.DataManagement
{
    public class LayerInfo : TableInfo
    {
        /// <summary>
        /// Projection Code (aka Factory code, spatial reference, WKID), -1 for table.
        /// </summary>
        public int ProjectionCode { get; set; }

        /// <summary>
        /// Geometry Type in text.
        /// </summary>
        public string GeometryTypeString { get; set; }

        public GeometryType GeometryType { get; set; }

        public LayerInfo()
        {
            Fields = new List<FieldInfo>();
            ProjectionCode = -1;
        }
    }
}
