using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using MonoGame.Framework.Utilities;
using NLog;
using NLog.Fluent;
using RocketUI.Design.Host;
using RocketUI.Design.Host.Utilities;
using RocketUI.Input.Listeners;
using RocketUI.Serialization.Xaml;

namespace RocketUI.Design
{
    public class RocketDesignerHost : IDisposable
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public RocketDesignerGame Game { get; }

        public RocketDesignerHostOptions Options { get; }

        public RocketDesignerHost(RocketDesignerHostOptions options)
        {
            //RocketXamlLoader.DesignMode = true;
            
            if (options == null)
                options = new RocketDesignerHostOptions();

            Options = options;
            var guiRendererType = ResolveGuiRendererType(options);
            if (guiRendererType != null)
            {
                //Directory.SetCurrentDirectory(Path.GetFullPath(Path.GetDirectoryName(guiRendererType.Assembly.Location)));
            }
            
            var guiRenderer     = TryCreateGuiRenderer(guiRendererType);
            Game = new RocketDesignerGame(guiRenderer);
            Game.Services.AddService(typeof(IEnumerable<InputListenerFactory>), new List<InputListenerFactory>()
            {
                playerIndex => new MouseInputListener(playerIndex),
                playerIndex => new KeyboardInputListener(playerIndex)
            });

            if (!string.IsNullOrWhiteSpace(options.File))
            {
                Game.Components.Add(new FileWatcherComponent(Game, options.File, OnFileUpdate));
                Game.PreviewScreen(options.File);
            }
        }

        private void OnFileUpdate(string obj)
        {
            Game.PreviewScreen(obj);
        }

        private static IGuiRenderer TryCreateGuiRenderer(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (!type.IsAssignableTo(typeof(IGuiRenderer)))
                throw new InvalidCastException($"Cannot cast type '{type}' to IGuiRenderer!");

            try
            {
                var guiRenderer = Activator.CreateInstance(type);
                if (guiRenderer != null)
                    return guiRenderer as IGuiRenderer;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to create GuiRenderer of type '{0}'", type);
                throw;
            }

            return null;
        }

        private static Type ResolveGuiRendererType(RocketDesignerHostOptions options)
        {
            if (options.AssemblySearchPaths == null)
                throw new ArgumentNullException(nameof(options.AssemblySearchPaths));

            foreach (var searchPath in options.AssemblySearchPaths)
            {
                foreach (var assemblyPathRaw in Directory.GetFiles(searchPath, "*.dll", SearchOption.AllDirectories))
                {
                    try
                    {
                        var assemblyPath = Path.GetFullPath(assemblyPathRaw);
                        Log.Debug("Searching Assembly '{0}'", assemblyPath);

                        var assembly =
                            System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);
                        
                        if (assembly.GetName()?.Name?.StartsWith("RocketUI", StringComparison.InvariantCulture) ??
                            false)
                        {
                            Log.Debug("Skipping Assembly '{0}' because it starts with RocketUI",
                                assembly.GetName()?.Name ?? assemblyPath);
                            continue;
                        }

                        var possibleTypes = assembly.GetTypesWithInterface<IGuiRenderer>();
                            
                        Log.Debug("Found {0} types implementing IGuiRenderer in assembly '{1}'", possibleTypes.Length,
                            assembly.GetName()?.Name ?? assemblyPath);

                        if (!possibleTypes.Any())
                            continue;

                        Type matchingType;

                        if (string.IsNullOrEmpty(options.GuiRendererType))
                            matchingType = possibleTypes.FirstOrDefault();
                        else
                            matchingType = possibleTypes.FirstOrDefault(x =>
                                x.FullName.EndsWith(options.GuiRendererType, StringComparison.InvariantCulture));

                        if (matchingType != null)
                        {
                            Log.Info("Found matching GuiRenderer type: {0}", matchingType);
                            return matchingType;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex,
                            "Exception while trying to search assembly '{0}' for IGuiRenderer implementations.",
                            assemblyPathRaw);
                        //throw;
                    }
                }
            }

            Log.Error("Unable to find a matching IGuiRenderer in any assemblies.");
            return null;
        }

        public void Run()
        {
            Game.Run();
        }

        public void Dispose()
        {
            Game?.Dispose();
        }
    }
}