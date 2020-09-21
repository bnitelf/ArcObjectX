using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcObjectX.Util
{
    public class FeatureUtil
    {
        
        public static IWorkspace GetWorkspace(IFeature feature)
        {
            IDataset dataset = (IDataset)feature.Class;
            IWorkspace ws = dataset.Workspace;
            return ws;
        }

        public static IWorkspace GetWorkspace(IRow row)
        {
            IDataset dataset = (IDataset)row.Table;
            IWorkspace ws = dataset.Workspace;
            return ws;
        }

        public static string GetDatasourcePath(IRow row)
        {
            IWorkspace ws = GetWorkspace(row);
            string dsPath = DataSourceUtil.GetDataSourcePath(ws);
            return dsPath;
        }

        public static string GetDatasourcePath(IFeature feature)
        {
            IWorkspace ws = GetWorkspace(feature);
            string dsPath = DataSourceUtil.GetDataSourcePath(ws);
            return dsPath;
        }

        public static string GetLayerName(IFeature feature)
        {
            string layerName = ((IDataset)feature.Class).Name;
            return layerName;
        }

        public static string GetLayerName(IRow row)
        {
            string layerName = ((IDataset)row.Table).Name;
            return layerName;
        }
    }
}
