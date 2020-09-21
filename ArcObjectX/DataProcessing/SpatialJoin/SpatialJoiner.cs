using ArcObjectX.DataManagement;
using ArcObjectX.Util;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcObjectX.DataProcessing.SpatialJoin
{
    public class SpatialJoiner
    {
        private const string FieldInput_ObjectID = "Input_ObjectID";
        private const string FieldRef_DsPath = "Ref_DsPath";
        private const string FieldRef_ObjectID = "Ref_ObjectID";

        /// <summary>
        /// Main layer use to find spatial relation feature in JoinLayer.
        /// Layer you want to find which feature in JoinLayer has spatial relation 
        /// (like intersect) with feature in Input Layer.
        /// </summary>
        public IFeatureClass InputLayer { get; set; }

        /// <summary>
        /// Layer to
        /// </summary>
        public IFeatureClass JoinLayer { get; set; }

        /// <summary>
        /// Get or Set ISpatialJoinMethod 
        /// (Method use to join Input layer to Join Layer)
        /// <para>Default is <see cref="SpatialJoinIntersectMethod">Intersect method</see></para>
        /// </summary>
        public ISpatialJoinMethod SpatialJoinMethod { get; set; }

        /// <summary>
        /// Set function to filter Input Layer to be join. If not set, no filter is applied.
        /// Send (InputFeature), Should return true if want to spatial join on this feature, otherwise false.
        /// </summary>
        public Func<IFeature, bool> OnFilterInputFeature { get; set; }

        /// <summary>
        /// Set function to filter out spatial join feature result. If not set, no filter is applied.
        /// Send (InputFeature, Joined Feature), Should return false if want to filter out.
        /// </summary>
        public Func<IFeature, IFeature, bool> OnFilterSpatialJoinResult { get; set; }

        /// <summary>
        /// Call back on Spatial Join (per input layer's record). Call every time when perform spatial join each record done.
        /// <para>Send (InputFeature, List of result Spatial Joined feature)</para>
        /// </summary>
        public Action<IFeature, List<IFeature>> OnResultSpatialJoinPerRecord { get; set; }

        /// <summary>
        /// Call back on Compose Spatial Relation Info (per input layer's record). Call every time when perform spatial join each record done 
        /// (Input Layer -> Join Layer).
        /// <para>Send (InputFeature, List of result Spatial Joined feature)</para>
        /// <para>Should return Addition fields you want to store in Spatial Relation Infos, 
        /// (the returned record count must equal to List&lt;IFeature&gt; count)
        /// return null or not set if no additional fields to add</para>
        /// </summary>
        public Func<IFeature, List<IFeature>, List<RecordData>> OnComposeSpatialJoinRelationResultPerRecord { get; set; }
        
        /// <summary>
        /// Progress call back; Send (current percent)
        /// </summary>
        public Action<int> OnProgressCallback { get; set; }

        public SpatialJoiner()
        {
            SpatialJoinMethod = new SpatialJoinIntersectMethod();
        }

        public List<RecordData> Join()
        {
            List<RecordData> listJoinInfos = new List<RecordData>();

            RecordData joinInfo;

            IFeature ft;
            IFeature ftJoined;
            List<IFeature> listSpatialJoinResults;
            List<IFeature> listSpatialJoinResultsFiltered;

            int count = 0;
            int total = InputLayer.FeatureCount(null);
            int percent = 0;
            int updateInterval = 10;
            int nextUpdatePercent = updateInterval;

            using (ComReleaser comReleaser = new ComReleaser())
            {
                IFeatureCursor cursor = InputLayer.Search(null, true);
                comReleaser.ManageLifetime(cursor);
                bool shouldExecute = true;

                while ((ft = cursor.NextFeature()) != null)
                {
                    if (OnFilterInputFeature != null)
                    {
                        shouldExecute = OnFilterInputFeature(ft);
                    }

                    if (shouldExecute)
                    {
                        listSpatialJoinResults = SpatialJoinMethod.GetJoinFeatures(ft, JoinLayer);

                        // Filter SpatialJoinResult out 
                        if (OnFilterSpatialJoinResult != null)
                        {
                            listSpatialJoinResultsFiltered = new List<IFeature>();
                            for (int i = 0; i < listSpatialJoinResults.Count; i++)
                            {
                                ftJoined = listSpatialJoinResults[i];
                                if (OnFilterSpatialJoinResult(ft, ftJoined) == true)
                                {
                                    listSpatialJoinResultsFiltered.Add(ftJoined);
                                }
                            }
                            listSpatialJoinResults = listSpatialJoinResultsFiltered;
                        }

                        // Call back custom function after get join result.
                        OnResultSpatialJoinPerRecord?.Invoke(ft, listSpatialJoinResults);

                        if (listSpatialJoinResults.Count > 0)
                        {
                            // Compose additionFields
                            List<RecordData> additionFieldsRecords = OnComposeSpatialJoinRelationResultPerRecord?.Invoke(ft, listSpatialJoinResults);
                            if (additionFieldsRecords != null &&
                                additionFieldsRecords.Count > 0 &&
                                additionFieldsRecords.Count != listSpatialJoinResults.Count)
                            {
                                throw new Exception("Result of OnComposeSpatialJoinRelationResultPerRecord must return List<RecordData> count = param List<IFeature> count.");
                            }

                            RecordData additionFields;

                            // Compose spatial join result infos
                            for (int i = 0; i < listSpatialJoinResults.Count; i++)
                            {
                                ftJoined = listSpatialJoinResults[i];

                                joinInfo = NewSpatialJoinInfo();
                                joinInfo[FieldInput_ObjectID] = ft.OID;
                                joinInfo[FieldRef_DsPath] = LayerUtil.GetDataSourcePath(JoinLayer);
                                joinInfo[FieldRef_ObjectID] = ftJoined.OID;

                                #region consider use later
                                //// Compose additionFields
                                //List<RecordData> additionFieldsRecords = OnComposeSpatialJoinRelationResultPerRecord?.Invoke(ft, listSpatialJoinResults);
                                //if (additionFieldsRecords != null)
                                //{
                                //    foreach (var additionFields in additionFieldsRecords)
                                //    {
                                //        string[] additionalFieldNames = additionFields.GetFieldNames();
                                //        string additionalFieldName;

                                //        for (int j = 0; j < additionalFieldNames.Length; j++)
                                //        {
                                //            additionalFieldName = additionalFieldNames[j];
                                //            joinInfo[additionalFieldName] = additionFields[additionalFieldName];
                                //        }
                                //    }
                                //}
                                #endregion

                                // Add additionFields to joinInfo
                                if (additionFieldsRecords != null && additionFieldsRecords.Count > 0)
                                {
                                    additionFields = additionFieldsRecords[i];
                                    string[] additionalFieldNames = additionFields.GetFieldNames();
                                    string additionalFieldName;

                                    for (int j = 0; j < additionalFieldNames.Length; j++)
                                    {
                                        additionalFieldName = additionalFieldNames[j];
                                        joinInfo[additionalFieldName] = additionFields[additionalFieldName];
                                    }
                                }

                                listJoinInfos.Add(joinInfo);
                            }
                        }
                    }

                    // Progress
                    count++;
                    percent = CalPercent(count, total);

                    if (percent >= nextUpdatePercent)
                    {
                        OnProgressCallback?.Invoke(percent);
                        nextUpdatePercent += updateInterval;
                    }
                }
            }

            return listJoinInfos;
        }

        private RecordData NewSpatialJoinInfo()
        {
            RecordData record = new RecordData();
            record[FieldInput_ObjectID] = -1;
            record[FieldRef_DsPath] = "";
            record[FieldRef_ObjectID] = -1;
            return record;
        }

        private int CalPercent(int current, int total)
        {
            float percent = 0;
            percent = (float)current / (float)total * 100;
            return (int)percent;
        }
    }
}
