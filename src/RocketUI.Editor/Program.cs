//#define DEV

using System;
using System.Collections.Generic;
using Chromely.Core;
using Chromely.Core.Configuration;
using Chromely.Core.Infrastructure;
using Chromely.Core.Network;
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
            config.WindowOptions.Title = "RocketUI Editor";
            config.AppName = "RocketUI.Editor";
            config.WindowOptions.RelativePathToIconFile = "icon.png";
#if DEV
            if (!(args.Length > 0 && ushort.TryParse(args[0], out var port)))
            {
                port = 7080;
            }

            config.StartUrl = $"http://localhost:{port}";
            config.DebuggingMode = true;
            using var devserver = StartDevServer(port);
#else
            config.StartUrl = "assembly://app/index.html";
            config.CefDownloadOptions.DownloadSilently = true;
            config.CefDownloadOptions.AutoDownloadWhenMissing = true;
            
            var assemblyOptions = new AssemblyOptions("RocketUI.Editor.dll", null, "dist");
            config.UrlSchemes.AddRange(new List<UrlScheme>()
            {
                new UrlScheme(DefaultSchemeName.ASSEMBLYRESOURCE, "assembly", "app", string.Empty,
                    UrlSchemeType.AssemblyResource, false, assemblyOptions)
            });
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