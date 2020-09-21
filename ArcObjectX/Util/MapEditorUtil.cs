using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcObjectX.Util
{
    /// <summary>
    /// Utility class for Start / Stop Edit ArcGIS Map Control Application.
    /// Before use any function set <see cref="Map"/> first.
    /// </summary>
    public class MapEditorUtil
    {
        public static IEngineEditor engineEditor = new EngineEditorClass();

        public static IMap Map { get; set; }

        public static bool IsStartEditing
        {
            get
            {
                if (engineEditor.EditState != esriEngineEditState.esriEngineStateEditing)
                {
                    return false;
                }

                return true;
            }
        }

        public static bool IsInEditOperation
        {
            get
            {
                if (engineEditor.EditState == esriEngineEditState.esriEngineStateEditing)
                {
                    IWorkspaceEdit2 oWorkspaceEdit = engineEditor.EditWorkspace as IWorkspaceEdit2;
                    return oWorkspaceEdit.IsInEditOperation;
                }
                else
                    return false;
            }
        }

        public static IWorkspace GetEditingWorkspace()
        {
            if (engineEditor.EditState == esriEngineEditState.esriEngineStateNotEditing)
            {
                return null;
            }
            else
            {
                return engineEditor.EditWorkspace;
            }
        }


        public static void StartEdit(string layerName)
        {
            // If an edit session has already been started, exit.
            if (engineEditor.EditState != esriEngineEditState.esriEngineStateNotEditing)
                return;

            IMap map = Map;
            IFeatureLayer featureLayer = GetLayerFromTOC(layerName) as IFeatureLayer;
            IDataset dataset = (IDataset)featureLayer.FeatureClass;
            IWorkspace workspace = dataset.Workspace;
            engineEditor.StartEditing(workspace, map);
        }


        /// <summary>
        /// Start edit from workspace whose layer in TOC.
        /// </summary>
        /// <param name="workspace">workspace must belong to layer in TOC.</param>
        public static void StartEdit(IWorkspace workspace)
        {
            // If an edit session has already been started, exit.
            if (engineEditor.EditState != esriEngineEditState.esriEngineStateNotEditing)
                return;

            IMap map = Map;
            engineEditor.StartEditing(workspace, map);
        }


        public static void StartOperation()
        {
            if (engineEditor.EditState != esriEngineEditState.esriEngineStateEditing)
            {
                throw new InvalidOperationException("Editor State not in Start Editing");
            }
            IWorkspaceEdit oWorkspaceEdit = engineEditor.EditWorkspace as IWorkspaceEdit;
            oWorkspaceEdit.StartEditOperation();
        }


        public static void StopOperation()
        {
            if (engineEditor.EditState != esriEngineEditState.esriEngineStateEditing)
            {
                throw new InvalidOperationException("Editor State not in Start Editing");
            }

            //IWorkspaceEdit2 oWorkspaceEdit = engineEditor.EditWorkspace as IWorkspaceEdit2;
            IWorkspaceEdit oWorkspaceEdit = engineEditor.EditWorkspace as IWorkspaceEdit;
            if (IsInEditOperation)
                oWorkspaceEdit.StopEditOperation();
        }

        public static void AbortOperation()
        {
            if (engineEditor.EditState != esriEngineEditState.esriEngineStateEditing)
            {
                throw new InvalidOperationException("Editor State not in Start Editing");
            }

            //IWorkspaceEdit2 oWorkspaceEdit = engineEditor.EditWorkspace as IWorkspaceEdit2;
            IWorkspaceEdit oWorkspaceEdit = engineEditor.EditWorkspace as IWorkspaceEdit;
            if (IsInEditOperation)
                oWorkspaceEdit.AbortEditOperation();
        }


        private static ILayer GetLayerFromTOC(string layerName)
        {
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
    }
}
