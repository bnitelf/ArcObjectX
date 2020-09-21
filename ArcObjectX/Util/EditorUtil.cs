using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcObjectX.Util
{
    /// <summary>
    /// Utility class for Start and Stop Edit ArcGIS Datasource.
    /// Start / Stop Edit = Transaction capability.
    /// Start / Stop Operation = sub action in transaction.
    /// </summary>
    public class EditorUtil
    {
        /// <summary>
        /// Start Edit (without undo, redo capability) then Start Operation.
        /// </summary>
        /// <param name="workspace"></param>
        public static void StartEditAndOperation(IWorkspace workspace)
        {
            IWorkspaceEdit iWorkspaceEdit = workspace as IWorkspaceEdit;
            bool withUndoRedo = false;

            if (!iWorkspaceEdit.IsBeingEdited())
            {
                iWorkspaceEdit.StartEditing(withUndoRedo);
                iWorkspaceEdit.StartEditOperation();
            }
        }

        /// <summary>
        /// Stop Operation then Stop Edit (save changes). 
        /// </summary>
        /// <param name="workspace"></param>
        public static void StopEditAndOperation(IWorkspace workspace)
        {
            IWorkspaceEdit iWorkspaceEdit = workspace as IWorkspaceEdit;
            bool saveEdits = true;

            if (iWorkspaceEdit.IsBeingEdited())
            {
                iWorkspaceEdit.StopEditOperation();
                iWorkspaceEdit.StopEditing(saveEdits);
            }
        }

        /// <summary>
        /// Stop Operation then Stop Edit. 
        /// </summary>
        /// <param name="workspace"></param>
        /// <param name="saveEdits"></param>
        public static void StopEditAndOperation(IWorkspace workspace, bool saveEdits)
        {
            IWorkspaceEdit iWorkspaceEdit = workspace as IWorkspaceEdit;

            if (iWorkspaceEdit.IsBeingEdited())
            {
                iWorkspaceEdit.StopEditOperation();
                iWorkspaceEdit.StopEditing(saveEdits);
            }
        }

        /// <summary>
        /// Rollback (abort) Operation then Stop Edit.
        /// </summary>
        /// <param name="workspace"></param>
        public static void StopEditAndRollback(IWorkspace workspace)
        {
            IWorkspaceEdit iWorkspaceEdit = workspace as IWorkspaceEdit;
            bool saveEdits = false;

            if (iWorkspaceEdit.IsBeingEdited())
            {
                iWorkspaceEdit.AbortEditOperation();
                iWorkspaceEdit.StopEditing(saveEdits);
            }
        }

        /// <summary>
        /// Start edit without undo, redo capability.
        /// </summary>
        /// <param name="workspace"></param>
        public static void StartEdit(IWorkspace workspace)
        {
            IWorkspaceEdit iWorkspaceEdit = workspace as IWorkspaceEdit;
            bool withUndoRedo = false;

            if (!iWorkspaceEdit.IsBeingEdited())
            {
                iWorkspaceEdit.StartEditing(withUndoRedo);
            }
        }

        /// <summary>
        /// Start edit.
        /// </summary>
        /// <param name="workspace"></param>
        /// <param name="withUndoRedo"></param>
        public static void StartEdit(IWorkspace workspace, bool withUndoRedo)
        {
            IWorkspaceEdit iWorkspaceEdit = workspace as IWorkspaceEdit;

            if (!iWorkspaceEdit.IsBeingEdited())
            {
                iWorkspaceEdit.StartEditing(withUndoRedo);
            }
        }

        /// <summary>
        /// Stop Edit (save changes).
        /// </summary>
        /// <param name="workspace"></param>
        public static void StopEdit(IWorkspace workspace)
        {
            IWorkspaceEdit iWorkspaceEdit = workspace as IWorkspaceEdit;
            bool saveEdits = true;

            if (iWorkspaceEdit.IsBeingEdited())
            {
                iWorkspaceEdit.StopEditing(saveEdits);
            }
        }

        /// <summary>
        /// Stop Edit.
        /// </summary>
        /// <param name="workspace"></param>
        /// <param name="saveEdits"></param>
        public static void StopEdit(IWorkspace workspace, bool saveEdits)
        {
            IWorkspaceEdit iWorkspaceEdit = workspace as IWorkspaceEdit;

            if (iWorkspaceEdit.IsBeingEdited())
            {
                iWorkspaceEdit.StopEditing(saveEdits);
            }
        }


        public static void StartOperation(IWorkspace workspace)
        {
            IWorkspaceEdit iWorkspaceEdit = workspace as IWorkspaceEdit;
            if (!iWorkspaceEdit.IsBeingEdited())
            {
                throw new InvalidOperationException("Workspace is not start edit yet.");
            }

            iWorkspaceEdit.StartEditOperation();
        }

        public static void StopOperation(IWorkspace workspace)
        {
            IWorkspaceEdit iWorkspaceEdit = workspace as IWorkspaceEdit;
            if (!iWorkspaceEdit.IsBeingEdited())
            {
                throw new InvalidOperationException("Workspace is not start edit yet.");
            }

            iWorkspaceEdit.StopEditOperation();
        }

        /// <summary>
        /// Abort (rollback) operation.
        /// </summary>
        /// <param name="workspace"></param>
        public static void AbortOperation(IWorkspace workspace)
        {
            IWorkspaceEdit iWorkspaceEdit = workspace as IWorkspaceEdit;
            if (!iWorkspaceEdit.IsBeingEdited())
            {
                throw new InvalidOperationException("Workspace is not start edit yet.");
            }

            iWorkspaceEdit.AbortEditOperation();
        }
    }
}
