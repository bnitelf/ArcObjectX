using System;
using ESRI.ArcGIS;

namespace ArcObjectX.License
{
    internal partial class LicenseInitializer
    {
        /// <summary>
        /// Get or Set ArcGIS Runtime want to using. These runtimes will be binded in declared order.
        /// </summary>
        public ProductCode[] UsedRunTime { get; set; }

        /// <summary>
        /// Get Active (Binded) ArcGIS Runtime.
        /// </summary>
        public ProductCode? ActiveRuntime
        {
            get
            {
                return RuntimeManager.ActiveRuntime?.Product;
            }
        }


        public event EventHandler OnBindingArcGISRuntimeFail;

        public LicenseInitializer()
        {
            ResolveBindingEvent += new EventHandler(BindingArcGISRuntime);

            // Default used runtimes.
            UsedRunTime = new ProductCode[]
            {
                ProductCode.Engine,
                ProductCode.Desktop
            };
        }

        void BindingArcGISRuntime(object sender, EventArgs e)
        {
            //
            // Modify ArcGIS runtime binding code as needed; for example, 
            // the list of products and their binding preference order.
            //
            ProductCode[] supportedRuntimes = UsedRunTime;

            foreach (ProductCode c in supportedRuntimes)
            {
                sbLog.Append($"Binding to ArcGIS Runtime: {c} ");
                if (RuntimeManager.Bind(c))
                {
                    sbLog.AppendLine("Success.");
                    return;
                }
                sbLog.AppendLine("Fail.");
            }

            //
            // Modify the code below on how to handle bind failure
            //

            // Failed to bind, announce and force exit
            Console.WriteLine("ArcGIS runtimes binding failed.");
            OnBindingArcGISRuntimeFail?.Invoke(this, EventArgs.Empty);
            //System.Environment.Exit(0);
        }
    }
}