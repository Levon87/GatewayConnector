using DataTransitGateWay.Services;
using Microsoft.Extensions.Configuration;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Values;
namespace DataTransitGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var configuration = builder.Configuration;
           // builder.Services.AddControllers();
            builder.Services.AddHttpClient();           
            builder.Services.AddOcelot(configuration);
            builder.Services.AddSingleton<DataTransitService>();           

            builder.Services.AddEndpointsApiExplorer();           

            Thread.Sleep(TimeSpan.FromMinutes(1.5));
            var ocelotConfiguration = configuration.GetSection("Ocelot").Get<OcelotConfiguration>();
            builder.Services.AddSingleton(ocelotConfiguration);

            var app = builder.Build();
            app.Services.GetRequiredService<DataTransitService>();

               

            app.UseAuthorization();

            app.MapControllers();

            app.UseOcelot();

            app.Run();
        }
    }
}
