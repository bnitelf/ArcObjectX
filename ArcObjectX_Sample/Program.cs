using ArcObjectX_Sample.ExampleExportTool;
namespace ArcObjectX_Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            ExportToolExample exportTool = new ExportToolExample();
            exportTool.CaseExportFeatureClass();
        }
    }
}
