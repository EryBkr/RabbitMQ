using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQHelloWorld.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace RabbitMQHelloWorld.publisher
{
    //Publisher tarafı data gönderen taraftır
    //Exchange kuyruk ile publisher arasında datanın kuyruklara dağılımını kontrol eden mekanizmadır
    class Program
    {
        static void Main(string[] args)
        {
            //Rabbitmq bağlantı library
            var factory = new ConnectionFactory();

            //Bağlantı noktasını belirledik
            factory.Uri = new Uri("amqp://guest:guest@localhost:5672");

            //Bağlantıyı açtık
            //using in ikinci kullanımı entegre ettik.
            //Bulunduğu fonksiyonun bitiminde garbagecollector işleme girer
            using var connection = factory.CreateConnection();

            //Bağlantı kanalı oluşturuldu
            var channel = connection.CreateModel();

            //Mesajlarımızı kuyruğa değil artık exchange oluşturup gönderiyoruz
            //Header exchange detaylı route işlemi için kullanılır.Key Value şeklinde header aracılığıyla key belirlenir.Route Key kullanılmaz
            //ilk parametre exchange ismidir
            //ikinci parametre exchange kayıt edilsin mi diye soruyor
            //Exchange type belirlendi
            channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);

            //Exchange Header bilgilerini oluşturdum
            Dictionary<string, object> headers = new Dictionary<string, object>();
            headers.Add("format", "pdf");
            headers.Add("shape", "a4");

            //Oluşturduğum header bilgilerini parametre olarak eklemek için paketledim
            var properties = channel.CreateBasicProperties();
            properties.Headers = headers;

            //Mesajların kalıcı hale gelmesi için ekledik.Exchange tiplerine bağlı bir özellik değildir
            properties.Persistent = true;

            //Complex type'ı oluşturdum
            var product = new Product { Id = 1, Name = "Silgi", Price = 15 };

            //Product objemizi json a çevirdik
            var productJsonString = JsonSerializer.Serialize(product);


            //Exchange , complex type ve header bilgileri eklendi
            channel.BasicPublish("header-exchange", string.Empty, properties, Encoding.UTF8.GetBytes(productJsonString));


            Console.ReadLine();
        }
    }
}
