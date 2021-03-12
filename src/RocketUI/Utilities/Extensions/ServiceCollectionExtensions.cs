using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RocketUI.Input.Listeners;

namespace RocketUI.Utilities.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInputListenerFactory<T>(this IServiceCollection services,
            InputListenerFactory<T> inputListenerFactory) where T : class, IInputListener
        {
            services.TryAddEnumerable(
                ServiceDescriptor.Singleton<IInputListenerFactory, DefaultInputListenerFactory<T>>(provider => new DefaultInputListenerFactory<T>(inputListenerFactory)                    ));
            return services;
        }
    }
}