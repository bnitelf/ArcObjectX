using System;
using System.Collections.Generic;

using ESRI.ArcGIS.Geodatabase;

using ArcObjectX.DataManagement.Tool;
namespace ArcObjectX_Sample.ExampleExportTool
{
    class ExportToolExample
    {
        public void CaseExportAllInfo()
        {
            IFeatureClass fClass = null;

            ArcObjectExportTool<object> arcObjectExportTool = new ArcObjectExportTool<object>();
            arcObjectExportTool.SetCallBack = CallBack;
            arcObjectExportTool.ExportInfo(new List<object>())
                .ExportTo(fClass)
                .SaveEveryNRecord(100)
                .Insert();
        }

        public void CaseExportSpecifiedInfo()
        {
            IFeatureClass fClass = null;

            ArcObjectExportTool<info> arcObjectExportTool = new ArcObjectExportTool<info>();
            arcObjectExportTool.SetCallBack = CallBack;
            arcObjectExportTool.ExportInfo(new List<info>())
                .ExportTo(fClass)
                .SaveEveryNRecord(100)
                .MappingField(x => x.Ref_OID, "Ref_OID")
                .Insert();
        }

        public void CaseExportFeatureClass()
        {
            IFeatureClass srcFeatureClass = null;
            IFeatureClass desFeatureClass = null;

            ArcObjectExportTool<IFeatureClass> arcObjectExportTool = new ArcObjectExportTool<IFeatureClass>();
            arcObjectExportTool.SetCallBack = CallBack;
            arcObjectExportTool.ExportInfo(srcFeatureClass)
                .ExportTo(desFeatureClass)
                .SaveEveryNRecord(100)
                .Insert();
        }

        public void CallBack(int value)
        {
            Console.WriteLine(value);
        }
    }

    public class info
    {
        public string Ref_OID { get; set; }
    }
}
