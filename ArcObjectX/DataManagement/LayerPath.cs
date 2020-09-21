using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcObjectX.DataManagement
{
    /// <summary>
    /// Provide function for reading and working with layer path.
    /// <para>Example</para>
    /// <para>- D:\Data\Datasource.gdb\ShopPoint</para>
    /// <para>- D:\Data\Datasource.gdb\datasetName\ShopPoint</para>
    /// <para>- D:\Data\Datasource.sde\ShopPoint</para>
    /// </summary>
    public class LayerPath
    {
        public string LayerPathStr { get; set; }

        public LayerPath(string layerPath)
        {
            LayerPathStr = layerPath;
        }

        public string GetDatasourcePath()
        {
            return GetDatasourcePath(LayerPathStr);
        }

        public string GetLayerName()
        {
            return GetLayerName(LayerPathStr);
        }

        public string GetDatasetName()
        {
            return GetDatasetName(LayerPathStr);
        }

        public static string GetDatasourcePath(string layerPath)
        {
            string[] splits = layerPath.Split('\\');
            return splits.FirstOrDefault();
        }

        public static string GetLayerName(string layerPath)
        {
            string[] splits = layerPath.Split('\\');
            return splits.LastOrDefault();
        }

        public static string GetDatasetName(string layerPath)
        {
            string[] splits = layerPath.Split('\\');
            if (splits.Length < 3)
            {
                return "";
            }
            else
            {
                return splits[1];
            }
        }
    }
}
