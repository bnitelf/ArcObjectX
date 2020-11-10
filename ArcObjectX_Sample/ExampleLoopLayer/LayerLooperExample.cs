using ArcObjectX.DataProcessing;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcObjectX_Sample.ExampleLoopLayer
{
    class LayerLooperExample
    {
        public void Run()
        {
            IFeatureClass fclass = null;
            IQueryFilter filter = null;
            bool readOnly = true;
            var listResults = LayerLooper.Loop(fclass, filter, readOnly, (feature) =>
            {
                return new List<string>();
                //return 1;
            });
        }
    }
}
