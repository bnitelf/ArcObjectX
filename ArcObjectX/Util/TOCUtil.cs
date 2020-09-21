using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcObjectX.Util
{
    /// <summary>
    /// Table of Content Utility class. For use with ArcGIS Map Control Application.
    /// Before use any function set <see cref="Map"/> first.
    /// </summary>
    public class TOCUtil
    {
        public static IMap Map { get; set; }

        public static ILayer GetLayer(string layerName)
        {
            if (Map == null)
            {
                throw new InvalidOperationException("Map is not set. (null)");
            }

            ILayer layer = null;
            IFeatureLayer featureLayer = null;
            IFeatureClass featureClass = null;
            IDataset dataset = null;
            IEnumLayer enumLayer = Map.get_Layers();
            enumLayer.Reset();
            layer = enumLayer.Next();
            while (layer != null)
            {
                if (!(layer is IRasterLayer) && (layer is IFeatureLayer) && (layer is IDataLayer) && (layer.Valid == true))
                {
                    featureLayer = layer as IFeatureLayer;
                    if (featureLayer == null)
                    {
                        layer = enumLayer.Next();
                        continue;
                    }
                    featureClass = featureLayer.FeatureClass;
                    if (featureClass == null)
                    {
                        layer = enumLayer.Next();
                        continue;
                    }
                    dataset = featureClass as IDataset;
                    if (dataset == null)
                    {
                        layer = enumLayer.Next();
                        continue;
                    }
                    if (dataset.Name.Trim().ToUpper().CompareTo(layerName.Trim().ToUpper()) == 0 ||
                        featureClass.AliasName.Trim().ToUpper().CompareTo(layerName.Trim().ToUpper()) == 0 ||
                        layer.Name.Trim().ToUpper().CompareTo(layerName.Trim().ToUpper()) == 0)
                    {
                        break;
                    }
                }

                layer = enumLayer.Next();
            }

            return layer;
        }


        public static IFeatureLayer GetFeatureLayer(string layerName)
        {
            ILayer layer = GetLayer(layerName);
            IFeatureLayer featureLayer = layer as IFeatureLayer;
            return featureLayer;
        }

        public static IFeatureClass GetFeatureClass(string layerName)
        {
            ILayer layer = GetLayer(layerName);
            IFeatureLayer featureLayer = layer as IFeatureLayer;
            IFeatureClass featureClass = featureLayer?.FeatureClass;
            return featureClass;
        }

        public static IWorkspace GetWorkspace(string layerName)
        {
            IFeatureClass featureClass = GetFeatureClass(layerName);
            IDataset dataset = featureClass as IDataset;
            IWorkspace ws = dataset?.Workspace;
            return ws;
        }

        public static bool CheckLayerExistInTOC(string layerName)
        {
            if (string.IsNullOrWhiteSpace(layerName))
                return false;

            ILayer pLayer = GetLayer(layerName);
            if (pLayer == null)
                return false;

            return true;
        }

        /// <summary>
        /// Check if layer is visible (tick).
        /// </summary>
        /// <param name="layerName"></param>
        /// <returns></returns>
        public static bool IsLayerVisible(string layerName)
        {
            ILayer layer = GetLayer(layerName);

            if (layer == null)
                return false;

            return layer.Visible;
        }

        public static void AddLayer(ILayer layer)
        {
            Map.AddLayer(layer);
        }

        public static void AddLayer(IFeatureLayer layer)
        {
            Map.AddLayer(layer);
        }

        public static void AddLayer(IFeatureClass fclass)
        {
            // Must include reference to ESRI.ArcGIS.Display too. otherwise will get error warning.
            //FeatureLayerClass iFeatureLayer = new FeatureLayerClass();
            IFeatureLayer featureLayer = new FeatureLayerClass();
            featureLayer.FeatureClass = fclass;
            featureLayer.Name = (fclass as IDataset).Name;
            Map.AddLayer(featureLayer);
        }

        public static void ChangeDataSource(ILayer layer, IWorkspace targetWorkspace)
        {
            var dataLayer = (IDataLayer)layer;
            var datasetName = (IDatasetName)dataLayer.DataSourceName;
            var newWorkspaceName = (IWorkspaceName)((IDataset)targetWorkspace).FullName;

            datasetName.WorkspaceName = newWorkspaceName;
            dataLayer.DataSourceName = (IName)datasetName;
            layer = (ILayer)dataLayer;
        }
    }
}
