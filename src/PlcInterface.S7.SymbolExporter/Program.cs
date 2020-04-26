using System;

namespace PlcInterface.S7.SymbolExporter
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var assemblyResolver = new AssemblyResolver(Array.Empty<string>(), new[] { @"C:\Program Files\Siemens\Automation\Portal V15\PublicAPI\V15\", @"C:\Program Files\Siemens\Automation\Portal V15\Bin\PublicAPI" });
            var settings = new Settings();
            var symbolExporter = new SymbolExporter(settings);
            //symbolExporter.Run();
            symbolExporter.ProcessBlocks();
        }
    }
}