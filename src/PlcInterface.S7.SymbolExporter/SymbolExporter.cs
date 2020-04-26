using PlcInterface.S7.SymbolExporter.OpenssExport;
using Siemens.Engineering;
using Siemens.Engineering.HW.Features;
using Siemens.Engineering.SW;
using Siemens.Engineering.SW.Blocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace PlcInterface.S7.SymbolExporter
{
    internal class DB
    {
        public DB()
        {
            ChildSymbols = new List<Symbol>();
        }

        public List<Symbol> ChildSymbols
        {
            get;
        }

        public string InstanceOfName
        {
            get;
            internal set;
        }

        public int InstanceOfNumber
        {
            get;
            internal set;
        }

        public bool IsPLCDB
        {
            get;
            internal set;
        }

        public string Name
        {
            get;
            set;
        }

        public int Number
        {
            get;
            set;
        }
    }

    internal class Symbol
    {
        public Symbol()
        {
            ChildSymbols = new List<Symbol>();
        }

        public List<Symbol> ChildSymbols
        {
            get;
        }

        public string DataType
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public int Offset
        {
            get; set;
        }
    }

    internal class SymbolExporter : IDisposable
    {
        private const string ExportPath = @"c:\Blocks";
        private readonly Settings settings;
        private bool disposedValue = false;
        private TiaPortal tiaPortal;
        private TiaPortalProcess tiaProcess;

        public SymbolExporter(Settings settings)
        {
            this.settings = settings;
        }

        public void Dispose()
            => Dispose(true);

        public void ProcessBlocks()
        {
            var firstFile = Directory.EnumerateFiles(ExportPath).First();
            var xd = new XmlDocument();
            xd.Load(firstFile);

            var interfaceNodes = xd.GetElementsByTagName("Interface");
            if (interfaceNodes.Count <= 0)
            {
                Console.WriteLine("No interface fount");
            }

            var parrent = interfaceNodes[0].ParentNode;
            var programmingLanguage = parrent.SelectSingleNode("ProgrammingLanguage");
            var programmingLanguageValue = programmingLanguage.InnerText.ToLowerInvariant();
            if (!programmingLanguageValue.Contains("db"))
            {
                return;
            }

            var db = parrent.Deserialize<AttributeList>();
            WriteSectionInfo(db.Interface.Sections, db.Name);
        }

        internal void Run()
        {
            try
            {
                StartTIA();
                var project = OpenProject(settings.ProjectFile);
                var plcSoftware = project
                    .Devices
                    .SelectMany(x => x.DeviceItems)
                    .Select(x => x.GetService<SoftwareContainer>())
                    .Where(x => x != null)
                    .Select(x => x.Software)
                    .Where(x => x is PlcSoftware)
                    .Cast<PlcSoftware>()
                    .FirstOrDefault();

                ExportBlock<DataBlock>(plcSoftware);

                project.Close();
                Console.WriteLine("Project closed");
                StopTIA();
            }
            finally
            {
                StopTIA();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    StopTIA();
                }

                disposedValue = true;
            }
        }

        private static void WriteSectionInfo(Sections test, string root)
        {
            if (test == null)
            {
                return;
            }

            foreach (var section in test.Section)
            {
                foreach (var member in section.Member)
                {
                    var text = $"{root}.{section.Name}.{member.Name}";
                    Console.WriteLine(text);
                    WriteSectionInfo(member.Sections, text);
                }
            }
        }

        private void ExportBlock<T>(PlcSoftware plcSoftware)
            where T : PlcBlock
        {
            var blocks = plcSoftware
                .BlockGroup
                .Blocks
                .Concat(GetBlocks(plcSoftware.BlockGroup.Groups))
                .Where(x => x is T)
                .Cast<T>();

            if (Directory.Exists(ExportPath))
            {
                Directory.Delete(ExportPath, true);
            }

            _ = Directory.CreateDirectory(ExportPath);

            foreach (var block in blocks)
            {
                Console.WriteLine($"DB{block.Number}: {block.Name}");
                var filename = Path.Combine(ExportPath, block.Name + ".xml");
                block.Export(new FileInfo(filename), ExportOptions.WithDefaults | ExportOptions.WithReadOnly);
            }
        }

        private IEnumerable<PlcBlock> GetBlocks(PlcBlockUserGroupComposition plcBlockUserGroups)
        {
            foreach (var group in plcBlockUserGroups)
            {
                foreach (var block in group.Blocks)
                {
                    yield return block;
                }

                foreach (var block1 in GetBlocks(group.Groups))
                {
                    yield return block1;
                }
            }
        }

        private Project OpenProject(string ProjectPath)
        {
            try
            {
                if (tiaPortal.Projects.Count > 0)
                {
                    Console.WriteLine("Using current open project");
                    return tiaPortal.Projects.First();
                }

                var project = tiaPortal.Projects.Open(new FileInfo(ProjectPath));
                Console.WriteLine($"Project {ProjectPath} opened");
                return project;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while opening project" + ex.Message);
                throw;
            }
        }

        private void PrintServices(IEngineeringServiceProvider serviceProvider, string name)
        {
            var builder = new StringBuilder()
                .AppendLine($"{name} has folowing services:");
            foreach (var serviceInfo in serviceProvider.GetServiceInfos())
            {
                _ = builder.AppendLine($"\t{serviceInfo.Type}");
            }

            Console.Write(builder.ToString());
        }

        private void StartTIA()
        {
            var processes = TiaPortal.GetProcesses();
            if (processes.Count == 1)
            {
                var process = processes.First();
                tiaPortal = process.Attach();
                return;
            }

            if (settings.WithoutUI == true)
            {
                tiaPortal = new TiaPortal(TiaPortalMode.WithoutUserInterface);
                Console.WriteLine("TIA Portal started without user interface");
                tiaProcess = TiaPortal.GetProcesses()[0];
            }
            else
            {
                tiaPortal = new TiaPortal(TiaPortalMode.WithUserInterface);
                Console.WriteLine("TIA Portal started with user interface");
                tiaProcess = TiaPortal.GetProcesses()[0];
            }
        }

        private void StopTIA()
        {
            tiaPortal?.Dispose();
            tiaPortal = null;

            tiaProcess?.Dispose();
            tiaProcess = null;

            Console.WriteLine("TIA Portal disposed");
        }
    }
}