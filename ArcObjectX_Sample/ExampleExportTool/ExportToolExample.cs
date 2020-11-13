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
            arcObjectExportTool.ExportInfo(new List<object>())
                .ExportAll(true)
                .ExportTo(fClass)
                .OutputType(OutputType.GDB)
                .SaveEveryNRecord(100)
                .Insert();
        }

        public void CaseExportSpecifiedInfo()
        {
            IFeatureClass fClass = null;

            ArcObjectExportTool<info> arcObjectExportTool = new ArcObjectExportTool<info>();
            arcObjectExportTool.ExportInfo(new List<info>())
                .ExportAll(false)
                .ExportTo(fClass)
                .OutputType(OutputType.GDB)
                .SaveEveryNRecord(100)
                .MappingField(x => x.Ref_OID, "Ref_OID")
                .Insert();
        }
    }

    public class info
    {
        public string Ref_OID { get; set; }
    }
}
