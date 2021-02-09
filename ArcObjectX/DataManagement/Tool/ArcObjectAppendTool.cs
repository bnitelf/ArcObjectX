using ArcObjectX.Util;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ArcObjectX.Util.Extension;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcObjectX.DataManagement.Tool
{
    public class ArcObjectAppendTool
    {
        /// <summary>
        /// Update progress callback every N percent.
        /// ex. 10 will trigger <see cref="ProgressCallback"/> every 10%
        /// </summary>
        public int PercentCallbackInterval { get; set; }

        /// <summary>
        /// Update progress callback function.
        /// </summary>
        public Action<int> ProgressCallback { get; set; }

        /// <summary>
        /// For converting value before set field.
        /// สำหรับทำ custom แปลงค่าก่อนที่จะ set field value.
        /// 
        /// <para>
        /// For use with Append function with IFeature-based object. (use with FeatureClass)
        /// </para>
        /// 
        /// Dict (FieldName, Callback function call when loop setting the specified fieldName.)
        /// Callback function จะถูก call เมื่อกำลัง loop set fieldName ตาม dict key อยู่เพื่อให้
        /// Callback function ((in) current src row, (in) field index of field in dict key, (out) output value)
        /// </summary>
        public Dictionary<string, Func<IFeature, int, object>> DictConvertFeatureFieldValueFunc { get; set; }


        /// <summary>
        /// For converting value before set field.
        /// สำหรับทำ custom แปลงค่าก่อนที่จะ set field value.
        /// 
        /// <para>
        /// For use with Append function with IRow-based object. (use with Table)
        /// </para>
        /// 
        /// Dict (FieldName, Callback function call when loop setting the specified fieldName.)
        /// Callback function จะถูก call เมื่อกำลัง loop set fieldName ตาม dict key อยู่เพื่อให้
        /// Callback function ((in) current src row, (in) field index of field in dict key, (out) output value)
        public Dictionary<string, Func<IRow, int, object>> DictConvertRowFieldValueFunc { get; set; }


        /// <summary>
        /// For converting value before set field.
        /// สำหรับทำ custom แปลงค่าก่อนที่จะ set field value.
        /// 
        /// <para>
        /// For use with Append function with SqlDataReader-based object. (use with Table from SqlDataReader)
        /// </para>
        /// 
        /// Dict (FieldName, Callback function call when loop setting the specified fieldName.)
        /// Callback function จะถูก call เมื่อกำลัง loop set fieldName ตาม dict key อยู่เพื่อให้
        /// Callback function ((in) current src row, (in) field index of field in dict key, (out) output value)
        public Dictionary<string, Func<SqlDataReader, int, object>> DictConvertSqlFieldValueFunc { get; set; }


        //  skip fields
        /// <summary>
        /// Fields to skip
        /// </summary>
        public HashSet<string> SkipFields { get; set; }

        public static string ShapeFieldName = "Shape";

        public ArcObjectAppendTool()
        {
            Init();
        }

        private void Init()
        {
            PercentCallbackInterval = 10;
            DictConvertFeatureFieldValueFunc = new Dictionary<string, Func<IFeature, int, object>>();
            DictConvertRowFieldValueFunc = new Dictionary<string, Func<IRow, int, object>>();
            DictConvertSqlFieldValueFunc = new Dictionary<string, Func<SqlDataReader, int, object>>();

            SkipFields = new HashSet<string>();
        }

        private Dictionary<string, Func<IRow, int, object>> MakeKeyToUpper(Dictionary<string, Func<IRow, int, object>> dictConvertFieldValueFunc)
        {
            // make key Upper
            Dictionary<string, Func<IRow, int, object>> dictConvertFieldValueFuncKeyUpper = new Dictionary<string, Func<IRow, int, object>>();

            if (dictConvertFieldValueFunc != null)
            {
                foreach (var item in dictConvertFieldValueFunc)
                {
                    dictConvertFieldValueFuncKeyUpper.Add(item.Key.ToUpper(), item.Value);
                }
            }
            return dictConvertFieldValueFuncKeyUpper;
        }

        private Dictionary<string, Func<IFeature, int, object>> MakeKeyToUpper(Dictionary<string, Func<IFeature, int, object>> dictConvertFieldValueFunc)
        {
            // make key Upper
            Dictionary<string, Func<IFeature, int, object>> dictConvertFieldValueFuncKeyUpper = new Dictionary<string, Func<IFeature, int, object>>();

            if (dictConvertFieldValueFunc != null)
            {
                foreach (var item in dictConvertFieldValueFunc)
                {
                    dictConvertFieldValueFuncKeyUpper.Add(item.Key.ToUpper(), item.Value);
                }
            }
            return dictConvertFieldValueFuncKeyUpper;
        }

        private Dictionary<string, Func<SqlDataReader, int, object>> MakeKeyToUpper(Dictionary<string, Func<SqlDataReader, int, object>> dictConvertFieldValueFunc)
        {
            // make key Upper
            Dictionary<string, Func<SqlDataReader, int, object>> dictConvertFieldValueFuncKeyUpper = new Dictionary<string, Func<SqlDataReader, int, object>>();

            if (dictConvertFieldValueFunc != null)
            {
                foreach (var item in dictConvertFieldValueFunc)
                {
                    dictConvertFieldValueFuncKeyUpper.Add(item.Key.ToUpper(), item.Value);
                }
            }
            return dictConvertFieldValueFuncKeyUpper;
        }

        /// <summary>
        /// Make DictConvertFieldValue key to upper to make check field name case insensitive.
        /// </summary>
        private void MakeAllDictConvertFieldValueKeyUpper()
        {
            if (DictConvertFeatureFieldValueFunc != null)
            {
                DictConvertFeatureFieldValueFunc = MakeKeyToUpper(DictConvertFeatureFieldValueFunc);
            }

            if (DictConvertRowFieldValueFunc != null)
            {
                DictConvertRowFieldValueFunc = MakeKeyToUpper(DictConvertRowFieldValueFunc);
            }

            if (DictConvertSqlFieldValueFunc != null)
            {
                DictConvertSqlFieldValueFunc = MakeKeyToUpper(DictConvertSqlFieldValueFunc);
            }
        }

        private int CalPercent(int current, int total)
        {
            float percent = 0;
            percent = (float)current / (float)total * 100;
            return (int)percent;
        }

        /// <summary>
        /// Append source FeatureCursor to Destination FeatureClass.
        /// Using IFeatureClassLoad.LoadOnlyMode = true.
        /// Support destination data format, SDE (FeatureClass), FileGDB (FeatureClass, Table).
        /// 
        /// <para>
        /// <see href="http://resources.esri.com/help/9.3/ArcGISDesktop/ArcObjects/esriGeoDatabase/IFeatureClassLoad.htm">
        /// More info IFeatureClassLoad</see>
        /// </para>
        /// </summary>
        /// <param name="ftCursorSrc"></param>
        /// <param name="fclassDest"></param>
        /// <param name="totalSrcRecord">Total record to append. Use for update progress callback.</param>
        /// <param name="flushDataEveryNRecord">Flush data every specified record.</param>
        /// <returns>Number of row appended.</returns>
        public int FastAppendUsingLoadOnlyMode(IFeatureCursor ftCursorSrc, IFeatureClass fclassDest, int totalSrcRecord, int flushDataEveryNRecord = 10000)
        {
            // Check and throw Exception
            if (ftCursorSrc == null)
            {
                // No data to append.
                return 0;
            }

            if (fclassDest == null)
            {
                throw new ArgumentNullException("fclassDest", "Destination FeatureClass must not be null.");
            }

            IFeatureClassLoad fclassLoadDest = fclassDest as IFeatureClassLoad;
            ISchemaLock schemaLockDest = fclassDest as ISchemaLock;
            IFeatureCursor ftCursorDest = null;
            IFeatureBuffer ftBufferDest = null;

            IFeature ftSrc = null;
            IFeature ftDest = null;
            IFields fieldsSrc = ftCursorSrc.Fields;
            IFields fieldsDest = fclassDest.Fields;

            IField fieldSrc;
            IField fieldDest;

            int inxFieldSrc = -1;

            // Skip fields
            SkipFields.Add(fclassDest.OIDFieldName.ToUpper());
            //SkipFields.Add(fclassDest.ShapeFieldName.ToUpper());

            string layerNameDest = ((IDataset)fclassDest).Name;

            // Check if LoadOnlyMode support
            if (fclassLoadDest == null)
            {
                throw new InvalidOperationException($"FastAppend (LoadOnlyMode) is not support for layer \"{layerNameDest}\"");
            }

            int updateInterval = PercentCallbackInterval;
            int nextUpdatePercent = updateInterval;
            int currentRow = 0;
            int percent = 0;
            int totalRecord = totalSrcRecord;

            try
            {
                // Create an insert cursor
                ftCursorDest = fclassDest.Insert(true);


                // Set schema lock and switch to Load Only Mode
                schemaLockDest.ChangeSchemaLock(esriSchemaLock.esriExclusiveSchemaLock);
                fclassLoadDest.LoadOnlyMode = true;

                // Make key Upper (for case-insensitive field name check)
                MakeAllDictConvertFieldValueKeyUpper();
                Func<IFeature, int, object> funcConvertValue = null;

                // Loop Src Record
                while ((ftSrc = ftCursorSrc.NextFeature()) != null)
                {
                    ftBufferDest = fclassDest.CreateFeatureBuffer();
                    ftDest = ftBufferDest as IFeature;

                    // Graphic
                    //ftDest.Shape = ftSrc.Shape;

                    // Attribute
                    // Loop Dest fields
                    CopyFieldsValue(ftSrc, ftDest);


                    ftCursorDest.InsertFeature(ftBufferDest);
                    ftBufferDest = null;

                    // Flush every 10000 row
                    if (currentRow % flushDataEveryNRecord == 0)
                    {
                        ftCursorDest.Flush();
                    }

                    // Update progess
                    currentRow++;
                    percent = CalPercent(currentRow, totalRecord);

                    if (percent >= nextUpdatePercent)
                    {
                        ProgressCallback?.Invoke(percent);
                        nextUpdatePercent += updateInterval;
                    }
                }

                ftCursorDest.Flush();
            }
            finally
            {
                // Clean up
                schemaLockDest?.ChangeSchemaLock(esriSchemaLock.esriSharedSchemaLock);

                if (fclassLoadDest != null)
                    fclassLoadDest.LoadOnlyMode = false;

                ComReleaser.ReleaseCOMObject(ftCursorSrc);
                ftCursorSrc = null;

                if (ftCursorDest != null)
                {
                    ComReleaser.ReleaseCOMObject(ftCursorDest);
                    ftCursorDest = null;
                }
            }

            return currentRow;
        }


        /// <summary>
        /// Append source Cursor to Destination FeatureClass.
        /// Using IFeatureClassLoad.LoadOnlyMode = true.
        /// Support destination data format, SDE (FeatureClass), FileGDB (FeatureClass, Table).
        /// 
        /// <para>
        /// <see href="http://resources.esri.com/help/9.3/ArcGISDesktop/ArcObjects/esriGeoDatabase/IFeatureClassLoad.htm">
        /// More info IFeatureClassLoad</see>
        /// </para>
        /// </summary>
        /// <param name="cursorSrc"></param>
        /// <param name="fclassDest"></param>
        /// <param name="totalSrcRecord">Total record to append. Use for update progress callback.</param>
        /// <param name="flushDataEveryNRecord">Flush data every specified record.</param>
        /// <returns>Number of row appended.</returns>
        public int FastAppendUsingLoadOnlyMode(ICursor cursorSrc, IFeatureClass fclassDest, int totalSrcRecord, int flushDataEveryNRecord = 10000)
        {
            // Check and throw Exception
            if (cursorSrc == null)
            {
                // No data to append.
                return 0;
            }

            if (fclassDest == null)
            {
                throw new ArgumentNullException("fclassDest", "Destination FeatureClass must not be null.");
            }

            IFeatureClassLoad fclassLoadDest = fclassDest as IFeatureClassLoad;
            ISchemaLock schemaLockDest = fclassDest as ISchemaLock;
            IFeatureCursor ftCursorDest = null;
            IFeatureBuffer ftBufferDest = null;

            IRow rowSrc = null;
            IFeature ftSrc = null;
            IFeature ftDest = null;
            IFields fieldsSrc = cursorSrc.Fields;
            IFields fieldsDest = fclassDest.Fields;

            IField fieldSrc;
            IField fieldDest;

            int inxFieldSrc = -1;

            // Skip fields
            SkipFields.Add(fclassDest.OIDFieldName.ToUpper());
            SkipFields.Add(fclassDest.ShapeFieldName.ToUpper());

            string layerNameDest = ((IDataset)fclassDest).Name;

            // Check if LoadOnlyMode support
            if (fclassLoadDest == null)
            {
                throw new InvalidOperationException($"FastAppend (LoadOnlyMode) is not support for layer \"{layerNameDest}\"");
            }

            int updateInterval = PercentCallbackInterval;
            int nextUpdatePercent = updateInterval;
            int currentRow = 0;
            int percent = 0;
            int totalRecord = totalSrcRecord;

            try
            {
                // Create an insert cursor
                ftCursorDest = fclassDest.Insert(true);


                // Set schema lock and switch to Load Only Mode
                schemaLockDest.ChangeSchemaLock(esriSchemaLock.esriExclusiveSchemaLock);
                fclassLoadDest.LoadOnlyMode = true;

                // Make key Upper (for case-insensitive field name check)
                MakeAllDictConvertFieldValueKeyUpper();
                Func<IRow, int, object> funcConvertValue = null;

                // Loop Src Record
                while ((rowSrc = cursorSrc.NextRow()) != null)
                {
                    ftBufferDest = fclassDest.CreateFeatureBuffer();
                    ftDest = ftBufferDest as IFeature;

                    // Graphic
                    CopyShapeFieldValue(rowSrc, ftDest);
                    

                    // Attribute
                    // Loop Dest fields
                    CopyFieldsValue(ftSrc, ftDest);


                    ftCursorDest.InsertFeature(ftBufferDest);
                    ftBufferDest = null;

                    // Flush every 10000 row
                    if (currentRow % flushDataEveryNRecord == 0)
                    {
                        ftCursorDest.Flush();
                    }

                    // Update progess
                    currentRow++;
                    percent = CalPercent(currentRow, totalRecord);

                    if (percent >= nextUpdatePercent)
                    {
                        ProgressCallback?.Invoke(percent);
                        nextUpdatePercent += updateInterval;
                    }
                }

                ftCursorDest.Flush();
            }
            finally
            {
                // Clean up
                schemaLockDest?.ChangeSchemaLock(esriSchemaLock.esriSharedSchemaLock);

                if (fclassLoadDest != null)
                    fclassLoadDest.LoadOnlyMode = false;

                ComReleaser.ReleaseCOMObject(cursorSrc);
                cursorSrc = null;

                if (ftCursorDest != null)
                {
                    ComReleaser.ReleaseCOMObject(ftCursorDest);
                    ftCursorDest = null;
                }
            }

            return currentRow;
        }


        /// <summary>
        /// Append source SqlDataReader to Destination FeatureClass.
        /// Using IFeatureClassLoad.LoadOnlyMode = true.
        /// Support destination data format, SDE (FeatureClass), FileGDB (FeatureClass, Table).
        /// 
        /// <para>
        /// <see href="http://resources.esri.com/help/9.3/ArcGISDesktop/ArcObjects/esriGeoDatabase/IFeatureClassLoad.htm">
        /// More info IFeatureClassLoad</see>
        /// </para>
        /// </summary>
        /// <param name="readerSrc"></param>
        /// <param name="fclassDest"></param>
        /// <param name="totalSrcRecord">Total record to append. Use for update progress callback.</param>
        /// <param name="flushDataEveryNRecord">Flush data every specified record.</param>
        /// <returns>Number of row appended.</returns>
        public int FastAppendUsingLoadOnlyMode(SqlDataReader readerSrc, IFeatureClass fclassDest, int totalSrcRecord, int flushDataEveryNRecord = 10000)
        {
            // Check and throw Exception
            if (readerSrc == null || !readerSrc.HasRows)
            {
                // No data to append.
                return 0;
            }

            if (fclassDest == null)
            {
                throw new ArgumentNullException("fclassDest", "Destination FeatureClass must not be null.");
            }


            IFeatureClassLoad fclassLoadDest = fclassDest as IFeatureClassLoad;
            ISchemaLock schemaLockDest = fclassDest as ISchemaLock;
            IFeatureCursor ftCursorDest = null;
            IFeatureBuffer ftBufferDest = null;

            //IFeature ftSrc = null;
            IFeature ftDest = null;
            //IFields fieldsSrc = ftCursorSrc.Fields;
            IFields fieldsDest = fclassDest.Fields;

            //IField fieldSrc;
            IField fieldDest;

            int inxFieldSrc = -1;

            string layerNameDest = ((IDataset)fclassDest).Name;

            // Skip fields
            SkipFields.Add(fclassDest.OIDFieldName.ToUpper());
            SkipFields.Add(fclassDest.ShapeFieldName.ToUpper());

            // Check if LoadOnlyMode support
            if (fclassLoadDest == null)
            {
                throw new InvalidOperationException($"FastAppend (LoadOnlyMode) is not support for layer \"{layerNameDest}\"");
            }

            int updateInterval = PercentCallbackInterval;
            int nextUpdatePercent = updateInterval;
            int currentRow = 0;
            int percent = 0;
            int totalRecord = totalSrcRecord;

            try
            {
                // Create an insert cursor
                ftCursorDest = fclassDest.Insert(true);


                // Set schema lock and switch to Load Only Mode
                schemaLockDest.ChangeSchemaLock(esriSchemaLock.esriExclusiveSchemaLock);
                fclassLoadDest.LoadOnlyMode = true;

                // Make key Upper (for case-insensitive field name check)
                Dictionary<string, Func<SqlDataReader, int, object>> dictConvertFieldValueFuncKeyUpper;
                dictConvertFieldValueFuncKeyUpper = MakeKeyToUpper(DictConvertSqlFieldValueFunc);
                Func<SqlDataReader, int, object> funcConvertValue = null;


                // Loop Src Record
                while (readerSrc.Read())
                {
                    ftBufferDest = fclassDest.CreateFeatureBuffer();
                    ftDest = ftBufferDest as IFeature;

                    // Graphic
                    CopyShapeFieldValueFromSqlDataReader(readerSrc, ftDest);

                    // Attribute
                    CopyFieldsValue(readerSrc, ftDest);


                    ftCursorDest.InsertFeature(ftBufferDest);
                    ftBufferDest = null;

                    // Flush every 10000 row
                    if (currentRow % flushDataEveryNRecord == 0)
                    {
                        ftCursorDest.Flush();
                    }

                    // Update progess
                    currentRow++;
                    percent = CalPercent(currentRow, totalRecord);

                    if (percent >= nextUpdatePercent)
                    {
                        ProgressCallback?.Invoke(percent);
                        nextUpdatePercent += updateInterval;
                    }
                }

                ftCursorDest.Flush();
            }
            finally
            {
                // Clean up
                schemaLockDest?.ChangeSchemaLock(esriSchemaLock.esriSharedSchemaLock);

                if (fclassLoadDest != null)
                    fclassLoadDest.LoadOnlyMode = false;

                readerSrc.Close();

                if (ftCursorDest != null)
                {
                    ComReleaser.ReleaseCOMObject(ftCursorDest);
                    ftCursorDest = null;
                }
            }

            return currentRow;
        }

        /// <summary>
        /// Append source SqlDataReader to Destination Table.
        /// Using IFeatureClassLoad.LoadOnlyMode = true.
        /// Support destination data format, SDE (FeatureClass), FileGDB (FeatureClass, Table).
        /// 
        /// <para>
        /// <see href="http://resources.esri.com/help/9.3/ArcGISDesktop/ArcObjects/esriGeoDatabase/IFeatureClassLoad.htm">
        /// More info IFeatureClassLoad</see>
        /// </para>
        /// </summary>
        /// <param name="readerSrc"></param>
        /// <param name="tableDest"></param>
        /// <param name="totalSrcRecord">Total record to append. Use for update progress callback.</param>
        /// <param name="flushDataEveryNRecord">Flush data every specified record.</param>
        /// <returns>Number of row appended.</returns>
        public int FastAppendUsingLoadOnlyMode(SqlDataReader readerSrc, ITable tableDest, int totalSrcRecord, int flushDataEveryNRecord = 10000)
        {
            // Check and throw Exception
            if (readerSrc == null || !readerSrc.HasRows)
            {
                // No data to append.
                return 0;
            }

            if (tableDest == null)
            {
                throw new ArgumentNullException("fclassDest", "Destination FeatureClass must not be null.");
            }


            IFeatureClassLoad fclassLoadDest = tableDest as IFeatureClassLoad;
            ISchemaLock schemaLockDest = tableDest as ISchemaLock;
            ICursor cursorDest = null;
            IRowBuffer rowBufferDest = null;

            //IFeature ftSrc = null;
            IRow rowDest = null;
            //IFields fieldsSrc = ftCursorSrc.Fields;
            IFields fieldsDest = tableDest.Fields;

            //IField fieldSrc;
            IField fieldDest;

            int inxFieldSrc = -1;

            string tableNameDest = ((IDataset)tableDest).Name;

            // Skip fields
            SkipFields.Add(tableDest.OIDFieldName.ToUpper());

            // Check if LoadOnlyMode support
            if (fclassLoadDest == null)
            {
                throw new InvalidOperationException($"FastAppend (LoadOnlyMode) is not support for table \"{tableNameDest}\"");
            }

            int updateInterval = PercentCallbackInterval;
            int nextUpdatePercent = updateInterval;
            int currentRow = 0;
            int percent = 0;
            int totalRecord = totalSrcRecord;

            try
            {
                // Create an insert cursor
                cursorDest = tableDest.Insert(true);


                // Set schema lock and switch to Load Only Mode
                schemaLockDest.ChangeSchemaLock(esriSchemaLock.esriExclusiveSchemaLock);
                fclassLoadDest.LoadOnlyMode = true;

                // Make key Upper (for case-insensitive field name check)
                MakeAllDictConvertFieldValueKeyUpper();
                Func<SqlDataReader, int, object> funcConvertValue = null;


                // Loop Src Record
                while (readerSrc.Read())
                {
                    rowBufferDest = tableDest.CreateRowBuffer();
                    rowDest = rowBufferDest as IRow;

                    // Attribute
                    CopyFieldsValue(readerSrc, rowDest);


                    cursorDest.InsertRow(rowBufferDest);
                    rowBufferDest = null;

                    // Flush every 10000 row
                    if (currentRow % flushDataEveryNRecord == 0)
                    {
                        cursorDest.Flush();
                    }

                    // Update progess
                    currentRow++;
                    percent = CalPercent(currentRow, totalRecord);

                    if (percent >= nextUpdatePercent)
                    {
                        ProgressCallback?.Invoke(percent);
                        nextUpdatePercent += updateInterval;
                    }
                }

                cursorDest.Flush();
            }
            finally
            {
                // Clean up
                schemaLockDest?.ChangeSchemaLock(esriSchemaLock.esriSharedSchemaLock);

                if (fclassLoadDest != null)
                    fclassLoadDest.LoadOnlyMode = false;

                readerSrc.Close();

                if (cursorDest != null)
                {
                    ComReleaser.ReleaseCOMObject(cursorDest);
                    cursorDest = null;
                }
            }

            return currentRow;
        }


        /// <summary>
        /// Append to Dest Feature Class. Auto map same field name. 
        /// Filter export only objectID in listFilterObjectIDs of source FeatureClass.
        /// </summary>
        /// <param name="fclassSrc"></param>
        /// <param name="fclassDest"></param>
        /// <param name="listFilterObjectIDs"></param>
        public int Append(IFeatureClass fclassSrc, IFeatureClass fclassDest, List<int> listFilterObjectIDs)
        {
            if (fclassSrc == null)
            {
                return 0;
            }

            if (fclassDest == null)
            {
                throw new ArgumentNullException("fclassDest", "Destination FeatureClass must not be null.");
            }

            if (listFilterObjectIDs == null)
            {
                throw new ArgumentNullException("listFilterObjectIDs", "listFilterObjectIDs must not be null.");
            }

            if (listFilterObjectIDs.Count == 0)
            {
                return 0;
            }

            List<List<int>> listParts = CollectionUtil.SplitToParts(listFilterObjectIDs, 2000);
            int part = 0;
            string sqlWhereIn;

            IQueryFilter filter;
            IFeatureCursor ftCursorSrc = null;

            IFeature ftSrc;
            IFeature ftDest;

            // Make key Upper
            MakeAllDictConvertFieldValueKeyUpper();

            int count = 0;
            int total = listFilterObjectIDs.Count;
            int percent = 0;
            int updateInterval = PercentCallbackInterval;
            int nextUpdatePercent = updateInterval;

            try
            {
                foreach (var listObjectIDPart in listParts)
                {
                    part++;

                    sqlWhereIn = SqlUtil.CreateWhereIn(fclassSrc.OIDFieldName, listObjectIDPart);

                    filter = new QueryFilterClass();
                    filter.WhereClause = sqlWhereIn;

                    ftCursorSrc = fclassSrc.Search(filter, false);

                    // Loop Src Record
                    while ((ftSrc = ftCursorSrc.NextFeature()) != null)
                    {
                        ftDest = fclassDest.CreateFeature();

                        // Graphic
                        CopyShapeFieldValue(ftSrc, ftDest);

                        // Attribute
                        CopyFieldsValue(ftSrc, ftDest);

                        // Save 
                        ftDest.Store();

                        count++;

                        percent = CalPercent(count, total);
                        if (percent >= nextUpdatePercent)
                        {
                            ProgressCallback?.Invoke(percent);
                            nextUpdatePercent += updateInterval;
                        }
                    }
                }
            }
            finally
            {
                if (ftCursorSrc != null)
                {
                    ComReleaser.ReleaseCOMObject(ftCursorSrc);
                    ftCursorSrc = null;
                }
            }

            return count;
        }


        /// <summary>
        /// Append to Dest Feature Class. Auto map same field name.
        /// </summary>
        /// <param name="ftCursorSrc"></param>
        /// <param name="fclassDest"></param>
        public int Append(IFeatureCursor ftCursorSrc, IFeatureClass fclassDest, int totalSrcRecord)
        {
            // Make key Upper
            MakeAllDictConvertFieldValueKeyUpper();

            IFeature ftSrc = null;
            IFeature ftDest = null;

            SkipFields.Add(fclassDest.OIDFieldName.ToUpper());
            SkipFields.Add(fclassDest.ShapeFieldName.ToUpper());

            int count = 0;
            int total = totalSrcRecord;
            int percent = 0;
            int updateInterval = PercentCallbackInterval;
            int nextUpdatePercent = updateInterval;

            try
            {
                // Loop Src Record
                while ((ftSrc = ftCursorSrc.NextFeature()) != null)
                {
                    ftDest = fclassDest.CreateFeature();

                    // Graphic
                    CopyShapeFieldValue(ftSrc, ftDest);

                    // Attribute
                    CopyFieldsValue(ftSrc, ftDest);

                    // Save 
                    ftDest.Store();

                    count++;

                    percent = CalPercent(count, total);
                    if (percent >= nextUpdatePercent)
                    {
                        ProgressCallback?.Invoke(percent);
                        nextUpdatePercent += updateInterval;
                    }
                }
            }
            finally
            {
                ComReleaser.ReleaseCOMObject(ftCursorSrc);
                ftCursorSrc = null;
            }

            return count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="readerSrc">Current read row from SqlDataReader</param>
        /// <param name="ftDest"></param>
        public void CopyShapeFieldValueFromSqlDataReader(SqlDataReader readerSrc, IFeature ftDest)
        {
            int inxSrc = readerSrc.GetOrdinal(ShapeFieldName);
            if (inxSrc == -1)
                throw new Exception($"Field Shape (\"{ShapeFieldName}\") not found in source SqlDataReader");

            object value = readerSrc.GetValue(inxSrc);

            int inxDest = ftDest.Fields.FindField(ShapeFieldName);
            if (inxDest == -1)
                throw new Exception($"Field Shape (\"{ShapeFieldName}\") not found in destination IFeature");

            ftDest.Value[inxDest] = value;
        }

        public void CopyShapeFieldValue(IFeature ftSrc, IFeature ftDest)
        {
            CopyShapeFieldValue(ftSrc as IRow, ftDest);
        }

        /// <summary>
        /// Copy ShapeField Value, support source IRow, IFeature
        /// </summary>
        /// <param name="rowSrc"></param>
        /// <param name="ftDest"></param>
        public void CopyShapeFieldValue(IRow rowSrc, IFeature ftDest)
        {
            if (!DictConvertRowFieldValueFunc.ContainsKey(ShapeFieldName.ToUpper()))
            {
                if (rowSrc is IFeature)
                {
                    IFeature ftSrc = rowSrc as IFeature;
                    ftDest.Shape = ftSrc.Shape;
                }
                else
                {
                    // src is table (don't have geometry)
                    // do nothing
                }
            }
            else
            {
                var funcConvertValue = DictConvertRowFieldValueFunc[ShapeFieldName.ToUpper()];
                int inxShapeField = rowSrc.Fields.FindField(ShapeFieldName);
                ftDest.Shape = funcConvertValue(rowSrc, inxShapeField) as IGeometry;
            }
        }

        /// <summary>
        /// Copy attribute fields only.
        /// </summary>
        /// <param name="ftSrc"></param>
        /// <param name="ftDest"></param>
        public void CopyFieldsValue(IFeature ftSrc, IFeature ftDest)
        {
            IFields fieldsSrc = ftSrc.Fields;
            IFields fieldsDest = ftDest.Fields;

            IField fieldSrc;
            IField fieldDest;

            int inxFieldSrc = -1;

            Func<IFeature, int, object> funcConvertValue;

            // Loop Dest fields
            for (int i = 0; i < fieldsDest.FieldCount; i++)
            {
                fieldDest = fieldsDest.Field[i];

                if (!fieldDest.Editable) continue;
                if (SkipFields.Contains(fieldDest.Name.ToUpper())) continue;

                inxFieldSrc = fieldsSrc.FindField(fieldDest.Name);
                if (inxFieldSrc != -1)
                {
                    if (!DictConvertFeatureFieldValueFunc.ContainsKey(fieldDest.Name.ToUpper()))
                    {
                        ftDest.Value[i] = ftSrc.Value[inxFieldSrc];
                    }
                    else
                    {
                        funcConvertValue = DictConvertFeatureFieldValueFunc[fieldDest.Name.ToUpper()];
                        ftDest.Value[i] = funcConvertValue(ftSrc, inxFieldSrc);
                    }
                }
            }
        }


        /// <summary>
        /// Copy attribute fields only.
        /// </summary>
        /// <param name="rowSrc"></param>
        /// <param name="rowDest"></param>
        public void CopyFieldsValue(IRow rowSrc, IRow rowDest)
        {
            IFields fieldsSrc = rowSrc.Fields;
            IFields fieldsDest = rowDest.Fields;

            IField fieldSrc;
            IField fieldDest;

            int inxFieldSrc = -1;

            Func<IRow, int, object> funcConvertValue;

            // Loop Dest fields
            for (int i = 0; i < fieldsDest.FieldCount; i++)
            {
                fieldDest = fieldsDest.Field[i];

                if (!fieldDest.Editable) continue;
                if (SkipFields.Contains(fieldDest.Name.ToUpper())) continue;

                inxFieldSrc = fieldsSrc.FindField(fieldDest.Name);
                if (inxFieldSrc != -1)
                {
                    if (!DictConvertRowFieldValueFunc.ContainsKey(fieldDest.Name.ToUpper()))
                    {
                        rowDest.Value[i] = rowSrc.Value[inxFieldSrc];
                    }
                    else
                    {
                        funcConvertValue = DictConvertRowFieldValueFunc[fieldDest.Name.ToUpper()];
                        rowDest.Value[i] = funcConvertValue(rowSrc, inxFieldSrc);
                    }
                }
            }
        }


        /// <summary>
        /// Copy attribute fields only.
        /// </summary>
        /// <param name="readerSrc"></param>
        /// <param name="ftDest"></param>
        public void CopyFieldsValue(SqlDataReader readerSrc, IFeature ftDest)
        {
            IFields fieldsDest = ftDest.Fields;
            IField fieldDest;
            int inxFieldSrc = -1;
            Func<SqlDataReader, int, object> funcConvertValue;


            // Loop Dest fields
            for (int i = 0; i < fieldsDest.FieldCount; i++)
            {
                fieldDest = fieldsDest.Field[i];

                if (!fieldDest.Editable) continue;
                if (SkipFields.Contains(fieldDest.Name.ToUpper())) continue;

                inxFieldSrc = readerSrc.GetOrdinal(fieldDest.Name);
                if (inxFieldSrc != -1)
                {
                    if (!DictConvertSqlFieldValueFunc.ContainsKey(fieldDest.Name.ToUpper()))
                    {
                        if (fieldDest.Type == esriFieldType.esriFieldTypeGUID)
                        {
                            ftDest.Value[i] = readerSrc.GetGuidStringIfAny(fieldDest.Name);
                        }
                        else
                        {
                            ftDest.Value[i] = readerSrc.GetValue(inxFieldSrc);
                        }
                    }
                    else
                    {
                        funcConvertValue = DictConvertSqlFieldValueFunc[fieldDest.Name.ToUpper()];
                        ftDest.Value[i] = funcConvertValue(readerSrc, inxFieldSrc);
                    }
                }
            }
        }


        /// <summary>
        /// Copy attribute fields only.
        /// </summary>
        /// <param name="readerSrc"></param>
        /// <param name="rowDest"></param>
        public void CopyFieldsValue(SqlDataReader readerSrc, IRow rowDest)
        {
            IFields fieldsDest = rowDest.Fields;
            IField fieldDest;
            int inxFieldSrc = -1;
            Func<SqlDataReader, int, object> funcConvertValue;


            // Loop Dest fields
            for (int i = 0; i < fieldsDest.FieldCount; i++)
            {
                fieldDest = fieldsDest.Field[i];

                if (!fieldDest.Editable) continue;
                if (SkipFields.Contains(fieldDest.Name.ToUpper())) continue;

                inxFieldSrc = readerSrc.GetOrdinal(fieldDest.Name);
                if (inxFieldSrc != -1)
                {
                    if (!DictConvertSqlFieldValueFunc.ContainsKey(fieldDest.Name.ToUpper()))
                    {
                        if (fieldDest.Type == esriFieldType.esriFieldTypeGUID)
                        {
                            rowDest.Value[i] = readerSrc.GetGuidStringIfAny(fieldDest.Name);
                        }
                        else
                        {
                            rowDest.Value[i] = readerSrc.GetValue(inxFieldSrc);
                        }
                    }
                    else
                    {
                        funcConvertValue = DictConvertSqlFieldValueFunc[fieldDest.Name.ToUpper()];
                        rowDest.Value[i] = funcConvertValue(readerSrc, inxFieldSrc);
                    }
                }
            }
        }
    }
}
