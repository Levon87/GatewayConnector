using RabbitMQ.Client;
using System.Net.Sockets;
using System.Text;

class DeviceServerServiceS
{
    static async Task Main(string[] args)
    {
        int port = 8888;

        TcpListener server = null;
        try
        {
            server = new TcpListener(System.Net.IPAddress.Any, port);
            server.Start();

            Console.WriteLine("Сервер запущен. Ожидание подключений...");

            while (true)
            {
                TcpClient client = await server.AcceptTcpClientAsync();
                Console.WriteLine("Получено новое подключение.");

                _ = HandleClientAsync(client);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Ошибка: " + e.Message);
        }
        finally
        {
            server?.Stop();
        }
    }

    static async Task HandleClientAsync(TcpClient client)
     {
        byte[] buffer = new byte[1024];
        StringBuilder messageBuilder = new StringBuilder();
        try
        {
            using (NetworkStream stream = client.GetStream())
            {
                int bytesRead;
                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    messageBuilder.Append(Encoding.ASCII.GetString(buffer, 0, bytesRead));
                }
            }

            string message = messageBuilder.ToString();
            Console.WriteLine("Получено сообщение от клиента: " + message);


            SendMessageToRabbitMQ(message);
            Console.WriteLine("Сообщение отправлено в RabbitMQ.");
        }
        catch (Exception e)
        {
            Console.WriteLine("Ошибка при обработке клиента: " + e.Message);
        }
        finally
        {
            client.Close();
        }
    }

    static void SendMessageToRabbitMQ(string message)
    {
        var factory = new ConnectionFactory() { HostName = "host.docker.internal" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            string queueName = "logQueue";
            channel.QueueDeclare(queue: queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "",
                                 routingKey: queueName,
                                 basicProperties: null,
                                 body: body);
        }
    }
}
