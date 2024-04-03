
using LogStorageService.Data;
using LogStorageService.Interfaces;
using LogStorageService.Services;
using Microsoft.EntityFrameworkCore;

namespace LogStorageService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Thread.Sleep(TimeSpan.FromMinutes(1));

            builder.Services.AddDbContext<LogStorageDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("LogStorageDbContext") ??
                throw new InvalidOperationException("Connection string 'LogControlContext' not found.")));
            
            builder.Services.AddTransient<ILogService, LogService>();
            builder.Services.AddControllers();           
            
             
            builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}

            using (var servicescope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = servicescope.ServiceProvider.GetService<LogStorageDbContext>();
                context?.Database.Migrate();
            }             

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
