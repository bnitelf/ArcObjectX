using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcObjectX.Util.Extension
{
    public static class FeatureClassExtension
    {

        /// <summary>
        /// Get "layer {layerName}" message.
        /// </summary>
        /// <param name="fclass">Feature Class</param>
        /// <returns></returns>
        public static string GetPrintName(this IFeatureClass fclass)
        {
            string tableName = ((IDataset)fclass).Name;
            return $"layer {tableName}";
        }

        /// <summary>
        /// Get feature class name.
        /// </summary>
        /// <param name="fclass"></param>
        /// <returns></returns>
        public static string GetName(this IFeatureClass fclass)
        {
            string layerName = ((IDataset)fclass).Name;
            return layerName;
        }

        /// <summary>
        /// Check if feature class has Z value enable.
        /// </summary>
        /// <param name="featureClass"></param>
        /// <returns></returns>
        public static bool HasZValue(this IFeatureClass featureClass)
        {
            string shapeFieldName = featureClass.ShapeFieldName;
            IFields fields = featureClass.Fields;
            int geometryIndex = fields.FindField(shapeFieldName);
            IField field = fields.get_Field(geometryIndex);
            IGeometryDef geometryDef = field.GeometryDef;

            return geometryDef.HasZ;
        }

        /// <summary>
        /// Check if feature class has M value enable.
        /// </summary>
        /// <param name="featureClass"></param>
        /// <returns></returns>
        public static bool HasMValue(this IFeatureClass featureClass)
        {
            string shapeFieldName = featureClass.ShapeFieldName;
            IFields fields = featureClass.Fields;
            int geometryIndex = fields.FindField(shapeFieldName);
            IField field = fields.get_Field(geometryIndex);
            IGeometryDef geometryDef = field.GeometryDef;

            return geometryDef.HasM;
        }
    }
}
