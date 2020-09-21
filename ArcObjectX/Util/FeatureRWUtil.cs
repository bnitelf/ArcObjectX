using ArcObjectX.DataManagement;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ArcObjectX.Util
{
    public class FeatureRWUtil
    {
        #region Config
        // Read field config
        public static bool ThrowErrorIfFieldNotFound = false;

        /// <summary>
        /// Skip write to fields.
        /// </summary>
        public static List<string> SkipFields = new List<string>() { "SHAPE" };

        #endregion

        #region Get Layer Name
        public static string GetLayerName(IFeatureClass fclass)
        {
            string layerName = ((IDataset)fclass).Name;
            return layerName;
        }

        public static string GetLayerName(IFeature ft)
        {
            string layerName = ((IDataset)ft.Class).Name;
            return layerName;
        }

        public static string GetLayerName(ITable table)
        {
            string layerName = ((IDataset)table).Name;
            return layerName;
        }

        public static string GetLayerName(IRow row)
        {
            string layerName = ((IDataset)row.Table).Name;
            return layerName;
        }
        #endregion

        #region Read Field
        /// <summary>
        /// Get string value of the specified fieldName, if field not exist in row or is null value return default;
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string ReadStringIfAny(IRow row, string fieldName, string defaultValue = null)
        {
            string value = defaultValue;
            int index = row.Fields.FindField(fieldName);
            if (index != -1)
            {
                object objValue = row.Value[index];
                if (objValue != DBNull.Value)
                {
                    value = objValue as string;
                }
            }
            else
            {
                if (ThrowErrorIfFieldNotFound)
                {
                    throw new Exception(GetFieldNotFoundMsg(fieldName, row));
                }
            }
            return value;
        }

        /// <summary>
        /// Get int value of the specified fieldName, if field not exist in row or is null value return default;
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int ReadIntIfAny(IRow row, string fieldName, int defaultValue = 0)
        {
            int value = defaultValue;
            int index = row.Fields.FindField(fieldName);
            if (index != -1)
            {
                object objValue = row.Value[index];
                if (objValue != DBNull.Value)
                {
                    value = (int)objValue;
                }
            }
            else
            {
                if (ThrowErrorIfFieldNotFound)
                {
                    throw new Exception(GetFieldNotFoundMsg(fieldName, row));
                }
            }
            return value;
        }

        /// <summary>
        /// Get short value of the specified fieldName, if field not exist in row or is null value return default;
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static short ReadShortIfAny(IRow row, string fieldName, short defaultValue = 0)
        {
            short value = defaultValue;
            int index = row.Fields.FindField(fieldName);
            if (index != -1)
            {
                object objValue = row.Value[index];
                if (objValue != DBNull.Value)
                {
                    value = (short)objValue;
                }
            }
            else
            {
                if (ThrowErrorIfFieldNotFound)
                {
                    throw new Exception(GetFieldNotFoundMsg(fieldName, row));
                }
            }
            return value;
        }

        /// <summary>
        /// Get long value of the specified fieldName, if field not exist in row or is null value return default;
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static long ReadLongIfAny(IRow row, string fieldName, long defaultValue = 0)
        {
            long value = defaultValue;
            int index = row.Fields.FindField(fieldName);
            if (index != -1)
            {
                object objValue = row.Value[index];
                if (objValue != DBNull.Value)
                {
                    value = (long)objValue;
                }
            }
            else
            {
                if (ThrowErrorIfFieldNotFound)
                {
                    throw new Exception(GetFieldNotFoundMsg(fieldName, row));
                }
            }
            return value;
        }

        /// <summary>
        /// Get float value of the specified fieldName, if field not exist in row or is null value return default;
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static float ReadFloatIfAny(IRow row, string fieldName, float defaultValue = 0)
        {
            float value = defaultValue;
            int index = row.Fields.FindField(fieldName);
            if (index != -1)
            {
                object objValue = row.Value[index];
                if (objValue != DBNull.Value)
                {
                    value = (float)objValue;
                }
            }
            else
            {
                if (ThrowErrorIfFieldNotFound)
                {
                    throw new Exception(GetFieldNotFoundMsg(fieldName, row));
                }
            }
            return value;
        }

        /// <summary>
        /// Get double value of the specified fieldName, if field not exist in row or is null value return default;
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static double ReadDoubleIfAny(IRow row, string fieldName, double defaultValue = 0)
        {
            double value = defaultValue;
            int index = row.Fields.FindField(fieldName);
            if (index != -1)
            {
                object objValue = row.Value[index];
                if (objValue != DBNull.Value)
                {
                    value = (double)objValue;
                }
            }
            else
            {
                if (ThrowErrorIfFieldNotFound)
                {
                    throw new Exception(GetFieldNotFoundMsg(fieldName, row));
                }
            }
            return value;
        }

        /// <summary>
        /// Get DateTime value of the specified fieldName, if field not exist in row or is null value return default;
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static DateTime ReadDatetimeIfAny(IRow row, string fieldName, DateTime defaultValue)
        {
            DateTime value = defaultValue;
            int index = row.Fields.FindField(fieldName);
            if (index != -1)
            {
                object objValue = row.Value[index];
                if (objValue != DBNull.Value)
                {
                    value = (DateTime)objValue;
                }
            }
            else
            {
                if (ThrowErrorIfFieldNotFound)
                {
                    throw new Exception(GetFieldNotFoundMsg(fieldName, row));
                }
            }
            return value;
        }

        /// <summary>
        /// Get DateTime value of the specified fieldName, if field not exist in row or is null value return default;
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static DateTime? ReadDatetimeIfAny(IRow row, string fieldName, DateTime? defaultValue = null)
        {
            DateTime? value = defaultValue;
            int index = row.Fields.FindField(fieldName);
            if (index != -1)
            {
                object objValue = row.Value[index];
                if (objValue != DBNull.Value)
                {
                    value = objValue as DateTime?;
                }
            }
            else
            {
                if (ThrowErrorIfFieldNotFound)
                {
                    throw new Exception(GetFieldNotFoundMsg(fieldName, row));
                }
            }
            return value;
        }


        /// <summary>
        /// Get string value of the specified fieldName, if field not exist in row or is null value return default;
        /// </summary>
        /// <param name="ft"></param>
        /// <param name="fieldName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string ReadStringIfAny(IFeature ft, string fieldName, string defaultValue = null)
        {
            return ReadStringIfAny(ft as IRow, fieldName, defaultValue);
        }

        /// <summary>
        /// Get int value of the specified fieldName, if field not exist in row or is null value return default;
        /// </summary>
        /// <param name="ft"></param>
        /// <param name="fieldName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int ReadIntIfAny(IFeature ft, string fieldName, int defaultValue = 0)
        {
            return ReadIntIfAny(ft as IRow, fieldName, defaultValue);
        }

        /// <summary>
        /// Get short value of the specified fieldName, if field not exist in row or is null value return default;
        /// </summary>
        /// <param name="ft"></param>
        /// <param name="fieldName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static short ReadShortIfAny(IFeature ft, string fieldName, short defaultValue = 0)
        {
            return ReadShortIfAny(ft as IRow, fieldName, defaultValue);
        }

        /// <summary>
        /// Get long value of the specified fieldName, if field not exist in row or is null value return default;
        /// </summary>
        /// <param name="ft"></param>
        /// <param name="fieldName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static long ReadLongIfAny(IFeature ft, string fieldName, long defaultValue = 0)
        {
            return ReadLongIfAny(ft as IRow, fieldName, defaultValue);
        }

        /// <summary>
        /// Get float value of the specified fieldName, if field not exist in row or is null value return default;
        /// </summary>
        /// <param name="ft"></param>
        /// <param name="fieldName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static float ReadFloatIfAny(IFeature ft, string fieldName, float defaultValue = 0)
        {
            return ReadFloatIfAny(ft as IRow, fieldName, defaultValue);
        }

        /// <summary>
        /// Get double value of the specified fieldName, if field not exist in row or is null value return default;
        /// </summary>
        /// <param name="ft"></param>
        /// <param name="fieldName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static double ReadDoubleIfAny(IFeature ft, string fieldName, double defaultValue = 0)
        {
            return ReadDoubleIfAny(ft as IRow, fieldName, defaultValue);
        }

        /// <summary>
        /// Get DateTime value of the specified fieldName, if field not exist in row or is null value return default;
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static DateTime ReadDatetimeIfAny(IFeature ft, string fieldName, DateTime defaultValue)
        {
            return ReadDatetimeIfAny(ft as IRow, fieldName, defaultValue);
        }

        /// <summary>
        /// Get DateTime value of the specified fieldName, if field not exist in row or is null value return default;
        /// </summary>
        /// <param name="ft"></param>
        /// <param name="fieldName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static DateTime? ReadDatetimeIfAny(IFeature ft, string fieldName, DateTime? defaultValue = null)
        {
            return ReadDatetimeIfAny(ft as IRow, fieldName, defaultValue);
        }
        
        public static T Read<T>(IRow row)
        {
            Type t = typeof(T).GetType();
            PropertyInfo[] propertyInfos = t.GetProperties();
            PropertyInfo propertyInfo;

            string[] fieldNames = propertyInfos.Select(x => x.Name).ToArray();
            string fieldName;
            
            T record = (T)Activator.CreateInstance(typeof(T));
            
            int index = -1;
            object value;

            for (int i = 0; i < fieldNames.Length; i++)
            {
                fieldName = fieldNames[i];
                index = row.Fields.FindField(fieldName);

                if (index != -1)
                {
                    value = row.Value[index];

                    propertyInfo = t.GetProperty(fieldName);
                    if (propertyInfo != null)
                    {
                        propertyInfo.SetValue(record, value);
                    }
                }
            }

            return record;
        }
        #endregion

        #region Write Field
        /// <summary>
        /// Update new value to specified field name. Use defaultValue if new value is null.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        public static void Write(IRow row, string fieldName, object value, object defaultValue = null)
        {
            if (value == null)
                value = defaultValue;

            int index = row.Fields.FindField(fieldName);
            if (index != -1)
            {
                row.Value[index] = value;
            }
            else
            {
                if (ThrowErrorIfFieldNotFound)
                {
                    throw new Exception(GetFieldNotFoundMsg(fieldName, row));
                }
            }
        }

        public static void Write(IFeature ft, string fieldName, object value, object defaultValue = null)
        {
            Write(ft as IRow, fieldName, value, defaultValue);
        }



        public static void Write(IRow row, RecordData updateData)
        {
            string[] fieldNames = updateData.GetFieldNames();
            string fieldName;
            object value;

            for (int i = 0; i < fieldNames.Length; i++)
            {
                fieldName = fieldNames[i];
                value = updateData[fieldName];
                Write(row, fieldName, value);
            }
        }


        public static void Write<T>(IRow row, T updateData)
        {
            PropertyInfo[] propertyInfos = updateData.GetType().GetProperties();

            string fieldName;
            object value;

            for (int i = 0; i < propertyInfos.Length; i++)
            {
                fieldName = propertyInfos[i].Name;
                value = propertyInfos[i].GetValue(updateData);
                Write(row, fieldName, value);
            }
        }

        public static void CopyFieldsValue(IFeature ftSrc, IFeature ftDest, bool copyGeometry = true)
        {
            IFields fieldsSrc = ftSrc.Fields;
            IFields fieldsDest = ftDest.Fields;

            IField fieldSrc;
            IField fieldDest;

            int inxFieldSrc = -1;

            // Loop Dest fields
            for (int i = 0; i < fieldsDest.FieldCount; i++)
            {
                fieldDest = fieldsDest.Field[i];

                if (!fieldDest.Editable) continue;
                if (SkipFields.Contains(fieldDest.Name.ToUpper())) continue;

                inxFieldSrc = fieldsSrc.FindField(fieldDest.Name);
                if (inxFieldSrc != -1)
                {
                    ftDest.Value[i] = ftSrc.Value[inxFieldSrc];
                }
            }

            if (copyGeometry)
            {
                ftDest.Shape = ftSrc.ShapeCopy;
            }
        }
        #endregion

        #region Message
        private static string GetFieldNotFoundMsg(string fieldName, IRow row)
        {
            string msg = $"Field \"{fieldName}\" not exist in {GetLayerOrTableMsg(row)} {GetLayerName(row)}";
            return msg;
        }

        private static string GetFieldNotFoundMsg(string fieldName, IFeature ft)
        {
            string msg = $"Field \"{fieldName}\" not exist in {GetLayerOrTableMsg(ft)} {GetLayerName(ft)}";
            return msg;
        }

        private static string GetFieldNotFoundMsg(string fieldName, IFeatureClass fclass)
        {
            string msg = $"Field \"{fieldName}\" not exist in {GetLayerOrTableMsg(fclass)} {GetLayerName(fclass)}";
            return msg;
        }

        private static string GetFieldNotFoundMsg(string fieldName, ITable table)
        {
            string msg = $"Field \"{fieldName}\" not exist in {GetLayerOrTableMsg(table)} {GetLayerName(table)}";
            return msg;
        }

        /// <summary>
        /// Get string ว่า object ที่ส่งเข้ามาเป็น "layer" or "table"
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private static string GetLayerOrTableMsg(IRow row)
        {
            IFeature ft = row as IFeature;

            if (ft != null)
            {
                return "layer";
            }
            else
            {
                return "table";
            }
        }

        /// <summary>
        /// Get string ว่า object ที่ส่งเข้ามาเป็น "layer" or "table"
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private static string GetLayerOrTableMsg(ITable table)
        {
            IFeatureClass fclass = table as IFeatureClass;

            if (fclass != null)
            {
                return "layer";
            }
            else
            {
                return "table";
            }
        }

        /// <summary>
        /// Get string ว่า object ที่ส่งเข้ามาเป็น "layer" or "table"
        /// </summary>
        /// <param name="ft"></param>
        /// <returns></returns>
        private static string GetLayerOrTableMsg(IFeature ft)
        {
            return "layer";
        }

        /// <summary>
        /// Get string ว่า object ที่ส่งเข้ามาเป็น "layer" or "table"
        /// </summary>
        /// <param name="fclass"></param>
        /// <returns></returns>
        private static string GetLayerOrTableMsg(IFeatureClass fclass)
        {
            return "layer";
        }


        #endregion
    }
}
