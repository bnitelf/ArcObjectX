using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcObjectX.DataProcessing
{
    public class LayerLooper
    {
        public static List<T> Loop<T>(IFeatureClass fclass, IQueryFilter filter, bool readOnly, Func<IFeature, T> funcLoopBody)
        {
            IFeature ft;
            List<T> listDatas = new List<T>();

            using (ComReleaser comReleaser = new ComReleaser())
            {
                IFeatureCursor cursor = fclass.Search(filter, readOnly);
                comReleaser.ManageLifetime(cursor);

                while ((ft = cursor.NextFeature()) != null)
                {
                    if (funcLoopBody != null)
                    {
                        T data = funcLoopBody(ft);
                        listDatas.Add(data);
                    }
                }
            }

            return listDatas;
        }


        public static List<T> Loop<T>(ITable table, IQueryFilter filter, bool readOnly, Func<IRow, T> funcLoopBody)
        {
            IRow row;
            List<T> listDatas = new List<T>();

            using (ComReleaser comReleaser = new ComReleaser())
            {
                ICursor cursor = table.Search(filter, readOnly);
                comReleaser.ManageLifetime(cursor);

                while ((row = cursor.NextRow()) != null)
                {
                    if (funcLoopBody != null)
                    {
                        T data = funcLoopBody(row);
                        listDatas.Add(data);
                    }
                }
            }

            return listDatas;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fclass"></param>
        /// <param name="filter"></param>
        /// <param name="readOnly"></param>
        /// <param name="funcLoopBody"></param>
        /// <returns>Looped record count.</returns>
        public static int Loop(IFeatureClass fclass, IQueryFilter filter, bool readOnly, Action<IFeature> funcLoopBody)
        {
            IFeature ft;
            int count = 0;

            using (ComReleaser comReleaser = new ComReleaser())
            {
                IFeatureCursor cursor = fclass.Search(filter, readOnly);
                comReleaser.ManageLifetime(cursor);

                while ((ft = cursor.NextFeature()) != null)
                {
                    count++;
                    funcLoopBody?.Invoke(ft);
                }
            }
            return count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="filter"></param>
        /// <param name="readOnly"></param>
        /// <param name="funcLoopBody"></param>
        /// <returns>Looped record count.</returns>
        public static int Loop(ITable table, IQueryFilter filter, bool readOnly, Action<IRow> funcLoopBody)
        {
            IRow row;
            int count = 0;

            using (ComReleaser comReleaser = new ComReleaser())
            {
                ICursor cursor = table.Search(filter, readOnly);
                comReleaser.ManageLifetime(cursor);

                while ((row = cursor.NextRow()) != null)
                {
                    count++;
                    funcLoopBody?.Invoke(row);
                }
            }
            return count;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fclass"></param>
        /// <param name="filter"></param>
        /// <param name="readOnly"></param>
        /// <param name="funcLoopBody">(feature, current record count)</param>
        /// <returns>Looped record count.</returns>
        public static int Loop(IFeatureClass fclass, IQueryFilter filter, bool readOnly, Action<IFeature, int> funcLoopBody)
        {
            IFeature ft;
            int count = 0;

            using (ComReleaser comReleaser = new ComReleaser())
            {
                IFeatureCursor cursor = fclass.Search(filter, readOnly);
                comReleaser.ManageLifetime(cursor);

                while ((ft = cursor.NextFeature()) != null)
                {
                    count++;
                    funcLoopBody?.Invoke(ft, count);
                }
            }
            return count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="filter"></param>
        /// <param name="readOnly"></param>
        /// <param name="funcLoopBody">(row, current record count)</param>
        /// <returns>Looped record count.</returns>
        public static int Loop(ITable table, IQueryFilter filter, bool readOnly, Action<IRow, int> funcLoopBody)
        {
            IRow row;
            int count = 0;

            using (ComReleaser comReleaser = new ComReleaser())
            {
                ICursor cursor = table.Search(filter, readOnly);
                comReleaser.ManageLifetime(cursor);

                while ((row = cursor.NextRow()) != null)
                {
                    count++;
                    funcLoopBody?.Invoke(row, count);
                }
            }
            return count;
        }


        /// <summary>
        /// Loop feature class, for update purpose. Using IFeatureClass.Update() method.
        /// By using this method for update, it is faster than updating individual edit IFeature from IFeatureClass.Search() method.
        /// Because the update is performed on the current 'cursor position'.
        /// </summary>
        /// <param name="fclass"></param>
        /// <param name="filter"></param>
        /// <param name="recycling"></param>
        /// <param name="funcLoopBody">argument (feature)</param>
        /// <returns>Looped record count.</returns>
        public static int LoopUpdate(IFeatureClass fclass, IQueryFilter filter, bool recycling, Action<IFeature> funcLoopBody)
        {
            IFeature ft;
            int count = 0;

            using (ComReleaser comReleaser = new ComReleaser())
            {
                IFeatureCursor cursor = fclass.Update(filter, recycling);
                comReleaser.ManageLifetime(cursor);

                while ((ft = cursor.NextFeature()) != null)
                {
                    count++;
                    funcLoopBody?.Invoke(ft);
                }
            }
            return count;
        }


        /// <summary>
        /// Loop feature class, for update purpose. Using IFeatureClass.Update() method.
        /// By using this method for update, it is faster than updating individual edit IFeature from IFeatureClass.Search() method.
        /// Because the update is performed on the current 'cursor position'.
        /// </summary>
        /// <param name="fclass"></param>
        /// <param name="filter"></param>
        /// <param name="recycling"></param>
        /// <param name="funcLoopBody">argument (feature, count current record)</param>
        /// <returns>Looped record count.</returns>
        public static int LoopUpdate(IFeatureClass fclass, IQueryFilter filter, bool recycling, Action<IFeature, int> funcLoopBody)
        {
            IFeature ft;
            int count = 0;

            using (ComReleaser comReleaser = new ComReleaser())
            {
                IFeatureCursor cursor = fclass.Update(filter, recycling);
                comReleaser.ManageLifetime(cursor);

                while ((ft = cursor.NextFeature()) != null)
                {
                    count++;
                    funcLoopBody?.Invoke(ft, count);
                    cursor.UpdateFeature(ft);
                }
            }
            return count;
        }
    }
}
