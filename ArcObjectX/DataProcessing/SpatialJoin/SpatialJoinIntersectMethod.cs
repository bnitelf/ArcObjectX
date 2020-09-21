using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcObjectX.DataProcessing.SpatialJoin
{
    public class SpatialJoinIntersectMethod : ISpatialJoinMethod
    {
        /// <summary>
        /// Sub fields to load from Join features. Default is "*" (all fields)
        /// </summary>
        public string SubFields { get; set; }

        public SpatialJoinIntersectMethod()
        {
            SubFields = "*";
        }

        public List<IFeature> GetJoinFeatures(IFeature inputFt, IFeatureClass joinFeatureClass)
        {
            List<IFeature> listResults = new List<IFeature>();

            IGeometry geometry = inputFt.Shape;

            ISpatialFilter spatialFilter = new SpatialFilterClass();
            spatialFilter.Geometry = geometry;
            spatialFilter.GeometryField = joinFeatureClass.ShapeFieldName;
            spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
            spatialFilter.SubFields = SubFields;

            IFeatureCursor cursor = joinFeatureClass.Search(spatialFilter, false);
            IFeature foundFeature = null;
            while ((foundFeature = cursor.NextFeature()) != null)
            {
                listResults.Add(foundFeature);
            }
            return listResults;
        }
    }
}
