using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ESRI.ArcGIS.Geodatabase;

using ArcObjectX.DataManagement.Tool.Export;
namespace ArcObjectX_Test.DataManagement
{
    /// <summary>
    /// Summary description for Test_Tools
    /// </summary>
    [TestClass]
    public class Test_Tools
    {
        [TestMethod]
        public void ExportAllInfo()
        {
            List<TestInfo> lstInfo = new List<TestInfo>();
            IFeatureClass fClass = null;

            ArcObjectExportTool objExport = new ArcObjectExportTool();
            objExport.SetExportInfo<TestInfo>()
                .SetExportAllInfo(true)
                .SetExportList(lstInfo)
                .SetExportFeatureClass(fClass)
                .Insert();
        }

        [TestMethod]
        public void ExportSpecifically()
        {
            List<TestInfo> lstInfo = new List<TestInfo>();
            IFeatureClass fClass = null;

            ArcObjectExportTool objExport = new ArcObjectExportTool();
            objExport.SetExportInfo<TestInfo>()
                .SetExportAllInfo(false)
                .SetExportList(lstInfo)
                .SetExportFeatureClass(fClass)
                .MappingField(x => x.Name, "BL_Name")
                .Insert();
        }
    }

    public class TestInfo
    {
        private string _Name = string.Empty;
        private string _Value = string.Empty;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public string Value
        {
            get { return _Value; }
            set { _Value = value; }
        }
    }
}
