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
        SHP,
        SDE
    }

    public class ArcObjectExportTool<T>
    {        
        private OutputType _OutputType;
        private int _SaveEveryNRecord = 0;
        private List<T> _LstInfo = null;
        private bool _IsExportAll = true;
        private Dictionary<string, string> _DicMapping = null;

        private IFeatureClass _DesFeatureClass = null;

        public ArcObjectExportTool<T> OutputType(OutputType outputType)
        {
            this._OutputType = outputType;

            return this;
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

        public ArcObjectExportTool<T> ExportAll(bool isExportAll = true)
        {
            this._IsExportAll = isExportAll;

            return this;
        }

        public ArcObjectExportTool<T> MappingField(Expression<Func<T , object>> expression , string desFieldName)
        {
            // Get name from Expression
            string keyName = GetExpressionValue(expression);

            // Add name to key of Dictionary
            _DicMapping.Add(keyName, desFieldName);

            return this;
        }

        private string GetExpressionValue(Expression expression)
        {
            LambdaExpression lambdaExpression = expression as LambdaExpression;

            MemberExpression memberExpression = (lambdaExpression.Body) as MemberExpression;

            return memberExpression.Member.Name;
        }

        public void Insert()
        {
            int currentRow = 0;

            // Check output is GDB or not
            if(_OutputType == Tool.OutputType.GDB)
            {
                // Cast IFeatureClass to IFeatureClassLoad for set LoadOnlyMode
                // because LoadOnlyMode will improve performace for insert
                // LoadOnlyMode can use only GDB and SDE type
                IFeatureClassLoad fClassLoad = _DesFeatureClass as IFeatureClassLoad;
                fClassLoad.LoadOnlyMode = true;
            }
            

            // Create IFeatureCursor for Insert 
            IFeatureCursor fCursor = _DesFeatureClass.Insert(true);

            // Loop list Info
            foreach(T info in _LstInfo)
            {
                // Create IFeatureBuffer and cast to IFeature
                IFeatureBuffer fBuffer = _DesFeatureClass.CreateFeatureBuffer();
                IFeature oFeature = fBuffer as IFeature;

                // Add info to IFeature
                AddInfo2Feature(info, oFeature);

                // Insert IFeatureBuffer to IFeatureCursor
                fCursor.InsertFeature(fBuffer);

                // Save every N row
                if (currentRow % _SaveEveryNRecord == 0)
                {
                    fCursor.Flush();
                }

                // + Row 
                currentRow++;
            }

            // Save All Insert Feature
            fCursor.Flush();
        }

        private void AddInfo2Feature(T info , IFeature oFeature)
        {
            // Check export all or export only specified property
            if (_IsExportAll == false)
            {
                // Loop all property in info
                PropertyInfo[] propertyInfos = typeof(T).GetProperties();
                for(int i = 0; i < propertyInfos.Length; i++)
                {
                    // Find index in feature
                    int inx = oFeature.Fields.FindField(propertyInfos[i].Name);
                    if(inx >= 0)
                    {
                        // Get value from info 
                        object objValue = propertyInfos.GetValue(i);

                        // Check feature is NullAble and objValue is null set value DBNull.Value
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
    }
}
