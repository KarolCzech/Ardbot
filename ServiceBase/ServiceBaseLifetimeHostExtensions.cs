using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Ardbot.ServiceBase {
    public static class ServiceBaseLifetimeHostExtensions {
        public static IHostBuilder UserServiceBaseLifetime(this IHostBuilder hostBuilder){
            return hostBuilder.ConfigureServices((hostContext, services) => services.AddSingleton<IHostLifetime, ServiceBaseLifetime>());
        }

        public static Task RunAsServiceAsync(this IHostBuilder hostBuilder,
            CancellationToken cancellationToken = default){
                return hostBuilder.UserServiceBaseLifetime().Build().RunAsync(cancellationToken);
            }
    }
}