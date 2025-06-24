using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace TestConsoleApp.Helpers
{
    public class AssemblyHelper
    {
        /// <summary>
        /// Get all assemblies
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Assembly> GetAllBinDirectoryAssemblies()
        {
            return GetAssemblies();
        }

        /// <summary>
        /// Get assemblies by search pattern. If empty, shows all.
        /// </summary>
        /// <param name="searchPattern"> Default value is '*'</param>
        /// <returns></returns>
        public static IEnumerable<Assembly> GetAssemblies(string searchPattern = "*")
        {
            var assemblies = new List<Assembly>();
            foreach (string dll in Directory.GetFiles(GetBinPath(), $"{searchPattern}.dll", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    assemblies.Add(Assembly.LoadFile(dll));
                }
                catch (FileLoadException)
                { } // The Assembly has already been loaded.
                catch (BadImageFormatException)
                { } // If a BadImageFormatException exception is thrown, the file is not an assembly.

            } // foreach dll

            return assemblies;
        }

        /// <summary>
        /// Get bin folder path
        /// </summary>
        /// <returns></returns>
        public static string GetBinPath()
        {
            var assemblyPath = Assembly.GetEntryAssembly() != null
                ? Assembly.GetEntryAssembly().Location
                : Assembly.GetExecutingAssembly().CodeBase;

            return Path.GetDirectoryName(new Uri(assemblyPath).LocalPath);
        }
    }
}
