using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcObjectX.DataManagement
{
    public class RecordData
    {
        private Dictionary<string, object> data = new Dictionary<string, object>();

        /// <summary>
        /// Get or Set value of the specified field.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public object this[string fieldName]
        {
            get
            {
                object value = null;
                string fname = fieldName.ToUpper();
                if (data.ContainsKey(fname))
                {
                    value = data[fname];
                }
                else
                {
                    throw new MissingFieldException($"Cannot get the value of field {fieldName}. Not found field {fieldName} in this RecordData.");
                }
                return value;
            }

            set
            {
                object newValue = value;
                string fname = fieldName.ToUpper();
                if (!data.ContainsKey(fname))
                {
                    data.Add(fname, newValue);
                }
                else
                {
                    data[fname] = newValue;
                }
            }
        }

        /// <summary>
        /// Get All field names.
        /// </summary>
        /// <returns></returns>
        public string[] GetFieldNames()
        {
            return data.Select(x => x.Key).ToArray();
        }

        public int FieldCount { get { return data.Count; } }

        public Type GetFieldType(string fieldName)
        {
            object value = this[fieldName];
            return value.GetType();
        }
    }
}
