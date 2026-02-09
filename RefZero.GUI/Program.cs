using System;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Build.Locator;

namespace RefZero.GUI
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Register MSBuild
            try
            {
                var instances = MSBuildLocator.QueryVisualStudioInstances().OrderByDescending(i => i.Version);
                var instance = instances.FirstOrDefault();

                if (instance != null)
                {
                    MSBuildLocator.RegisterInstance(instance);
                }
            }
            catch (Exception)
            {
                // Fallback or handle error appropriately in GUI context
            }

            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}