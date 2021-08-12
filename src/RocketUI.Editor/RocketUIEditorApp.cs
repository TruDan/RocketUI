using Chromely;
using Chromely.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace RocketUI.Editor
{
    public class RocketUIEditorApp : ChromelyBasicApp
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            services.AddLogging(configure => configure.AddConsole());

#if DEV
            services.AddSingleton<CefContextMenuHandler, DefaultContextMenuHandler>();
#endif
            RegisterControllerAssembly(services, typeof(RocketUIEditorApp).Assembly);
        }
        
    }
}