using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

using ESRI.ArcGIS.Geodatabase;

namespace ArcObjectX.DataManagement.Tool
{
    public enum OutputType
    {
        GDB,
        MDB,
        SHP
    }

    public class ArcObjectExportTool<T>
    {
        private Action<int> _ActionCallBack;
        private OutputType _OutputType = OutputType.GDB;
        private int _SaveEveryNRecord;
        private List<T> _LstInfo;
        private IFeatureClass _SrcFeatureClass;
        private Dictionary<string, string> _DicMapping;

        private IFeatureClass _DesFeatureClass = null;

        public Action<int> SetCallBack
        {
            set
            {
                _ActionCallBack = value;
            }
        }

        /// <summary>
        /// Set value for Save data every specified record.
        /// </summary>
        public ArcObjectExportTool<T> SaveEveryNRecord(int nValue = 1000)
        {
            this._SaveEveryNRecord = nValue;

            return this;
        }

        /// <summary>
        /// Set Destination FeatureClass
        /// </summary>
        public ArcObjectExportTool<T> ExportTo(IFeatureClass featureClass)
        {
            this._DesFeatureClass = featureClass;

            return this;
        }

        /// <summary>
        /// Set List Object for Export
        /// </summary>
        public ArcObjectExportTool<T> ExportInfo(List<T> lstInfo)
        {
            this._LstInfo = lstInfo;

            return this;
        }

        /// <summary>
        /// Set source Featureclass for export (copy)
        /// </summary>
        public ArcObjectExportTool<T> ExportInfo(IFeatureClass featureClass)
        {
            this._SrcFeatureClass = featureClass;

            return this;
        }

        /// <summary>
        /// Mapping Field between Property field and destination field
        /// </summary>
        public ArcObjectExportTool<T> MappingField(Expression<Func<T , object>> expression , string desFieldName)
        {
            /// create dictionary for keep mapping 
            if (_DicMapping == null) _DicMapping = new Dictionary<string, string>();

            /// Get name from Expression
            string keyName = GetExpressionValue(expression);

            /// Add name to key of Dictionary
            _DicMapping.Add(keyName, desFieldName);

            return this;
        }

        /// <summary>
        /// Get value from expression
        /// </summary>
        private string GetExpressionValue(Expression expression)
        {
            LambdaExpression lambdaExpression = expression as LambdaExpression;

            MemberExpression memberExpression = (lambdaExpression.Body) as MemberExpression;

            return memberExpression.Member.Name;
        }

        /// <summary>
        /// Add Value to destination
        /// </summary>
        public void Insert()
        {
            int currentRow = 0;

            /// Check output is GDB or not
            if(_OutputType == OutputType.GDB)
            {
                /* 
                    Cast IFeatureClass to IFeatureClassLoad for set LoadOnlyMode
                    and because LoadOnlyMode can use only GDB and SDE type
                    LoadOnlyMode will improve performace for insert
                 */
                IFeatureClassLoad fClassLoad = _DesFeatureClass as IFeatureClassLoad;
                fClassLoad.LoadOnlyMode = true;
            }

            #region Old code

            /// Create IFeatureCursor for Insert 
            IFeatureCursor fCursor = _DesFeatureClass.Insert(true);

            /// Loop list Info
            foreach(T info in _LstInfo)
            {
                /// Create IFeatureBuffer and cast to IFeature
                IFeatureBuffer fBuffer = _DesFeatureClass.CreateFeatureBuffer();
                IFeature oFeature = fBuffer as IFeature;

                /// Add info to IFeature
                AddInfo2Feature(info, oFeature);

                /// Insert IFeatureBuffer to IFeatureCursor
                fCursor.InsertFeature(fBuffer);

                /// Save every N row
                if (currentRow % _SaveEveryNRecord == 0)
                {
                    fCursor.Flush();
                }

                /// Update Row 
                currentRow++;

                /// CallBack
                int percentTotal = CalPercentageFromTotal(currentRow, _LstInfo.Count);
                _ActionCallBack?.Invoke(percentTotal);
            }

            /// Save All Insert Feature
            fCursor.Flush();

            #endregion Old code
        }

        private void AddInfo2Feature(T info , IFeature oFeature)
        {
            /// Check Dictionary Mapping have value
            if (_DicMapping == null)
            {
                /// Get All property in Generic Type
                PropertyInfo[] propertyInfos = typeof(T).GetProperties();

                /// Loop all property in info
                for (int i = 0; i < propertyInfos.Length; i++)
                {
                    /// Find index in feature
                    int inx = oFeature.Fields.FindField(propertyInfos[i].Name);
                    if(inx >= 0)
                    {
                        /// Get value from info 
                        object objValue = propertyInfos.GetValue(i);

                        /// Check feature is NullAble and objValue is null set value DBNull.Value
                        if (oFeature.Fields.Field[inx].IsNullable == true && objValue == null)
                        {
                            oFeature.set_Value(inx, DBNull.Value);
                        }
                        else
                        {
                            oFeature.set_Value(inx, objValue);
                        }
                    }
                }
            }
            else
            {
                Type type = typeof(T);

                foreach(KeyValuePair<string , string> fieldMapping in _DicMapping)
                {
                    int inx = oFeature.Fields.FindField(fieldMapping.Value);
                    if(inx >= 0)
                    {
                        /// Get Value from info
                        object objValue = type.GetProperty(fieldMapping.Key).GetValue(info);

                        /// Check feature is NullAble and objValue is null set value DBNull.Value
                        if (oFeature.Fields.Field[inx].IsNullable == true && objValue == null)
                        {
                            oFeature.set_Value(inx, DBNull.Value);
                        }
                        else
                        {
                            oFeature.set_Value(inx, objValue);
                        }
                    }
                }
            }
        }

        private int CalPercentageFromTotal(int current, int total)
        {
            float percent = 0;
            percent = (float)current / (float)total * 100;
            return (int)percent;
        }
    }
}
