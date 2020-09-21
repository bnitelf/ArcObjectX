using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcObjectX.DataProcessing.SpatialJoin
{
    public class SpatialJoinByISpatialFilterMethod : ISpatialJoinMethod
    {
        public ISpatialFilter SpatialFilter { get; set; }

        public SpatialJoinByISpatialFilterMethod(ISpatialFilter spatialFilter)
        {
            SpatialFilter = spatialFilter;
        }

        public List<IFeature> GetJoinFeatures(IFeature inputFt, IFeatureClass joinFeatureClass)
        {
            List<IFeature> listResults = new List<IFeature>();

            IGeometry geometry = inputFt.Shape;

            SpatialFilter.Geometry = geometry;
            SpatialFilter.GeometryField = joinFeatureClass.ShapeFieldName;

            IFeatureCursor cursor = joinFeatureClass.Search(SpatialFilter, false);
            IFeature foundFeature = null;
            while ((foundFeature = cursor.NextFeature()) != null)
            {
                listResults.Add(foundFeature);
            }
            return listResults;
        }
    }
}
