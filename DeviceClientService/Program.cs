using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;
using System.Net.Sockets;
using System.Text;

namespace DeviceClientService
{
    internal class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            // Настройки подключения TCP
             
            string serverIp = "deviceserverservice";

            int port = 8888;

            // Инициализация QUARTZ Job Scheduler
            var schedulerFactory = new StdSchedulerFactory();
            var scheduler = await schedulerFactory.GetScheduler();
            await scheduler.Start();

            // Определение и настройка задания для отправки информации каждые 30 секунд
            var job = JobBuilder.Create<DeviceClientServiceJob>()
                .WithIdentity("deviceClientServiceJob", "deviceClientServiceGroup")
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("deviceClientServiceTrigger", "deviceClientServiceGroup")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(30)
                    .RepeatForever())
                .Build();

            await scheduler.ScheduleJob(job, trigger);

            // Подключение к серверу и отправка данных
            using (var client = new TcpClient(serverIp, port))
            {
                using (var stream = client.GetStream())
                {
                    Console.OutputEncoding = Encoding.UTF8;
                    Console.WriteLine("Успешное подключение к серверу.");

                    // Ожидание завершения работы приложения
                    Console.ReadLine();
                }
            }
        }
    }

    public class DeviceClientServiceJob : IJob
    {
        public async System.Threading.Tasks.Task Execute(IJobExecutionContext context)
        {
             
            var deviceInfo = new
            {
                Time = DateTime.UtcNow,
                IsChecked = false
            };

            string jsonMessage = JsonConvert.SerializeObject(deviceInfo);
            string currentTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");

            // Отправка времени на сервер по TCP
            using (var client = new TcpClient("deviceserverservice", 8888))
            {
                using (var stream = client.GetStream())
                {
                    byte[] data = Encoding.ASCII.GetBytes(jsonMessage);
                    await stream.WriteAsync(data, 0, data.Length);
                    Console.OutputEncoding = Encoding.UTF8;
                    Console.WriteLine($"Данные отправлены на сервер: {currentTime}");
                }
            }
        }
    }
}
