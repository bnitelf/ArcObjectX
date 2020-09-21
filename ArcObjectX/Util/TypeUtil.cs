using ArcObjectX.DataManagement;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcObjectX.Util
{
    public class TypeUtil
    {
        /// <summary>
        /// Convert C# type to esriFieldType.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static esriFieldType ToEsriFieldType(Type type)
        {
            esriFieldType esriFieldType;

            if (type == typeof(short))
            {
                return esriFieldType.esriFieldTypeSmallInteger;
            }
            else if (type == typeof(int) ||
                     type == typeof(long))
            {
                return esriFieldType.esriFieldTypeInteger;
            }
            else if (type == typeof(float))
            {
                return esriFieldType.esriFieldTypeSingle;
            }
            else if (type == typeof(double))
            {
                return esriFieldType.esriFieldTypeDouble;
            }
            else if (type == typeof(DateTime))
            {
                return esriFieldType.esriFieldTypeDate;
            }
            else if (type == typeof(string))
            {
                return esriFieldType.esriFieldTypeString;
            }
            else
            {
                return esriFieldType.esriFieldTypeString;
            }
        }

        /// <summary>
        /// Convert to <see cref="esriFieldType"/>. Valid values are, 
        /// <para>esriFieldType string ex. "esriFieldTypeString"</para>
        /// <para>(TEXT, STRING), (FLOAT, SINGLE), DOUBLE, SHORT, (INT, LONG), (DATE, DATETIME), BLOB, RASTER, GUID, GLOBALID, GEOMETRY</para>
        /// <para>Keywords in parenthesis () will result the same esriFieldType.</para>
        /// </summary>
        /// <param name="fieldType"></param>
        /// <returns></returns>
        public static esriFieldType ToEsriFieldType(string fieldType)
        {
            if (string.IsNullOrWhiteSpace(fieldType))
            {
                throw new ArgumentException("fieldType must not null or blank.");
            }

            esriFieldType esriFieldType;

            if (Enum.TryParse<esriFieldType>(fieldType, out esriFieldType))
            {
                return esriFieldType;
            }

            switch (fieldType.Trim().ToUpper())
            {
                case "STRING":
                case "TEXT":
                    return esriFieldType.esriFieldTypeString;

                case "SHORT":
                    return esriFieldType.esriFieldTypeSmallInteger;

                case "INT":
                case "LONG":
                    return esriFieldType.esriFieldTypeInteger;

                case "FLOAT":
                case "SINGLE":
                    return esriFieldType.esriFieldTypeSingle;

                case "DOUBLE":
                    return esriFieldType.esriFieldTypeDouble;

                case "DATE":
                case "DATETIME":
                    return esriFieldType.esriFieldTypeDate;

                case "BLOB":
                    return esriFieldType.esriFieldTypeBlob;

                case "RASTER":
                    return esriFieldType.esriFieldTypeRaster;

                case "GUID":
                    return esriFieldType.esriFieldTypeGUID;

                case "GLOBALID":
                    return esriFieldType.esriFieldTypeGlobalID;

                case "GEOMETRY":
                    return esriFieldType.esriFieldTypeGeometry;

                default:
                    throw new ArgumentException($"fieldType \"{fieldType}\" is not a valid value.");
            }
        }

        public static SdeConnectionInfo ToSdeConnectionInfo(IPropertySet propertySet)
        {
            Dictionary<string, string> dictProperties = ToDictionaryIPropertySet(propertySet);

            SdeConnectionInfo sdeConnectionInfo = new SdeConnectionInfo();
            if (dictProperties.ContainsKey("SERVER")) sdeConnectionInfo.Server = dictProperties["SERVER"];
            if (dictProperties.ContainsKey("DATABASE")) sdeConnectionInfo.DBName = dictProperties["DATABASE"];
            if (dictProperties.ContainsKey("USER")) sdeConnectionInfo.User = dictProperties["USER"];
            if (dictProperties.ContainsKey("VERSION")) sdeConnectionInfo.Version = dictProperties["VERSION"];

            return sdeConnectionInfo;
        }

        public static Dictionary<string, string> ToDictionaryIPropertySet(IPropertySet propertySet)
        {
            Dictionary<string, string> dictProperties = new Dictionary<string, string>();

            object[] nameArray = new object[1];
            object[] valueArray = new object[1];
            propertySet.GetAllProperties(out nameArray[0], out valueArray[0]);
            object[] names = (object[])nameArray[0];
            object[] values = (object[])valueArray[0];

            for (int i = 0; i < propertySet.Count; i++)
            {
                string nameString = names[i].ToString().ToUpper();
                string valueString = values[i].ToString();

                dictProperties.Add(nameString, valueString);
            }
            return dictProperties;
        }
    }
}
