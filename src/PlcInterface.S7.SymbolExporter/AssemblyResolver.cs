using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PlcInterface.S7.SymbolExporter
{
    internal class AssemblyResolver
    {
        private readonly IEnumerable<string> extraDirectories;
        private readonly IEnumerable<string> ignoredAssemblies;

        public AssemblyResolver(IEnumerable<string> ignoredAssemblies, IEnumerable<string> extraDirectories)
        {
            this.extraDirectories = extraDirectories;
            this.ignoredAssemblies = new List<string>(ignoredAssemblies) { "System.Reactive.Debugger" };
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        public virtual Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                var fullname = new AssemblyName(args.Name);
                var fields = args.Name.Split(',');
                var name = fields[0];
                var culture = fields.Length >= 3 ? fields[2] : string.Empty;

                // failing to ignore queries for satellite resource assemblies or using [assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.MainAssembly)]
                // in AssemblyInfo.cs will crash the program on non en-US based system cultures.
                if (name.EndsWith(".XmlSerializers") || (name.EndsWith(".resources") && !culture.EndsWith("neutral")))
                {
                    return null;
                }

                if (ignoredAssemblies.Contains(name))
                {
                    return null;
                }

                //Console.WriteLine("Resolving Assembly: " + fullname);
                var wantedDLL = fullname.Name + ".dll";
                var rootDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var directoriesToSearch = new List<string>(extraDirectories) { rootDir };
                directoriesToSearch.AddRange(Directory.GetDirectories(rootDir, "*", SearchOption.AllDirectories));
                Assembly foundAssembly = null;
                foreach (var dir in directoriesToSearch)
                {
                    foundAssembly = TryLoadFile(dir, wantedDLL);

                    if (foundAssembly != null)
                    {
                        //Console.WriteLine($"Resolved {fullname} in {foundAssembly.Location}");
                        break;
                    }
                }

                if (foundAssembly == null)
                {
                    Console.WriteLine($"Failed to resolve assembly for {args.Name}");
                }

                return foundAssembly;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to resolve assembly for {args.Name}; {ex.Message}");
                return null;
            }
        }

        private Assembly TryLoadFile(string directory, string wantedDLL)
        {
            try
            {
                var dllPath = Path.Combine(directory, wantedDLL);
                dllPath = Environment.ExpandEnvironmentVariables(dllPath);

                if (File.Exists(dllPath))
                {
                    return Assembly.LoadFile(dllPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Assembly resolve failed for {wantedDLL} in {directory}; {ex.Message}");
            }

            return null;
        }
    }
}