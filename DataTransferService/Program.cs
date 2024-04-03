using DataTransferService.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DataTransferService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);

            using var serviceProvider = services.BuildServiceProvider();
            var rabbitMQService = serviceProvider.GetRequiredService<TransferService>();

            rabbitMQService.StartListening();          
          
        }

        static void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddSingleton<TransferService>();
        }
    }
}
