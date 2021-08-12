#define DEV

using System;
using Chromely.Core;
using Chromely.Core.Configuration;
#if DEV
using RocketUI.Editor.Utilities;
#endif

namespace RocketUI.Editor
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var config = DefaultConfiguration.CreateForRuntimePlatform();
#if DEV
            if (!(args.Length > 0 && ushort.TryParse(args[0], out var port)))
            {
                port = 7080;
            }

            config.StartUrl = $"http://localhost:{port}";
            config.DebuggingMode = true;
            using var devserver = StartDevServer(port);
#else
            config.StartUrl = "local://dist/index.html";
#endif

            AppBuilder
                .Create()
                .UseConfig<DefaultConfiguration>(config)
                .UseApp<RocketUIEditorApp>()
                .Build()
                .Run(args);
        }

#if DEV

        private static IDisposable StartDevServer(ushort port = 7080)
        {
            var server = new VueDevServerWrapper(port);
            server.Start();
            return server;
        }
#endif
    }
}