using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DataTransitGateWay.Services
{
    public class DataTransitService : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly HttpClient _httpClient;


        public DataTransitService(HttpClient httpClient)
        {
            _httpClient = httpClient;

            // Создание соединения с RabbitMQ и объявление очереди
            var factory = new ConnectionFactory() { HostName = "host.docker.internal", UserName = "guest", Password = "guest" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            string queueName = "logQueue";
            _channel.QueueDeclare(queue: queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            StartListeningToRabbitMQ();
        }

        public Task StartListeningToRabbitMQ()
        {
            // Создание consumer для получения сообщений из очереди
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                // Получение сообщения из очереди
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                // Логика обработки полученного сообщения перед отправкой в другой сервис
                var processedMessage = ProcessMessage(message);

                // Логика отправки сообщения в  другой сервис
                await SendToLogStorageService(processedMessage);

                // Подтверждение получения сообщения
                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            // Начало прослушивания очереди
            _channel.BasicConsume(queue: "logQueue", autoAck: false, consumer: consumer);

            return Task.CompletedTask;
        }

        // Логика обработки полученного сообщения
        private string ProcessMessage(string message)
        {

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            LogModel log = JsonSerializer.Deserialize<LogModel>(message, options);           

            log.IsChecked = true;
            return JsonSerializer.Serialize(log);
        }

        // Логика отправки сообщения 
        private async Task SendToLogStorageService(string message)
        {
            // Отправка сообщения с использованием HttpClient
            var content = new StringContent(message, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("http://logstorageservice:8080/api/logs", content);

            if (!response.IsSuccessStatusCode)
            {
                // Обработка ошибки при отправке сообщения 
                return;
            }
        }

        //  При надобности  можно вызвать Метод Dispose для закрытия соединения с RabbitMQ
        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }
    }

    class LogModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public DateTime Time { get; set; }

        [Required]
        public bool IsChecked { get; set; }
    }
}
