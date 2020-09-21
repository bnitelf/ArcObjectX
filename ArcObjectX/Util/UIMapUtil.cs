using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArcObjectX.Util
{
    /// <summary>
    /// Map Control Utility class. 
    /// Before use any function set <see cref="Map"/>, <see cref="AxMapControl"/>, and <see cref="AxTocControl"/> first.
    /// </summary>
    public class UIMapUtil
    {
        public static AxMapControl AxMapControl { get; set; }
        public static AxTOCControl AxTocControl { get; set; }

        public static string OpeningMxdPath
        {
            get
            {
                if (AxMapControl == null)
                {
                    return "";
                }
                else
                {
                    return AxMapControl.DocumentFilename;
                }
            }
        }

        public static void SaveMxd()
        {
            IMapDocument oMapDoc = new MapDocumentClass();
            oMapDoc.Open(AxMapControl.DocumentFilename);

            if (oMapDoc.IsReadOnly[AxMapControl.DocumentFilename])
            {
                throw new InvalidOperationException("Can not save mxd. Is read only.");
            }

            AxMapControl.Update();
            AxTocControl.Update();

            oMapDoc.ReplaceContents((IMxdContents)AxMapControl.Map);
            oMapDoc.Save(true, true);
            oMapDoc.Close();

            if (oMapDoc != null)
            {
                ComReleaser.ReleaseCOMObject(oMapDoc);
                oMapDoc = null;
            }

            if (AxMapControl.Map != null)
            {
                ComReleaser.ReleaseCOMObject(AxMapControl.Map);
                AxMapControl.Map = null;
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public static void OpenMxd(string mxdPath)
        {
            AxMapControl.LoadMxFile(mxdPath);
        }

        public static void CloseMxd()
        {
            // Load blank mxd.
        }
    }
}
