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


            List<IQueryFilter> ListQcQueryFilter = new List<IQueryFilter>();
            int countAll = LayerLooper.Loop(fclass, ListQcQueryFilter, readOnly, (feature) =>
            {
                // do somethings
                return true; // = continue; in loop.

                Console.WriteLine("This code will be skip like you use continue; statement.");
            });

            int countOnlyLoopSuccess = LayerLooper.Loop(fclass, ListQcQueryFilter, readOnly, (feature) =>
            {
                // do somethings
                return false; // = break; in loop.

                Console.WriteLine("This code will be skip like you use break; statement.");
            });

        }
    }
}
