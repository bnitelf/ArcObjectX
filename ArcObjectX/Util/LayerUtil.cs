using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geoprocessor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcObjectX.Util
{
    public class LayerUtil
    {
        #region Config
        public static int DefaultStringWidth = 50;
        public static int DefaultIntWidth = 10;
        public static int DefaultShortWidth = 4;
        public static int DefaultFloatWidth = 10;
        public static int DefaultFloatPrecision = 2;
        public static int DefaultDoubleWidth = 10;
        public static int DefaultDoublePrecision = 6;
        #endregion

        public static IWorkspace GetWorkspace(IFeatureClass fclass)
        {
            IDataset dataset = (IDataset)fclass;
            IWorkspace ws = dataset.Workspace;
            return ws;
        }

        public static IWorkspace GetWorkspace(ITable table)
        {
            IDataset dataset = (IDataset)table;
            IWorkspace ws = dataset.Workspace;
            return ws;
        }

        /// <summary>
        /// Get feature class / table name.
        /// </summary>
        /// <param name="fclass"></param>
        /// <returns></returns>
        public static string GetLayerName(ITable table)
        {
            string layerName = ((IDataset)table).Name;
            return layerName;
        }

        public static string GetLayerName(IFeatureClass fclass)
        {
            string layerName = ((IDataset)fclass).Name;
            return layerName;
        }

        /// <summary>
        /// Get feature class / table name without table owner.
        /// Useful for sde datasource.
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public string GetLayerNameWithOutTableOwner(ITable table)
        {
            IDataset ds = table as IDataset;
            string layerName = ds.Name;
            string layerNameWithoutOwner = layerName.Split('.').Last();
            return layerNameWithoutOwner;
        }

        /// <summary>
        /// Get feature class / table name without table owner.
        /// Useful for sde datasource.
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public string GetLayerNameWithOutTableOwner(IFeatureClass fclass)
        {
            IDataset ds = fclass as IDataset;
            string layerName = ds.Name;
            string layerNameWithoutOwner = layerName.Split('.').Last();
            return layerNameWithoutOwner;
        }

        /// <summary>
        /// Get "layer {layerName}" message.
        /// </summary>
        /// <param name="fclass">Feature Class</param>
        /// <returns></returns>
        public static string GetPrintName(IFeatureClass fclass)
        {
            string tableName = ((IDataset)fclass).Name;
            return $"layer {tableName}";
        }

        /// <summary>
        /// Get "table {tableName}" message. Or "layer {layerName}" if ITable is IFeatureClass instance.
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static string GetPrintName(ITable table)
        {
            if (table is IFeatureClass)
            {
                return GetPrintName(table as IFeatureClass);
            }

            string tableName = ((IDataset)table).Name;
            return $"table {tableName}";
        }


        /// <summary>
        /// Check if table enable attachment or not. (support IFeatureClass too)
        /// </summary>
        /// <returns></returns>
        public static bool IsEnableAttachment(ITable table)
        {
            ITableAttachments tableAttachments = (ITableAttachments)table;
            return tableAttachments.HasAttachments;
        }

        /// <summary>
        /// Check if table enable Editor Tracking or not. (support IFeatureClass too)
        /// By using indirect check. Check if has Date field type and is not editable.
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static bool IsEnableEditorTrackingIndirectCheck(ITable table)
        {
            for (int i = 0; i < table.Fields.FieldCount; i++)
            {
                IField field = table.Fields.Field[i];

                if (field.Type == esriFieldType.esriFieldTypeDate && field.Editable == false)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layerPath">example: D:\Data.gdb\PointLayer, D:\Data.sde\myDataset\PointLayer </param>
        /// <returns></returns>
        public static bool IsEnableEditorTrackingByLayerPath(string layerPath)
        {
            // Example : @"D:\Data.gdb\PointLayer";

            // Case connection in ArcMap
            // Example : @"Database Connections\mySdeDB.sde\ShopPoint";

            Geoprocessor GP = new Geoprocessor();
            object DataType = "";
            IDataElement DataElement = GP.GetDataElement(layerPath, ref DataType);
            IDEEditorTracking ideEditTrack = (IDEEditorTracking)DataElement;
            if (ideEditTrack.EditorTrackingEnabled == true) { return true; }
            else { return false; }
        }

        /// <summary>
        /// Return List editor tracking fields, If not enable return empty list.
        /// </summary>
        /// <param name="layerPath">example: D:\Data.gdb\PointLayer, D:\Data.sde\myDataset\PointLayer </param>
        /// <param name="createUserField"></param>
        /// <param name="createDateField"></param>
        /// <param name="lastEditUserField"></param>
        /// <param name="lastEditDateField"></param>
        /// <returns>List editor tracking field, order in this order (createUserField, createDateField, lastEditUserField, lastEditDateField)</returns>
        public static List<string> GetEditorTrackingFieldNames(string layerPath
            , out string createUserField, out string createDateField
            , out string lastEditUserField, out string lastEditDateField)
        {
            List<string> editorTrackingFields = new List<string>();
            createUserField = "";
            createDateField = "";
            lastEditUserField = "";
            lastEditDateField = "";

            Geoprocessor GP = new Geoprocessor();
            object DataType = "";
            IDataElement DataElement = GP.GetDataElement(layerPath, ref DataType);
            IDEEditorTracking ideEditTrack = (IDEEditorTracking)DataElement;
            if (ideEditTrack.EditorTrackingEnabled == true) 
            {
                createUserField = ideEditTrack.CreatorFieldName;
                createDateField = ideEditTrack.CreatedAtFieldName;
                lastEditUserField = ideEditTrack.EditorFieldName;
                lastEditDateField = ideEditTrack.EditedAtFieldName;
            }

            return editorTrackingFields;
        }

        #region Spatial Reference
        public static ISpatialReference GetSpatialReference(IFeatureClass fclass)
        {
            IGeoDataset geoDataset = (IGeoDataset)fclass;
            ISpatialReference sr = geoDataset.SpatialReference;
            return sr;
        }

        /// <summary>
        /// Get projection code (aka: WKID)
        /// </summary>
        /// <param name="fclass"></param>
        /// <returns></returns>
        public static int GetProjectionCode(IFeatureClass fclass)
        {
            IGeoDataset geoDataset = (IGeoDataset)fclass;
            int wkid = geoDataset.SpatialReference.FactoryCode;
            return wkid;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectionCode">projectionCode (aka: WKID)</param>
        /// <returns></returns>
        public static ISpatialReference CreateProjectedSpatialReference(int projectionCode)
        {
            ISpatialReferenceFactory spatialReferenceFactory = new SpatialReferenceEnvironmentClass();
            IProjectedCoordinateSystem pcs = spatialReferenceFactory.CreateProjectedCoordinateSystem(projectionCode);
            ISpatialReference sr = pcs;
            return sr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectionCode">projectionCode (aka: WKID)</param>
        /// <returns></returns>
        public static ISpatialReference CreateGeographicSpatialReference(int projectionCode)
        {
            ISpatialReferenceFactory spatialReferenceFactory = new SpatialReferenceEnvironmentClass();
            IGeographicCoordinateSystem gcs = spatialReferenceFactory.CreateGeographicCoordinateSystem(projectionCode);
            ISpatialReference sr = gcs;
            return sr;
        }

        public static bool IsProjectedCoordinateSystem(IFeatureClass fclass)
        {
            return IsProjectedCoordinateSystem(GetSpatialReference(fclass));
        }

        public static bool IsGeographicCoordinateSystem(IFeatureClass fclass)
        {
            return IsGeographicCoordinateSystem(GetSpatialReference(fclass));
        }

        public static bool IsProjectedCoordinateSystem(ISpatialReference sr)
        {
            if (sr is IProjectedCoordinateSystem)
            {
                return true;
            }
            return false;
        }

        public static bool IsGeographicCoordinateSystem(ISpatialReference sr)
        {
            if (sr is IGeographicCoordinateSystem)
            {
                return true;
            }
            return false;
        }


        public static IFields ConvertGeometryFieldFromLowToHighPrecision(IFields fields)
        {
            IFieldsEdit fieldsEdit = (IFieldsEdit)fields;

            IField field = null;
            IFieldEdit fieldEdit = null;


            for (int i = 0; i < fields.FieldCount; i++)
            {
                field = fields.Field[i];
                fieldEdit = (IFieldEdit)field;

                if (field.GeometryDef != null)
                {
                    IGeometryDefEdit geometryDefEdit = (IGeometryDefEdit)fieldEdit.GeometryDef;
                    if (IsLowPrecisionSpatialReference(geometryDefEdit.SpatialReference))
                    {
                        geometryDefEdit.SpatialReference_2 = ConvertSpatialReferenceFromLowToHighPrecision(geometryDefEdit.SpatialReference);
                    }

                    break;
                }
            }

            return fields;
        }

        public static bool IsLowPrecisionSpatialReference(ISpatialReference sr)
        {
            IControlPrecision2 controlPrecision2 = sr as IControlPrecision2;
            return !controlPrecision2.IsHighPrecision;
        }

        public static ISpatialReference ConvertSpatialReferenceFromLowToHighPrecision(ISpatialReference lowSpatialReference)
        {
            return ConvertSpatialReferenceFromLowToHighPrecision(lowSpatialReference, -1, 0, 0);
        }

        /// <summary>
        /// Converts an existing low precision spatial reference and returns a new high precision spatial reference.
        /// </summary>
        /// <param name="lowSpatialReference">An ISpatialReference interface that is the low spatial reference to be converted.</param>
        /// <param name="xyDoubler">A System.Int32 that is the amount of doubling (2^x) based on the input resolution; -1 produces a value close to the default resolution. Example: -1</param>
        /// <param name="mDoubler">A System.Int32 that determines doubling of m-resolution based on input m-resolution; the default is 0. Example: 0</param>
        /// <param name="zDoubler">A System.Int32 that determines doubling of z-resolution based on input z-resolution; default is 0. Example: 0</param>
        /// <returns>An ISpatialReference interface that is the new high precision spatial reference.</returns>
        public static ISpatialReference ConvertSpatialReferenceFromLowToHighPrecision(ISpatialReference lowSpatialReference, int xyDoubler, int mDoubler, int zDoubler)
        {
            IControlPrecision2 controlPrecision2 = lowSpatialReference as IControlPrecision2;
            if (controlPrecision2.IsHighPrecision)
                throw new ArgumentException("Expected a low precision spatial reference.", "lowSpatialReference");

            ISpatialReferenceFactory3 spatialReferenceFactory3 = new SpatialReferenceEnvironmentClass();
            ISpatialReference highSpatialReference = spatialReferenceFactory3.ConstructHighPrecisionSpatialReference(lowSpatialReference, xyDoubler, zDoubler, mDoubler);
            return highSpatialReference;
        }

        #endregion

        #region Fields
        public static void AddField(ITable table, string fieldName, esriFieldType fieldType, int width = -1, int precision = -1, bool nullable = true, object defaultValue = null, string aliasName = null)
        {
            if (table.FindField(fieldName) != -1)
            {
                string printName = GetPrintName(table);
                throw new InvalidOperationException($"Field \"{fieldName}\" already exists in {printName}.");
            }
            
            IField field = CreateField(fieldName, fieldType, width, precision, nullable, defaultValue, aliasName);
            table.AddField(field);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="fieldName"></param>
        /// <param name="fieldType">See valid values at <see cref="TypeUtil.ToEsriFieldType(string)"/></param>
        /// <param name="width"></param>
        /// <param name="precision"></param>
        /// <param name="nullable"></param>
        /// <param name="defaultValue"></param>
        /// <param name="aliasName"></param>
        public static void AddField(ITable table, string fieldName, string fieldType, int width = -1, int precision = -1, bool nullable = true, object defaultValue = null, string aliasName = null)
        {
            esriFieldType esriFieldType = TypeUtil.ToEsriFieldType(fieldType);
            AddField(table, fieldName, esriFieldType, width, precision, nullable, defaultValue, aliasName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fclass"></param>
        /// <param name="fieldName"></param>
        /// <param name="fieldType">See valid values at <see cref="TypeUtil.ToEsriFieldType(string)"/></param>
        /// <param name="width"></param>
        /// <param name="precision"></param>
        /// <param name="nullable"></param>
        /// <param name="defaultValue"></param>
        /// <param name="aliasName"></param>
        public static void AddField(IFeatureClass fclass, string fieldName, string fieldType, int width = -1, int precision = -1, bool nullable = true, object defaultValue = null, string aliasName = null)
        {
            AddField(fclass as ITable, fieldName, fieldType, width, precision, nullable, defaultValue, aliasName);
        }

        public static void AddField(IFeatureClass fclass, string fieldName, esriFieldType fieldType, int width = -1, int precision = -1, bool nullable = true, object defaultValue = null, string aliasName = null)
        {
            AddField(fclass as ITable, fieldName, fieldType, width, precision, nullable, defaultValue, aliasName);
        }

        public static void AddFieldIfNotExist(ITable table, string fieldName, esriFieldType fieldType, int width = -1, int precision = -1, bool nullable = true, object defaultValue = null, string aliasName = null)
        {
            if (table.FindField(fieldName) == -1)
            {
                AddField(table, fieldName, fieldType, width, precision, nullable, defaultValue, aliasName);
            }
        }

        public static void AddFieldIfNotExist(ITable table, string fieldName, string fieldType, int width = -1, int precision = -1, bool nullable = true, object defaultValue = null, string aliasName = null)
        {
            if (table.FindField(fieldName) == -1)
            {
                AddField(table, fieldName, fieldType, width, precision, nullable, defaultValue, aliasName);
            }
        }

        public static void AddFieldIfNotExist(IFeatureClass fclass, string fieldName, esriFieldType fieldType, int width = -1, int precision = -1, bool nullable = true, object defaultValue = null, string aliasName = null)
        {
            if (fclass.FindField(fieldName) == -1)
            {
                AddField(fclass, fieldName, fieldType, width, precision, nullable, defaultValue, aliasName);
            }
        }

        public static void AddFieldIfNotExist(IFeatureClass fclass, string fieldName, string fieldType, int width = -1, int precision = -1, bool nullable = true, object defaultValue = null, string aliasName = null)
        {
            if (fclass.FindField(fieldName) == -1)
            {
                AddField(fclass, fieldName, fieldType, width, precision, nullable, defaultValue, aliasName);
            }
        }

        public static IField CreateField(string fieldName, esriFieldType fieldType, int width = -1, int precision = -1, bool nullable = true, object defaultValue = null, string aliasName = null)
        {
            IField field = new FieldClass();
            SetField(field, fieldName, fieldType, width, precision, nullable, defaultValue, aliasName);
            return field;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="fieldType">See valid values at <see cref="ToEsriFieldType(string)"/></param>
        /// <param name="width"></param>
        /// <param name="precision"></param>
        /// <param name="nullable"></param>
        /// <param name="defaultValue"></param>
        /// <param name="aliasName"></param>
        /// <returns></returns>
        public static IField CreateField(string fieldName, string fieldType, int width = -1, int precision = -1, bool nullable = true, object defaultValue = null, string aliasName = null)
        {
            IField field = new FieldClass();
            SetField(field, fieldName, fieldType, width, precision, nullable, defaultValue, aliasName);
            return field;
        }

        /// <summary>
        /// Set field properties.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="fieldName"></param>
        /// <param name="fieldType"></param>
        /// <param name="width">(optional) For int, short, long, float, double, use to assign to precision property (max digit). If not specified, will use DefaultXXXWidth parameter. <see cref="DefaultStringWidth"/></param>
        /// <param name="precision">(optional) For float, double, use to assign to scale property (max decimal place). If not specified, will use DefaultXXXPrecision parameter. <see cref="DefaultDoublePrecision"/></param>
        /// <param name="nullable">(optional) default true</param>
        /// <param name="defaultValue">(optional)</param>
        /// <param name="aliasName">(optional) If not specified, will be the same as field name.
        /// </param>
        /// <returns></returns>
        public static IField SetField(IField field, string fieldName, esriFieldType fieldType, int width = -1, int precision = -1, bool nullable = true, object defaultValue = null, string aliasName = null)
        {
            // create a user defined text field
            IFieldEdit fieldEdit = (IFieldEdit)field; // Explicit Cast

            // setup field properties
            fieldEdit.Name_2 = fieldName;
            fieldEdit.Type_2 = fieldType;
            fieldEdit.IsNullable_2 = nullable;
            fieldEdit.Editable_2 = true;

            // Field size
            // - default field size
            if (width <= 0)
                width = GetDefaultFieldWidthByType(fieldType);

            if (precision <= 0)
                precision = GetDefaultFieldPrecisionByType(fieldType);

            // - set size
            switch (fieldType)
            {
                case esriFieldType.esriFieldTypeString:
                    fieldEdit.Length_2 = width;
                    break;

                case esriFieldType.esriFieldTypeInteger:
                case esriFieldType.esriFieldTypeSmallInteger:
                    fieldEdit.Precision_2 = width;
                    break;

                case esriFieldType.esriFieldTypeSingle:
                case esriFieldType.esriFieldTypeDouble:
                    fieldEdit.Precision_2 = width;
                    fieldEdit.Scale_2 = precision;
                    break;

                default:
                    // do nothing
                    break;
            }

            if (defaultValue != null)
            {
                fieldEdit.DefaultValue_2 = defaultValue;
            }

            if (!string.IsNullOrWhiteSpace(aliasName))
            {
                fieldEdit.AliasName_2 = aliasName;
            }

            return field;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="fieldName"></param>
        /// <param name="fieldType">See valid values at <see cref="ToEsriFieldType(string)"/></param>
        /// <param name="width"></param>
        /// <param name="precision"></param>
        /// <param name="nullable"></param>
        /// <param name="defaultValue"></param>
        /// <param name="aliasName"></param>
        /// <returns></returns>
        public static IField SetField(IField field, string fieldName, string fieldType, int width = -1, int precision = -1, bool nullable = true, object defaultValue = null, string aliasName = null)
        {
            esriFieldType esriFieldType = TypeUtil.ToEsriFieldType(fieldType);
            SetField(field, fieldName, esriFieldType, width, precision, nullable, defaultValue, aliasName);
            return field;
        }

        /// <summary>
        /// Get Geometry Field and Clone.
        /// </summary>
        /// <param name="fclass"></param>
        /// <returns></returns>
        public static IField GetGeometryField(IFeatureClass fclass)
        {
            return GetGeometryField(fclass.Fields);
        }

        /// <summary>
        /// Get Geometry Field and Clone.
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static IField GetGeometryField(IFields fields)
        {
            IField field = null;

            for (int i = 0; i < fields.FieldCount; i++)
            {
                field = fields.Field[i];
                if (field.Type == esriFieldType.esriFieldTypeGeometry)
                {
                    return CloneField(field);
                }
            }

            return field;
        }

        /// <summary>
        /// Get default field width.
        /// For int, short, long, float, double, use to assign to precision property.
        /// For other type, return 0.
        /// <para>Precision (max digit, number before dot.)</para>
        /// </summary>
        /// <param name="fieldType"></param>
        /// <returns></returns>
        public static int GetDefaultFieldWidthByType(esriFieldType fieldType)
        {
            switch (fieldType)
            {
                case esriFieldType.esriFieldTypeString:
                    return DefaultStringWidth;

                case esriFieldType.esriFieldTypeInteger:
                    return DefaultIntWidth;

                case esriFieldType.esriFieldTypeSmallInteger:
                    return DefaultShortWidth;

                case esriFieldType.esriFieldTypeSingle:
                    return DefaultFloatWidth;

                case esriFieldType.esriFieldTypeDouble:
                    return DefaultDoubleWidth;

                default:
                    return 0;
            }
        }

        /// <summary>
        /// Get default field precision.
        /// For float, double, use to assign to scale property.
        /// For other type, return 0.
        /// <para>Scale (max decimal place, number after dot.)</para>
        /// </summary>
        /// <param name="fieldType"></param>
        /// <returns></returns>
        public static int GetDefaultFieldPrecisionByType(esriFieldType fieldType)
        {
            switch (fieldType)
            {
                case esriFieldType.esriFieldTypeSingle:
                    return DefaultFloatPrecision;

                case esriFieldType.esriFieldTypeDouble:
                    return DefaultDoublePrecision;

                case esriFieldType.esriFieldTypeString:
                case esriFieldType.esriFieldTypeInteger:
                case esriFieldType.esriFieldTypeSmallInteger:
                default:
                    return 0;
            }
        }

        public static void DeleteField(ITable table, string fieldName)
        {
            int inx = table.FindField(fieldName);
            if (inx == -1)
                return;

            IField field = table.Fields.Field[inx];
            table.DeleteField(field);
        }

        public static void DeleteFields(ITable table, List<string> fieldNames)
        {
            foreach (var fieldName in fieldNames)
            {
                DeleteField(table, fieldName);
            }
        }

        public static IField CloneField(IField field)
        {
            IClone clone = field as IClone;
            return clone.Clone() as IField;
        }

        public static IFields CloneFields(IFields fields)
        {
            IClone clone = fields as IClone;
            return clone.Clone() as IFields;
        }

        public static List<string> GetMissingFields(ITable table, List<string> fieldNames)
        {
            List<string> missingFields = new List<string>();
            foreach (var fieldName in fieldNames)
            {
                if (table.FindField(fieldName) == -1)
                {
                    missingFields.Add(fieldName);
                }
            }
            return missingFields;
        }

        public static string GetMissingFieldsMessage(ITable table, List<string> fieldNames)
        {
            List<string> missingFields = GetMissingFields(table, fieldNames);
            string tablePrintName = GetPrintName(table);
            return $"Field(s) not found in {tablePrintName} : \r\n- {string.Join("\r\n- ", missingFields)}";
        }

        public static string GetMissingFieldsMessageWithDatasourcePath(ITable table, List<string> fieldNames)
        {
            List<string> missingFields = GetMissingFields(table, fieldNames);
            string tablePrintName = GetPrintName(table);
            string dsPath = GetDataSourcePath(table);
            string message = $"Field(s) not found in {tablePrintName} :";
            message += $"{Environment.NewLine}{dsPath}";
            message += $"{Environment.NewLine}- {string.Join($"{Environment.NewLine}- ", missingFields)}";
            return message;
        }
        #endregion

        public static string GetDataSourcePath(ITable table)
        {
            // TODO: Test
            IDataset ds = table as IDataset;
            string dsPath = ds.Workspace.PathName;

            if (string.IsNullOrWhiteSpace(dsPath))
            {
                // SDE Datasource
                dsPath = TypeUtil.ToSdeConnectionInfo(ds.Workspace.ConnectionProperties).ToDsPathLikedString();
            }

            return dsPath;
        }

        public static string GetDataSourcePath(IFeatureClass fclass)
        {
            return GetDataSourcePath(fclass as ITable);
        }

        /// <summary>
        /// Check if has M value. Used to store route data.
        /// </summary>
        /// <param name="fclass"></param>
        /// <returns></returns>
        public static bool HasMValue (IFeatureClass fclass)
        {
            string shapeFieldName = fclass.ShapeFieldName;
            IFields fields = fclass.Fields;
            int geometryIndex = fields.FindField(shapeFieldName);
            IField field = fields.get_Field(geometryIndex);
            IGeometryDef geometryDef = field.GeometryDef;

            return geometryDef.HasM;
        }

        /// <summary>
        /// Check if has Z value. Used to store 3D data.
        /// </summary>
        /// <param name="fclass"></param>
        /// <returns></returns>
        public static bool HasZValue(IFeatureClass fclass)
        {
            string shapeFieldName = fclass.ShapeFieldName;
            IFields fields = fclass.Fields;
            int geometryIndex = fields.FindField(shapeFieldName);
            IField field = fields.get_Field(geometryIndex);
            IGeometryDef geometryDef = field.GeometryDef;

            return geometryDef.HasZ;
        }



    }
}
