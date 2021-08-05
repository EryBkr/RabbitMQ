using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Web.Watermark.Services
{
    //Eventi iletecek class
    public class RabbitMqPublisher
    {
        //Servisimi DI ile elde edeceğim
        private readonly RabbitMQClientService _clientService;

        public RabbitMqPublisher(RabbitMQClientService clientService)
        {
            _clientService = clientService;
        }

        //Event i kuyruğa göndereceğim
        public void Publish(ProductImageCreatedEvent createdEvent)
        {
            //Servis aracılığıyla channel oluşturdum
            var channel = _clientService.Connect();
            //Eventi json a cast ettim
            var bodyString = JsonSerializer.Serialize(createdEvent);
            //json datamı byte array a dönüştürdüm
            var eventToByte = Encoding.UTF8.GetBytes(bodyString);

            //mesajlar kayıtlı kalsın
            var property = channel.CreateBasicProperties();
            property.Persistent = true;

            //Go to Queue :)
            channel.BasicPublish(RabbitMQClientService.ExchangeName, RabbitMQClientService.RoutingWaterMark, property, eventToByte);
        }
    }
}
