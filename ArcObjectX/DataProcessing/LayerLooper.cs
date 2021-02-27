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
        /// <summary>
        /// Loop then load feature value to List of <typeparamref name="T"/>.
        /// The <paramref name="funcLoopBody"/> is used to transfer IFeature to <typeparamref name="T"/> object.
        /// <
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fclass"></param>
        /// <param name="filter"></param>
        /// <param name="readOnly"></param>
        /// <param name="funcLoopBody"></param>
        /// <returns>List of loaded <typeparamref name="T"/></returns>
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
        /// Loop feature class by using IQueryFilter (can be null = loop all record)
        /// <para/><paramref name="funcLoopBody"/> is a call back function that will be call during every loop. Do want ever you want in there. 
        /// Use return; will result same behavior as continue; statement in loop. This Loop function not support break; if you want to exit loop (or use break; liked statement) 
        /// Use <see cref="Loop(IFeatureClass, IQueryFilter, bool, Func{IFeature, bool})"/> instead.
        /// </summary>
        /// <param name="fclass">Feature class (or layer) you want to loop</param>
        /// <param name="filter">can be null = loop all record</param>
        /// <param name="readOnly">True = will use less memory (for read only purpose), False = use more memory (often used for update purpose)</param>
        /// <param name="funcLoopBody">Call back function that will be call during every loop.</param>
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
        /// Loop feature class by using IQueryFilter (can be null = loop all record)
        /// <para/><paramref name="funcLoopBody"/> is a call back function that will be call during every loop. Do want ever you want in there. 
        /// return True to continue loop, False to break loop.
        /// </summary>
        /// <param name="fclass">Feature class (or layer) you want to loop</param>
        /// <param name="filter">can be null = loop all record</param>
        /// <param name="readOnly">True = will use less memory (for read only purpose), False = use more memory (often used for update purpose)</param>
        /// <param name="funcLoopBody">Call back function that will be call during every loop.</param>
        /// <returns>Looped record count.</returns>
        public static int Loop(IFeatureClass fclass, IQueryFilter filter, bool readOnly, Func<IFeature, bool> funcLoopBody)
        {
            IFeature ft;
            int count = 0;
            bool continueLoop = true;

            using (ComReleaser comReleaser = new ComReleaser())
            {
                IFeatureCursor cursor = fclass.Search(filter, readOnly);
                comReleaser.ManageLifetime(cursor);

                while ((ft = cursor.NextFeature()) != null)
                {
                    count++;

                    if (funcLoopBody != null)
                    {
                        continueLoop = funcLoopBody(ft);
                        if (continueLoop == false)
                            break;
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// Loop feature class by using List of IQueryFilter (can be null or empty list = loop all record)
        /// <para/><paramref name="funcLoopBody"/> is a call back function that will be call during every loop. Do want ever you want in there.
        /// return True to continue loop, False to break loop.
        /// </summary>
        /// <param name="fclass">Feature class (or layer) you want to loop</param>
        /// <param name="ListFilters">can be null or empty list = loop all record</param>
        /// <param name="readOnly">True = will use less memory (for read only purpose), False = use more memory (often used for update purpose)</param>
        /// <param name="funcLoopBody">Call back function that will be call during every loop.</param>
        /// <returns>Looped record count.</returns>
        public static int Loop(IFeatureClass fclass, List<IQueryFilter> ListFilters, bool readOnly, Func<IFeature, bool> funcLoopBody)
        {
            IQueryFilter filter = null;
            int count = 0;
            int tempTotal = 0;
            int tempCount = 0;

            if (ListFilters == null || ListFilters.Count == 0)
            {
                count = Loop(fclass, filter, readOnly, funcLoopBody);
            }
            else
            {
                for (int i = 0; i < ListFilters.Count; i++)
                {
                    filter = ListFilters[i];
                    tempTotal = fclass.FeatureCount(filter);
                    tempCount = Loop(fclass, filter, readOnly, funcLoopBody);
                    count += tempCount;

                    // = funcLoopBody want to break; the loop.
                    if (tempTotal != tempCount)
                        break;  
                }
            }
            
            return count;
        }

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
        /// <param name="table"></param>
        /// <param name="filter"></param>
        /// <param name="readOnly"></param>
        /// <param name="funcLoopBody"></param>
        /// <returns>Looped record count.</returns>
        public static int Loop(ITable table, IQueryFilter filter, bool readOnly, Func<IRow, bool> funcLoopBody)
        {
            IRow row;
            int count = 0;
            bool continueLoop = true;

            using (ComReleaser comReleaser = new ComReleaser())
            {
                ICursor cursor = table.Search(filter, readOnly);
                comReleaser.ManageLifetime(cursor);

                while ((row = cursor.NextRow()) != null)
                {
                    count++;

                    if (funcLoopBody != null)
                    {
                        continueLoop = funcLoopBody(row);
                        if (continueLoop == false)
                            break;
                    }
                }
            }
            return count;
        }

        public static int Loop(ITable table, List<IQueryFilter> ListFilters, bool readOnly, Func<IRow, bool> funcLoopBody)
        {
            IQueryFilter filter = null;
            int count = 0;
            int tempTotal = 0;
            int tempCount = 0;

            if (ListFilters == null || ListFilters.Count == 0)
            {
                count = Loop(table, filter, readOnly, funcLoopBody);
            }
            else
            {
                for (int i = 0; i < ListFilters.Count; i++)
                {
                    filter = ListFilters[i];
                    tempTotal = table.RowCount(filter);
                    tempCount = Loop(table, filter, readOnly, funcLoopBody);
                    count += tempCount;

                    // = funcLoopBody want to break; the loop.
                    if (tempTotal != tempCount)
                        break;
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
        /// <param name="funcLoopBody">(feature, current record count 1-based)</param>
        /// <returns>Looped record count.</returns>
        public static int Loop(IFeatureClass fclass, IQueryFilter filter, bool readOnly, Func<IFeature, int, bool> funcLoopBody)
        {
            IFeature ft;
            int count = 0;
            bool continueLoop = true;

            using (ComReleaser comReleaser = new ComReleaser())
            {
                IFeatureCursor cursor = fclass.Search(filter, readOnly);
                comReleaser.ManageLifetime(cursor);

                while ((ft = cursor.NextFeature()) != null)
                {
                    count++;

                    if (funcLoopBody != null)
                    {
                        continueLoop = funcLoopBody(ft, count);
                        if (continueLoop == false)
                            break;
                    }
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
        /// <param name="funcLoopBody">(row, current record count 1-based)</param>
        /// <returns>Looped record count.</returns>
        public static int Loop(ITable table, IQueryFilter filter, bool readOnly, Func<IRow, int, bool> funcLoopBody)
        {
            IRow row;
            int count = 0;
            bool continueLoop = true;

            using (ComReleaser comReleaser = new ComReleaser())
            {
                ICursor cursor = table.Search(filter, readOnly);
                comReleaser.ManageLifetime(cursor);

                while ((row = cursor.NextRow()) != null)
                {
                    count++;

                    if (funcLoopBody != null)
                    {
                        continueLoop = funcLoopBody(row, count);
                        if (continueLoop == false)
                            break;
                    }
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
