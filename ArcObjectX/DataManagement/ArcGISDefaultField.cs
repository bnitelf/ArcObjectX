using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcObjectX.DataManagement
{
    /// <summary>
    /// Class provide default field name that ArcGIS use.
    /// </summary>
    public class ArcGISDefaultField
    {
        /// <summary>
        /// Layer and Table
        /// </summary>
        public static string ObjectID = "ObjectID";

        /// <summary>
        /// Geometry field
        /// </summary>
        public static string Shape = "Shape";

        /// <summary>
        /// Polygon, Line type Layer (in SDE 
        /// </summary>
        public static string Shape_Length = "Shape_Length";

        /// <summary>
        /// Polygon type layer
        /// </summary>
        public static string Shape_Area = "Shape_Area";

        /// <summary>
        /// Editor Tracking Field
        /// </summary>
        public static string Created_Date = "Created_Date";

        /// <summary>
        /// Editor Tracking Field
        /// </summary>
        public static string Created_User = "Created_User";

        /// <summary>
        /// Editor Tracking Field
        /// </summary>
        public static string Last_Edited_Date = "Last_Edited_Date";

        /// <summary>
        /// Editor Tracking Field
        /// </summary>
        public static string Last_Edited_User = "Last_Edited_User";

        /// <summary>
        /// If add GlobalID field to data
        /// </summary>
        public static string GlobalID = "GlobalID";

        /// <summary>
        /// for __ATTACH (attachment table)
        /// </summary>
        public static string Rel_GlobalID = "Rel_GlobalID";

        public static List<string> GetAllFields()
        {
            return new List<string>()
            {
                ObjectID,
                Shape,
                Shape_Length,
                Shape_Area,
                Created_Date,
                Created_User,
                Last_Edited_Date,
                Last_Edited_User,
                GlobalID,
                Rel_GlobalID
            };
        }
    }
}
