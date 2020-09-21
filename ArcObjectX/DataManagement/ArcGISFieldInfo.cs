using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcObjectX.DataManagement
{
    /// <summary>
    /// Field info that ArcGIS always has.
    /// </summary>
    public class ArcGISFieldInfo : FieldInfo
    {
        public bool IsObjectIDField { get; set; }
    }
}
