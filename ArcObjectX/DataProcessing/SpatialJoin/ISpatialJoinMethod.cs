using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcObjectX.DataProcessing.SpatialJoin
{
    public interface ISpatialJoinMethod
    {
        /// <summary>
        /// Get join features. 
        /// In other word, get intersected (depend on what method used) features in Join Feature Class.
        /// </summary>
        /// <param name="inputFt"></param>
        /// <param name="joinFeatureClass"></param>
        /// <returns></returns>
        List<IFeature> GetJoinFeatures(IFeature inputFt, IFeatureClass joinFeatureClass);
    }
}
