using ArcObjectX.License;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcObjectX_Sample.ExampleInitArcObjectLicense
{
    class InitLicenseExample
    {
        public void Example1()
        {
            // This example use auto initialize method. 
            // The easiest, simplest way to init ArcObject license.

            LicenseHelper licenseHelper = new LicenseHelper();
            if (licenseHelper.AutoInitialize() == false)
            {
                string message = string.Format("{0}\r\n{1}",
                    licenseHelper.LicenseMessage(),
                    licenseHelper.InitLicenseLogMessage());

                // MessageBox.Show(message);
                Console.WriteLine(message);
            }


            // Code open Form here.

            // Shutdown license when program close.
            licenseHelper.ShutdownLicense();
        }
    }
}
