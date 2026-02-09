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
                    MessageBox.Show($"Using MSBuild: {instance.Name} - {instance.Version}\nPath: {instance.MSBuildPath}", "MSBuild Registered");
                }
                else 
                {
                     MessageBox.Show("No Visual Studio instances found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to register MSBuild: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}