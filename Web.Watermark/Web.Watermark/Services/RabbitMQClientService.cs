using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Watermark.Services
{
    //exchange , kuyruk oluşturma gibi işlemleri üstlenecek olan sınıfımız
    public class RabbitMQClientService : IDisposable
    {
        //DI aracılığıyla gelecek.Startupta tanımladım
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        public static string ExchangeName = "ImageDirectExchange";
        public static string RoutingWaterMark = "watermark-route-image";
        public static string QueueName = "queue-watermark-image";

        private readonly ILogger<RabbitMQClientService> _logger;

        public RabbitMQClientService(ConnectionFactory connectionFactory, ILogger<RabbitMQClientService> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        //Kanalı oluşturuyorum
        public IModel Connect()
        {
            //Connection u oluşturdum
            _connection = _connectionFactory.CreateConnection();

            //Kanal var ise return et
            if (_channel is { IsOpen: true })
            {
                return _channel;
            }
                

            //Kanal oluşturdum
            _channel = _connection.CreateModel();
            //Exchange oluşturdum
            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct, true, false);
            //kuyruk oluşturdum
            _channel.QueueDeclare(QueueName, true, false, false, null);
            //Kuyruğu exchange bind ediyorum
            _channel.QueueBind(exchange: ExchangeName, queue: QueueName, routingKey: RoutingWaterMark);
            //Logladık
            _logger.LogInformation("RabbitMQ bağlantısı oluşturuldu");

            return _channel;
        }

        //Kaynak kullanımının istediğimiz zaman sonlanabilmesi için IDisposable arayüzünü implemente ettik.Dispose metodu içerisinde bu işlemleri kullandık.Using bloglarını artık bu service için kullanabileceğiz
        public void Dispose()
        {
            //Kanal var ise kapat var ise dispose et.
            _channel?.Close();
            _channel?.Dispose();
         
            _connection?.Close();
            _connection?.Dispose();
          
            _logger.LogInformation("RabbitMQ ile bağlantı sonlandırıldı");
        }

    }
}
