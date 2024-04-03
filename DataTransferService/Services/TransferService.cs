using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace DataTransferService.Services
{
    public class TransferService : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly HttpClient _httpClient;

        public TransferService(HttpClient httpClient)
        {
            _httpClient = httpClient;

            var factory = new ConnectionFactory() { HostName = "host.docker.internal", Port = 5672, UserName = "guest", Password = "guest" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "logQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        public void StartListening()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                ProcessMessage(message);

                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            _channel.BasicConsume(queue: "logQueue", autoAck: false, consumer: consumer);
        }

        private void ProcessMessage(string message)
        {
            // Ваша логика обработки сообщения перед отправкой через HttpClient
            var processedMessage = message.ToUpper();

            SendToOtherService(processedMessage);
        }

        private async void SendToOtherService(string message)
        {
            var content = new StringContent(message, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("http://logstorageservice/api/logs/addlog", content);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Ошибка при отправке сообщения в другой сервис.");
            }
        }

        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }
    }
}
