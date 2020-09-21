using ESRI.ArcGIS;
using ESRI.ArcGIS.esriSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcObjectX.License
{
    /// <summary>
    /// 
    /// </summary>
    public class LicenseHelper
    {
        private static LicenseInitializer _licenseInitializer = new LicenseInitializer();

        /// <summary>
        /// Default false
        /// </summary>
        public bool InitializeLowerProductFirst { get; set; }


        /// <summary>
        /// Basic initialization, will try to bind to available ArcGIS Runtimes (from Engine -> Desktop -> Server -> ArcReader)
        /// then try to initialize all available licenses (from highest <see cref="esriLicenseProductCode"/> to lowest). 
        /// But no Extension license is initialized.
        /// 
        /// <para>Call <see cref="ShutdownLicense"/> when program is about to close. To return license back to License Server.</para>
        /// </summary>
        /// <returns></returns>
        public bool AutoInitialize()
        {
            bool success = false;

            // Set default ini
            ProductCode[] arcGisRuntimes = new ProductCode[]
            {
                ProductCode.Engine,
                ProductCode.Desktop,
                ProductCode.Server,
                ProductCode.ArcReader
            };

            esriLicenseProductCode[] licenseProductCodes = new esriLicenseProductCode[]
            {
                esriLicenseProductCode.esriLicenseProductCodeAdvanced,
                esriLicenseProductCode.esriLicenseProductCodeStandard,
                esriLicenseProductCode.esriLicenseProductCodeBasic,
                esriLicenseProductCode.esriLicenseProductCodeArcServer,
                esriLicenseProductCode.esriLicenseProductCodeEngineGeoDB,
                esriLicenseProductCode.esriLicenseProductCodeEngine,
            };

            esriLicenseExtensionCode[] licenseExtensionCodes = new esriLicenseExtensionCode[0];

            _licenseInitializer.InitializeLowerProductFirst = InitializeLowerProductFirst;
            _licenseInitializer.UsedRunTime = arcGisRuntimes;
            success = _licenseInitializer.InitializeApplication(licenseProductCodes, licenseExtensionCodes);
            return success;
        }


        /// <summary>
        /// Get a summary of the status of product and extensions initialization.
        /// </summary>
        /// <returns></returns>
        public string LicenseMessage()
        {
            return _licenseInitializer.LicenseMessage();
        }

        /// <summary>
        /// Get initialized license log message.
        /// </summary>
        /// <returns></returns>
        public string InitLicenseLogMessage()
        {
            return _licenseInitializer.InitLicenseLogMessage;
        }

        /// <summary>
        /// Call every time your program about to close.
        /// </summary>
        public void ShutdownLicense()
        {
            _licenseInitializer.ShutdownApplication();
        }
    }
}
