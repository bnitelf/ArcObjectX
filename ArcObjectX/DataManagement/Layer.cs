using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcObjectX.DataManagement
{
    /// <summary>
    /// Layer object. Provide layer functionality.
    /// </summary>
    public class Layer
    {
        private ITable _table;
        private IFeatureClass _featureClass;

        public IFeatureClass FeatureClass { get; set; }
        public ITable Table { get; set; }

        public Layer()
        {

        }

        public Layer(IFeatureClass fclass)
        {
            _featureClass = fclass;
            _table = fclass as ITable;
        }

        public Layer(ITable table)
        {
            _featureClass = table as IFeatureClass;
            _table = table;
        }

        /// <summary>
        /// Check if this layer object is Table or not.
        /// </summary>
        /// <returns></returns>
        public bool IsTable()
        {
            if (_featureClass == null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Check if this layer object is FeatureClass or not.
        /// </summary>
        /// <returns></returns>
        public bool IsLayer()
        {
            if (_featureClass != null)
                return true;
            else
                return false;
        }

        public string GetLayerName()
        {
            IDataset ds = _table as IDataset;
            string layerName = ds.Name;
            return layerName;
        }

        public string GetLayerNameWithOutTableOwner()
        {
            IDataset ds = _table as IDataset;
            string layerName = ds.Name;
            string layerNameWithoutOwner = layerName.Split('.').Last();
            return layerNameWithoutOwner;
        }

        public string GetDatasourcePath()
        {
            IDataset ds = _table as IDataset;
            string dsPath = ds.BrowseName;
            return dsPath;
        }
    }
}
