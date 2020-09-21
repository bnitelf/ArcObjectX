using ArcObjectX.DataManagement;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcObjectX.Util
{
    /// <summary>
    /// Utility function related to GIS Datasource.
    /// </summary>
    public class DataSourceUtil
    {
        #region Configs
        public static bool ThrowErrorIfNotSuccess = true;
        /// <summary>
        /// Valid value = (oracle, SQLServe, for other db see )
        /// </summary>
        public static string DefaultSdeDBClient = "SQLServer";
        #endregion

        #region Private Members
        private static string MessageCannotOpenDatasource = "Cannot open datasource {0}.";
        #endregion

        #region
        public static string GetDataSourcePath(IWorkspace workspace)
        {
            string dsPath = workspace.PathName;

            if (string.IsNullOrWhiteSpace(dsPath))
            {
                // SDE Datasource
                IDataset ds = workspace as IDataset;
                dsPath = TypeUtil.ToSdeConnectionInfo(ds.PropertySet).ToDsPathLikedString();
            }

            return dsPath;
        }
        #endregion

        #region Open Datasource
        /// <summary>
        /// Support open (mdb, gdb, shp, sde) datasource.
        /// </summary>
        /// <param name="datasourcePath"></param>
        /// <returns></returns>
        public static IWorkspace Open(string datasourcePath)
        {
            IWorkspace workspace = null;
            string ext = Path.GetExtension(datasourcePath).ToUpper();
            switch (ext)
            {
                case ".SDE":
                    workspace = OpenSDE(datasourcePath);
                    break;

                case ".GDB":
                    workspace = OpenGdb(datasourcePath);
                    break;

                case ".MDB":
                    workspace = OpenMdb(datasourcePath);
                    break;

                case ".SHP":
                case "":
                    workspace = OpenShp(datasourcePath);
                    break;

                default:
                    throw new NotSupportedException($"Not support extension ({ext})");
            }

            return workspace;
        }

        /// <summary>
        /// Open SDE data source.
        /// </summary>
        /// <param name="sdeConInfo"></param>
        /// <returns></returns>
        public static IWorkspace Open(SdeConnectionInfo sdeConInfo)
        {
            return OpenSDE(sdeConInfo);
        }

        public static IWorkspace TryOpen(SdeConnectionInfo sdeConInfo, int maxTry, out int tryCount, out List<Exception> exceptions)
        {
            return OpenSDE(sdeConInfo, maxTry, out tryCount, out exceptions);
        }

        public static IWorkspace OpenMdb(string datasourcePath)
        {
            IWorkspaceFactory workspaceFactory = new AccessWorkspaceFactory();
            IWorkspace workspace = Open(datasourcePath, workspaceFactory);
            return workspace;
        }

        public static IWorkspace OpenGdb(string datasourcePath)
        {
            IWorkspaceFactory workspaceFactory = new FileGDBWorkspaceFactoryClass();
            IWorkspace workspace = Open(datasourcePath, workspaceFactory);
            return workspace;
        }

        /// <summary>
        /// Open shapefile (support datasourcePath end with and without ".shp")
        /// </summary>
        /// <param name="datasourcePath"></param>
        /// <returns></returns>
        public static IWorkspace OpenShp(string datasourcePath)
        {
            string folderPath = datasourcePath;
            if (datasourcePath.EndsWith(".shp", StringComparison.OrdinalIgnoreCase))
            {
                folderPath = Path.GetDirectoryName(datasourcePath);
            }

            IWorkspaceFactory workspaceFactory = new ShapefileWorkspaceFactory();
            IWorkspace workspace = Open(datasourcePath, workspaceFactory);
            return workspace;
        }

        private static IWorkspace Open(string datasourcePath, IWorkspaceFactory workspaceFactory)
        {
            IWorkspace workspace = workspaceFactory.OpenFromFile(datasourcePath, 0);

            if (workspace == null)
            {
                if (ThrowErrorIfNotSuccess)
                {
                    throw new IOException(string.Format(MessageCannotOpenDatasource + " workspace null.", datasourcePath));
                }
            }
            return workspace;
        }

        public static IWorkspace OpenSDE(string datasourcePath)
        {
            IWorkspaceFactory workspaceFactory = new SdeWorkspaceFactoryClass();
            IWorkspace workspace = Open(datasourcePath, workspaceFactory);
            return workspace;
        }

        /// <summary>
        /// Open SDE from specified config, Using <see cref="DefaultSdeDBClient"/>
        /// </summary>
        /// <param name="server"></param>
        /// <param name="dbname"></param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public static IWorkspace OpenSDE(string server, string dbname, string user, string pass, string version)
        {
            IWorkspace workspace = OpenSDE(server, dbname, user, pass, version, DefaultSdeDBClient);
            return workspace;
        }

        /// <summary>
        /// Open SDE using OS Authentication Mode and <see cref="DefaultSdeDBClient"/>
        /// </summary>
        /// <param name="server"></param>
        /// <param name="dbname"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public static IWorkspace OpenSDE(string server, string dbname, string version)
        {
            IWorkspace workspace = OpenSDE(server, dbname, version, DefaultSdeDBClient);
            return workspace;
        }

        public static IWorkspace OpenSDE(SdeConnectionInfo sdeConInfo)
        {
            string dbClient = sdeConInfo.DbClient.ToIPropertyString();

            IWorkspace workspace = OpenSDE(sdeConInfo.Server,
                                           sdeConInfo.DBName,
                                           sdeConInfo.User,
                                           sdeConInfo.Pass,
                                           sdeConInfo.Version,
                                           dbClient);
            return workspace;
        }

        public static IWorkspace OpenSDE(SdeConnectionInfo sdeConInfo, int maxTry, out int tryCount, out List<Exception> exceptions)
        {
            string dbClient = sdeConInfo.DbClient.ToIPropertyString();

            IWorkspace workspace = TryOpenSDE(sdeConInfo.Server,
                                           sdeConInfo.DBName,
                                           sdeConInfo.User,
                                           sdeConInfo.Pass,
                                           sdeConInfo.Version,
                                           dbClient,
                                           maxTry,
                                           out tryCount,
                                           out exceptions);
            return workspace;
        }

        public static IWorkspace OpenSDE(string server, string dbname, string user, string pass, string version, string dbClient)
        {
            IWorkspace workspace = null;

            IPropertySet propertySet = new PropertySetClass();
            propertySet.SetProperty("Server", server);
            propertySet.SetProperty("DB_CONNECTION_PROPERTIES", server);
            propertySet.SetProperty("Database", dbname);
            //propertySet.SetProperty("Instance", $"sde:sqlserver:{DBServer}");
            propertySet.SetProperty("user", user);
            propertySet.SetProperty("password", pass);
            propertySet.SetProperty("version", version); // ex. sde.Default
            propertySet.SetProperty("DBCLIENT", dbClient);

            IWorkspaceFactory workspaceFactory = new SdeWorkspaceFactoryClass();
            workspace = workspaceFactory.Open(propertySet, 0);

            string datasourcePath = $"SDE, Server={server}, DB={dbname}, User={user}, version={version}, dbClient={dbClient}";

            if (workspace == null)
            {
                if (ThrowErrorIfNotSuccess)
                {
                    throw new IOException(GetMessageCannotOpenDatasourceCauseNull(datasourcePath));
                }
            }

            return workspace;
        }

        /// <summary>
        /// Try Open SDE by specified max try number. Useful in case SDE server is not stable. 
        /// Sometimes you will have to try connecting to the datasource for some amount of times to connect successfully.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="dbname"></param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <param name="version"></param>
        /// <param name="dbClient"></param>
        /// <param name="maxTry"></param>
        /// <param name="tryCount"></param>
        /// <param name="exceptions"></param>
        /// <returns></returns>
        public static IWorkspace TryOpenSDE(string server, string dbname, string user, string pass, string version, string dbClient, int maxTry, out int tryCount, out List<Exception> exceptions)
        {
            IWorkspace workspace = null;
            tryCount = 1;
            exceptions = new List<Exception>();

            while (tryCount <= maxTry)
            {
                try
                {
                    workspace = OpenSDE(server, dbname, user, pass, version, dbClient);
                    if (workspace == null)
                    {
                        string datasourcePath = GetSDEDatasourcePath(server, dbname, user, pass, version, dbClient);
                        throw new IOException(GetMessageCannotOpenDatasourceCauseNull(datasourcePath));
                    }
                    else
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    tryCount++;
                }
            }

            if (tryCount > maxTry)
            {
                if (ThrowErrorIfNotSuccess)
                {
                    string datasourcePath = GetSDEDatasourcePath(server, dbname, user, pass, version, dbClient);
                    throw new IOException(GetMessageCannotOpenDatasourceCauseMaxTryReach(datasourcePath, maxTry));
                }
            }
            return workspace;
        }

        /// <summary>
        /// Open SDE using OS Authentication Mode.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="dbname"></param>
        /// <param name="version"></param>
        /// <param name="dbClient"></param>
        /// <returns></returns>
        public static IWorkspace OpenSDE(string server, string dbname, string version, string dbClient)
        {
            IWorkspace workspace = null;

            IPropertySet propertySet = new PropertySetClass();
            propertySet.SetProperty("Server", server);
            propertySet.SetProperty("DB_CONNECTION_PROPERTIES", server);
            propertySet.SetProperty("Database", dbname);
            propertySet.SetProperty("version", version); // ex. sde.Default
            propertySet.SetProperty("DBCLIENT", dbClient);
            propertySet.SetProperty("AUTHENTICATION_MODE", "OSA"); // Valid value "OSA", "DBMS"

            IWorkspaceFactory workspaceFactory = new SdeWorkspaceFactoryClass();
            workspace = workspaceFactory.Open(propertySet, 0);

            string datasourcePath = $"SDE, Server={server}, DB={dbname}, AuthenMode=OSA, version={version}, dbClient={dbClient}";

            if (workspace == null)
            {
                if (ThrowErrorIfNotSuccess)
                {
                    throw new IOException(string.Format(MessageCannotOpenDatasource + " workspace null.", datasourcePath));
                }
            }

            return workspace;
        }

        public static IWorkspaceFactory GetWorkspaceFactory(string datasourcePath)
        {
            if (datasourcePath.EndsWith(".mdb", StringComparison.OrdinalIgnoreCase))
            {
                return new AccessWorkspaceFactoryClass();
            }
            else if (datasourcePath.EndsWith(".gdb", StringComparison.OrdinalIgnoreCase))
            {
                return new FileGDBWorkspaceFactoryClass();
            }
            else if (datasourcePath.EndsWith(".sde", StringComparison.OrdinalIgnoreCase))
            {
                return new SdeWorkspaceFactoryClass();
            }
            else
            {
                // Expect Shapefile folder
                return new ShapefileWorkspaceFactoryClass();
            }
        }
        #endregion Open Datasource

        /// <summary>
        /// Create Sde IPropertySet for use to connect to sde. Using DBMS Authentication Mode.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="dbname"></param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <param name="version"></param>
        /// <param name="dbClient"></param>
        /// <returns></returns>
        public static IPropertySet CreateSdeIPropertySet(string server, string dbname, string user, string pass, string version, string dbClient)
        {
            IPropertySet propertySet = new PropertySetClass();
            propertySet.SetProperty("Server", server);
            propertySet.SetProperty("Database", dbname);
            //propertySet.SetProperty("Instance", $"sde:sqlserver:{DBServer}");
            propertySet.SetProperty("user", user);
            propertySet.SetProperty("password", pass);
            propertySet.SetProperty("version", version); // ex. sde.Default
            propertySet.SetProperty("DBCLIENT", dbClient);

            return propertySet;
        }

        /// <summary>
        /// Create Sde IPropertySet for use to connect to sde. Using OS Authentication Mode.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="dbname"></param>
        /// <param name="version"></param>
        /// <param name="dbClient"></param>
        /// <returns></returns>
        public static IPropertySet CreateSdeIPropertySet(string server, string dbname, string version, string dbClient)
        {
            IPropertySet propertySet = new PropertySetClass();
            propertySet.SetProperty("Server", server);
            propertySet.SetProperty("Database", dbname);
            propertySet.SetProperty("version", version); // ex. sde.Default
            propertySet.SetProperty("DBCLIENT", dbClient);
            propertySet.SetProperty("AUTHENTICATION_MODE", "OSA"); // Valid value "OSA", "DBMS"

            return propertySet;
        }

        #region SDE
        public static List<string> GetAllSDEVersionName(IWorkspace workspace)
        {
            List<string> versionNames = new List<string>();

            IVersionedWorkspace versionedWorkspace = workspace as IVersionedWorkspace;

            if (versionedWorkspace != null)
            {
                //get a enumeration of all the versions on the versioned workspace
                IEnumVersionInfo enumVersionInfo = versionedWorkspace.Versions;
                enumVersionInfo.Reset();

                IVersionInfo versionInfo = enumVersionInfo.Next();
                while (versionInfo != null)
                {
                    versionNames.Add(versionInfo.VersionName);
                    versionInfo = enumVersionInfo.Next();
                }
            }

            return versionNames;
        }
        #endregion

        #region Create Datasource

        /// <summary>
        /// Support create mdb, gdb, shp
        /// </summary>
        /// <param name="datasourcePath"></param>
        public static void Create(string datasourcePath)
        {
            string ext = Path.GetExtension(datasourcePath).ToUpper();
            switch (ext)
            {
                case ".GDB":
                    CreateGdb(datasourcePath);
                    break;

                case ".MDB":
                    CreateMdb(datasourcePath);
                    break;

                case ".SHP":
                case "":
                    CreateShp(datasourcePath);
                    break;

                default:
                    throw new NotSupportedException($"Not support extension ({ext})");
            }
        }

        public static void CreateGdb(string datasourcePath)
        {
            IWorkspaceFactory workspaceFactory = new FileGDBWorkspaceFactoryClass();
            Create(datasourcePath, workspaceFactory);
        }

        public static void CreateMdb(string datasourcePath)
        {
            IWorkspaceFactory workspaceFactory = new AccessWorkspaceFactoryClass();
            Create(datasourcePath, workspaceFactory);
        }

        public static void CreateShp(string datasourcePath)
        {
            string folderPath = datasourcePath;
            if (datasourcePath.EndsWith(".shp", StringComparison.OrdinalIgnoreCase))
            {
                folderPath = Path.GetDirectoryName(datasourcePath);
            }

            // todo: [Nut] recheck if correct
            IWorkspaceFactory workspaceFactory = new ShapefileWorkspaceFactory();
            Create(folderPath, workspaceFactory);
        }

        private static void Create(string datasourcePath, IWorkspaceFactory workspaceFactory)
        {
            string parentFolderPath = Path.GetDirectoryName(datasourcePath);
            string fileName = Path.GetFileName(datasourcePath);
            IWorkspaceName workspaceName = workspaceFactory.Create(parentFolderPath, fileName, null, 0);

            //// Cast the workspace name object to the IName interface and open the workspace.
            //IName name = (IName)workspaceName;
            //IWorkspace workspace = (IWorkspace)name.Open();
            //return workspace;
        }

        /// <summary>
        /// Create .sde file.
        /// </summary>
        public static void CreateSDEConnectionFile(string sdeConnectionFilepath, string server, string dbname, string user, string pass, string version, string dbClient)
        {
            IPropertySet propertySet = CreateSdeIPropertySet(server, dbname, user, pass, version, dbClient);
            IWorkspaceFactory workspaceFactory = new SdeWorkspaceFactoryClass();

            string parentFolderPath = Path.GetDirectoryName(sdeConnectionFilepath);
            string fileName = Path.GetFileName(sdeConnectionFilepath);

            workspaceFactory.Create(parentFolderPath, fileName, propertySet, 0);
        }

        #endregion


        #region Check if Layer / Table Exists
        /// <summary>
        /// Check if Layer or Table with specified name exist.
        /// </summary>
        /// <param name="workspace"></param>
        /// <param name="layerName"></param>
        /// <returns></returns>
        public static bool IsLayerExist(IWorkspace workspace, string layerName)
        {
            if (((IWorkspace2)workspace).get_NameExists(esriDatasetType.esriDTFeatureClass, layerName))
            {
                return true;
            }

            if (((IWorkspace2)workspace).get_NameExists(esriDatasetType.esriDTTable, layerName))
            {
                return true;
            }

            return false;
        }

        public static bool IsTableExist(IWorkspace workspace, string tableName)
        {
            if (((IWorkspace2)workspace).get_NameExists(esriDatasetType.esriDTTable, tableName))
            {
                return true;
            }

            return false;
        }
        #endregion


        #region Get Layer / Table
        public static IFeatureClass GetFeatureClass(IWorkspace workspace, string featureClassName)
        {
            IFeatureWorkspace ftWorkspace = workspace as IFeatureWorkspace;

            if (!IsLayerExist(workspace, featureClassName) && ThrowErrorIfNotSuccess)
            {
                string dsPath = GetDataSourcePath(workspace);
                throw new Exception($"Layer \"{featureClassName}\" not exists in {dsPath}");
            }

            IFeatureClass fclass = ftWorkspace.OpenFeatureClass(featureClassName);
            if (fclass == null && ThrowErrorIfNotSuccess)
            {
                string dsPath = GetDataSourcePath(workspace);
                throw new Exception($"Can not get layer \"{featureClassName}\" from {dsPath}.");
            }
            return fclass;
        }

        public static ITable GetTable(IWorkspace workspace, string tableName)
        {
            IFeatureWorkspace ftWorkspace = workspace as IFeatureWorkspace;

            if (!IsTableExist(workspace, tableName) && ThrowErrorIfNotSuccess)
            {
                string dsPath = GetDataSourcePath(workspace);
                throw new Exception($"Table \"{tableName}\" not exists in {dsPath}");
            }


            ITable table = ftWorkspace.OpenTable(tableName);
            if (table == null && ThrowErrorIfNotSuccess)
            {
                string dsPath = GetDataSourcePath(workspace);
                throw new Exception($"Can not get table \"{tableName}\" from {dsPath}.");
            }
            return table;
        }

        public static List<string> GetAllLayerName(IWorkspace workspace)
        {
            List<string> result = new List<string>();
            IEnumDataset oEnumDataset = null;
            IFeatureWorkspace oFeatureWorkspace = null;
            IDataset dataset = null;

            oEnumDataset = workspace.get_Datasets(esriDatasetType.esriDTFeatureClass);
            oFeatureWorkspace = workspace as IFeatureWorkspace;

            while ((dataset = oEnumDataset.Next()) != null)
            {
                // Get Feature Class
                if (dataset.Type == esriDatasetType.esriDTFeatureClass)
                {
                    result.Add(dataset.Name);
                }
            }

            return result;
        }

        public static List<string> GetAllTableName(IWorkspace workspace)
        {
            List<string> result = new List<string>();
            IEnumDataset oEnumDataset = null;
            IFeatureWorkspace oFeatureWorkspace = null;
            IDataset dataset = null;

            oEnumDataset = workspace.get_Datasets(esriDatasetType.esriDTTable);
            oFeatureWorkspace = workspace as IFeatureWorkspace;

            while ((dataset = oEnumDataset.Next()) != null)
            {
                // Get Feature Class
                if (dataset.Type == esriDatasetType.esriDTTable)
                {
                    result.Add(dataset.Name);
                }
            }

            return result;
        }

        public static List<string> GetAllTableNameExceptAttachmentTable(IWorkspace workspace)
        {
            List<string> result = GetAllTableName(workspace);
            result = result.Where(x => !x.EndsWith("__ATTACH", StringComparison.OrdinalIgnoreCase)).ToList();
            return result;
        }
        #endregion


        #region Delete Layer / Table
        /// <summary>
        /// Delete Layer / Table if exists.
        /// </summary>
        /// <param name="workspace"></param>
        /// <param name="layerName"></param>
        public void DeleteLayerIfExist(IWorkspace workspace, string layerName)
        {
            if (((IWorkspace2)workspace).get_NameExists(esriDatasetType.esriDTFeatureClass, layerName))
            {
                IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)workspace;
                // ClassCastException here
                IDataset dataset = (IDataset)featureWorkspace.OpenFeatureClass(layerName);
                if (dataset.CanDelete())
                {
                    dataset.Delete();
                }
                else
                {
                    // alert: the feature class exists but couldn't be deleted
                    throw new InvalidOperationException($"Layer {layerName} exist but cannot delete.");
                }
                return;
            }

            if (((IWorkspace2)workspace).get_NameExists(esriDatasetType.esriDTTable, layerName))
            {
                IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)workspace;
                // ClassCastException here
                IDataset dataset = (IDataset)featureWorkspace.OpenTable(layerName);
                if (dataset.CanDelete())
                {
                    dataset.Delete();
                }
                else
                {
                    // alert: the feature class exists but couldn't be deleted
                    throw new InvalidOperationException($"Table {layerName} exist but cannot delete.");
                }
                return;
            }
        }
        #endregion

        #region Create Layer / Table
        /// <summary>
        /// Create feature class. If exist open feature class.
        /// </summary>
        /// <param name="workspace">datasource to create feature class in</param>
        /// <param name="featureDataset">dataset used to create feature class in, use null if don't create in dataset.</param>
        /// <param name="featureClassName"></param>
        /// <param name="fields">fields to create</param>
        /// <param name="CLSID">default null</param>
        /// <param name="CLSEXT">default null</param>
        /// <param name="strConfigKeyword">default null / blank (string.Empty)</param>
        /// <returns></returns>
        public static IFeatureClass CreateOrOpenFeatureClass(IWorkspace workspace, IFeatureDataset featureDataset, string featureClassName, IFields fields, UID CLSID, UID CLSEXT, string strConfigKeyword)
        {
            if (featureClassName == "") return null; // name was not passed in 

            IFeatureClass featureClass;
            IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)workspace; // Explicit Cast
            IWorkspace2 workspace2 = (IWorkspace2)workspace;

            if (workspace2.get_NameExists(esriDatasetType.esriDTFeatureClass, featureClassName)) //feature class with that name already exists 
            {
                featureClass = featureWorkspace.OpenFeatureClass(featureClassName);
                return featureClass;
            }

            // assign the class id value if not assigned
            if (CLSID == null)
            {
                CLSID = new UIDClass();
                CLSID.Value = "esriGeoDatabase.Feature";
            }

            IObjectClassDescription objectClassDescription = new FeatureClassDescriptionClass();

            // if a fields collection is not passed in then supply our own
            if (fields == null)
            {
                // create the fields using the required fields method (default, required fields)
                fields = objectClassDescription.RequiredFields;

                //IFieldsEdit fieldsEdit = (IFieldsEdit)fields; // Explicit Cast
                //IField field = new FieldClass();

                //// create a user defined text field
                //IFieldEdit fieldEdit = (IFieldEdit)field; // Explicit Cast

                //// setup field properties
                //fieldEdit.Name_2 = "SampleField";
                //fieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
                //fieldEdit.IsNullable_2 = true;
                //fieldEdit.AliasName_2 = "Sample Field Column";
                //fieldEdit.DefaultValue_2 = "test";
                //fieldEdit.Editable_2 = true;
                //fieldEdit.Length_2 = 100;

                //// add field to field collection
                //fieldsEdit.AddField(field);
                //fields = (IFields)fieldsEdit; // Explicit Cast
            }

            System.String strShapeField = "";

            // locate the shape field
            for (int j = 0; j < fields.FieldCount; j++)
            {
                if (fields.get_Field(j).Type == esriFieldType.esriFieldTypeGeometry)
                {
                    strShapeField = fields.get_Field(j).Name;
                }
            }

            // Use IFieldChecker to create a validated fields collection.
            IFieldChecker fieldChecker = new FieldCheckerClass();
            IEnumFieldError enumFieldError = null;
            IFields validatedFields = null;
            fieldChecker.ValidateWorkspace = (IWorkspace)workspace;
            fieldChecker.Validate(fields, out enumFieldError, out validatedFields);

            // The enumFieldError enumerator can be inspected at this point to determine 
            // which fields were modified during validation.

            // finally create and return the feature class
            if (featureDataset == null)// if no feature dataset passed in, create at the workspace level
            {
                featureClass = featureWorkspace.CreateFeatureClass(featureClassName, validatedFields, CLSID, CLSEXT, esriFeatureType.esriFTSimple, strShapeField, strConfigKeyword);
            }
            else
            {
                featureClass = featureDataset.CreateFeatureClass(featureClassName, validatedFields, CLSID, CLSEXT, esriFeatureType.esriFTSimple, strShapeField, strConfigKeyword);
            }
            return featureClass;
        }

        /// <summary>
        /// Create feature class. If exist open feature class.
        /// </summary>
        /// <param name="workspace"></param>
        /// <param name="featureClassName"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static IFeatureClass CreateOrOpenFeatureClass(IWorkspace workspace, string featureClassName, IFields fields)
        {
            IFeatureDataset featureDataset = null; // no dataset.
            return CreateOrOpenFeatureClass(workspace, featureDataset, featureClassName, fields, null, null, "");
        }

        /// <summary>
        /// Create feature class in dataset. If exist open feature class.
        /// </summary>
        /// <param name="workspace"></param>
        /// <param name="featureDataset"></param>
        /// <param name="featureClassName"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static IFeatureClass CreateOrOpenFeatureClass(IWorkspace workspace, IFeatureDataset featureDataset, string featureClassName, IFields fields)
        {
            // but if featureDataset is null. feature class will be create without dataset.
            return CreateOrOpenFeatureClass(workspace, featureDataset, featureClassName, fields, null, null, "");
        }

        ///<summary>Creates a table. If exist open table.</summary>
        /// 
        ///<param name="workspace">An IWorkspace interface</param>
        ///<param name="tableName">A System.String of the table name in the workspace. Example: "owners"</param>
        ///<param name="fields">An IFields interface or Nothing</param>
        ///  
        ///<returns>An ITable interface or Nothing</returns>
        ///  
        ///<remarks>
        ///Notes:
        ///(1) If an IFields interface is supplied for the 'fields' collection it will be used to create the
        ///    table. If a Nothing value is supplied for the 'fields' collection, a table will be created using 
        ///    default values in the method.
        ///(2) If a table with the supplied 'tableName' exists in the workspace an ITable will be returned.
        ///    if table does not exit a new one will be created.
        ///</remarks>
        public static ITable CreateOrOpenTable(IWorkspace workspace, string tableName, IFields fields)
        {
            // create the behavior clasid for the featureclass
            UID uid = new UIDClass();

            if (workspace == null) return null; // valid feature workspace not passed in as an argument to the method

            IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)workspace; // Explicit Cast
            IWorkspace2 workspace2 = (IWorkspace2)workspace;
            ITable table;

            if (workspace2.get_NameExists(esriDatasetType.esriDTTable, tableName))
            {
                // table with that name already exists return that table 
                table = featureWorkspace.OpenTable(tableName);
                return table;
            }

            uid.Value = "esriGeoDatabase.Object";

            IObjectClassDescription objectClassDescription = new ObjectClassDescriptionClass();

            // if a fields collection is not passed in then supply our own
            if (fields == null)
            {
                // create the fields using the required fields method
                fields = objectClassDescription.RequiredFields;
                //IFieldsEdit fieldsEdit = (IFieldsEdit)fields; // Explicit Cast

                //IField field = new FieldClass();

                //// create a user defined text field
                //IFieldEdit fieldEdit = (IFieldEdit)field; // Explicit Cast

                //// setup field properties
                //fieldEdit.Name_2 = "SampleField";
                //fieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
                //fieldEdit.IsNullable_2 = true;
                //fieldEdit.AliasName_2 = "Sample Field Column";
                //fieldEdit.DefaultValue_2 = "test";
                //fieldEdit.Editable_2 = true;
                //fieldEdit.Length_2 = 100;

                //// add field to field collection
                //fieldsEdit.AddField(field);
                //fields = (IFields)fieldsEdit; // Explicit Cast
            }

            // Use IFieldChecker to create a validated fields collection.
            IFieldChecker fieldChecker = new FieldCheckerClass();
            IEnumFieldError enumFieldError = null;
            IFields validatedFields = null;
            fieldChecker.ValidateWorkspace = (IWorkspace)workspace;
            fieldChecker.Validate(fields, out enumFieldError, out validatedFields);

            // The enumFieldError enumerator can be inspected at this point to determine 
            // which fields were modified during validation.


            // create and return the table
            table = featureWorkspace.CreateTable(tableName, validatedFields, uid, null, "");

            return table;
        }

        /// <summary>
        /// Copy schema of specified layer from workspaceSrc to workspaceDest.
        /// </summary>
        /// <param name="workspaceSrc"></param>
        /// <param name="workspaceDest"></param>
        /// <param name="layerNameSrc"></param>
        /// <param name="layerNameDest"></param>
        public static void CopySchema(IWorkspace workspaceSrc, IWorkspace workspaceDest, string layerNameSrc, string layerNameDest)
        {
            if (!IsLayerExist(workspaceSrc, layerNameSrc))
            {
                throw new InvalidOperationException($"Layer/Table {layerNameSrc} is not exist in Src workspace");
            }
            if (IsLayerExist(workspaceDest, layerNameDest))
            {
                throw new InvalidOperationException($"Layer/Table {layerNameDest} already exist in Dest workspace");
            }

            IDataset datasetSrc = null;
            IFields fields = null;
            UID clsid = null;
            UID clsext = null;

            if (!IsTableExist(workspaceSrc, layerNameSrc))
            {
                // Src is Feature class
                IFeatureClass fclass = ((IFeatureWorkspace)workspaceSrc).OpenFeatureClass(layerNameSrc);
                datasetSrc = fclass as IDataset;

                fields = fclass.Fields;
                clsid = fclass.CLSID;
                clsext = fclass.EXTCLSID;

                IFeatureDataset ftDataset = null; // no dataset 
                CreateOrOpenFeatureClass(workspaceDest, ftDataset, layerNameDest, fields, clsid, clsext, "");
            }
            else
            {
                // Src is Table
                ITable table = ((IFeatureWorkspace)workspaceSrc).OpenTable(layerNameSrc);
                datasetSrc = table as IDataset;

                fields = table.Fields;
                clsid = table.CLSID;
                clsext = table.EXTCLSID;

                CreateOrOpenTable(workspaceDest, layerNameDest, fields);
            }
        }
        #endregion

        #region Get Layer / Table Name
        /// <summary>
        /// Get SDE table / layer name. Ex. "tableOwner.layerName"
        /// </summary>
        /// <param name="tableOwner"></param>
        /// <param name="layerName"></param>
        /// <returns></returns>
        public static string GetQualifiedLayerName(string tableOwner, string layerName)
        {
            return $"{tableOwner}.{layerName}";
        }

        /// <summary>
        /// Get SDE table / layer name. Ex. "tableOwner.layerName"
        /// </summary>
        /// <param name="sdeConInfo"></param>
        /// <param name="layerName"></param>
        /// <returns></returns>
        public static string GetQualifiedLayerName(SdeConnectionInfo sdeConInfo, string layerName)
        {
            return $"{sdeConInfo.TableOwner}.{layerName}";
        }

        /// <summary>
        /// Get SDE Full Qualified table / layer name. Ex. "Server.tableOwner.layerName"
        /// </summary>
        /// <param name="sdeConInfo"></param>
        /// <param name="layerName"></param>
        /// <returns></returns>
        public static string GetFullQualifiedLayerName(SdeConnectionInfo sdeConInfo, string layerName)
        {
            return $"{sdeConInfo.Server}.{sdeConInfo.TableOwner}.{layerName}";
        }
        #endregion

        #region Get Message 
        /// <summary>
        /// For get default message when Cannot Open Datasource.
        /// </summary>
        /// <param name="datasourcePath"></param>
        /// <returns></returns>
        private static string GetMessageCannotOpenDatasourceCauseNull (string datasourcePath)
        {
            return string.Format(MessageCannotOpenDatasource + " workspace null.", datasourcePath);
        }

        /// <summary>
        /// For get default message when Cannot Open Datasource.
        /// </summary>
        /// <param name="datasourcePath"></param>
        /// <returns></returns>
        private static string GetMessageCannotOpenDatasourceCauseMaxTryReach(string datasourcePath, int maxTry)
        {
            return string.Format(MessageCannotOpenDatasource + $" After tried connect {maxTry} time(s).", datasourcePath);
        }

        /// <summary>
        /// Get default SDE Datasource Path.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="dbname"></param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <param name="version"></param>
        /// <param name="dbClient"></param>
        /// <returns></returns>
        private static string GetSDEDatasourcePath(string server, string dbname, string user, string pass, string version, string dbClient)
        {
            string datasourcePath = $"SDE, Server={server}, DB={dbname}, User={user}, version={version}, dbClient={dbClient}";
            return datasourcePath;
        }
        #endregion
    }
}
