using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using CommandLine.Text;

namespace RocketUI.Design.Host
{
    public static class Program
    {
        class Options : RocketDesignerHostOptions
        {
            [Option('f', "file", Required = false)]
            public override string File
            {
                get => base.File;
                set => base.File = value;
            }

            [Option("renderer", Required = false)]
            public override string GuiRendererType
            {
                get => base.GuiRendererType;
                set => base.GuiRendererType = value;
            }

            [Option("dir", Min = 1, Required = true, Separator = ',')]
            public IEnumerable<string> AssemblySearchPaths
            {
                get => base.AssemblySearchPaths;
                set => base.AssemblySearchPaths = value?.ToArray();
            }

            [Option("assembly", Required = false)]
            public override string AssemblyName
            {
                get => base.AssemblyName;
                set => base.AssemblyName = value;
            }

            [Option('p',"project", Required = false)]
            public string ProjectPath
            {
                get;
                set;
            }
        }
        
        [STAThread]
        static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<Options>(args)
                .WithParsed(RunWithOptions);
        }

        private static void RunWithOptions(Options options)
        {
            using (var host = new RocketDesignerHost(options))
                host.Run();
        }
    }
}