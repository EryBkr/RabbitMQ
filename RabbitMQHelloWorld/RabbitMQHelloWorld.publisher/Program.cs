using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMQHelloWorld.publisher
{
    //Publisher tarafı data gönderen taraftır
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

            //Mesaj iletimi için kuyruk oluşturduk
            //ilk parametre kuyruk adı,ikinci parametre kuyruk saklansın mı
            //üçünü parametre sadece bu kanal üzerinden mi kuyruğa bağlanılsın diye soruyor
            //dördüncü kuyruğa bağlı olan subscriber ların hepsi silinirse kuyruk silinsin mi,
            channel.QueueDeclare("hello-queue", true, false, false);

            //İletilecek ifade
            string message = "hello world";

            //İletim byte şeklinde olduğu için byte a çevirdik
            var messageBody = Encoding.UTF8.GetBytes(message);

            //kuyruğa data ekleme
            //ilk parametre exchange ismi
            //exchange miz olmadığı için kanalın da hangi kuyruğa mesajı ileteceğine karar verebilmesi için ikinci parametre de kuyruk ismi veriyoruz
            //Son parametrede mesajımı veriyorum
            channel.BasicPublish(string.Empty, "hello-queue", null, messageBody);

            Console.WriteLine("Mesaj gönderilmiştir");
            Console.ReadLine();
        }
    }
}
