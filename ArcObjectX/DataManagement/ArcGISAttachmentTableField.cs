using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcObjectX.DataManagement
{
    /// <summary>
    /// Provide field name of ArcGIS Attachment Table Fields.
    /// </summary>
    public class ArcGISAttachmentTableField
    {
        /// <summary>
        /// ObjectID type field of attachment table.
        /// </summary>
        public static string AttachmentID = "AttachmentID";

        /// <summary>
        /// If main layer/table not has GlobalID field, 
        /// ArcGIS will use this field to link attachment record with it.
        /// </summary>
        public static string Rel_ObjectID = "Rel_ObjectID";

        /// <summary>
        /// If main layer/table has GlobalID field, 
        /// ArcGIS will use this field to link attachment record with it.
        /// </summary>
        public static string Rel_GlobalID = "Rel_GlobalID";

        /// <summary>
        /// file format text.
        /// </summary>
        public static string Content_Type = "Content_Type";

        /// <summary>
        /// original file name.
        /// </summary>
        public static string Att_Name = "Att_Name";

        /// <summary>
        /// Data size in byte.
        /// </summary>
        public static string Data_Size = "Data_Size";

        /// <summary>
        /// Store BLOB data.
        /// </summary>
        public static string Data = "Data";
    }
}
