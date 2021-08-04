using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Linq;
using System.Text;
using System.Threading;

namespace RabbitMQHelloWorld.publisher
{
    public enum LogNames
    {
        Critical = 1,
        Error = 2,
        Warning = 3,
        Info = 4
    }


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
            //Direct exchamge mesajları bizim belirlediğimiz kuyruklara özel olarak iletir
            //ilk parametre exchange ismidir
            //ikinci parametre exchange kayıt edilsin mi diye soruyor
            //Exchange type belirlendi
            channel.ExchangeDeclare("logs-direct", durable: true, type: ExchangeType.Direct);

            //Enum isimlerini foreach ile dönüyorum
            Enum.GetNames(typeof(LogNames)).ToList().ForEach(logType =>
            {
                var queueName = $"direct-queue-{logType}";

                var routeKey = $"route-{logType}";

                //Bazı senaryolarda kuyruk subscriber tarafında oluşturulurdu fakat burada ki exchange tipimiz belirli bir kuyruğa bağlanacağı için burada oluşturma gereği duyduk
                //Her Log tipi için kuyruk oluşturduk
                channel.QueueDeclare(queueName, true, false, false);

                //Kuyruk ile Exchange yi Bind ettik
                //route key Exchange kuyruğa data gönderirken route key e göre gönderme işlemi yapacaktır.Bind ederken bunu da belirliyoruz
                channel.QueueBind(queueName, "logs-direct", routeKey, null);
            });

            //For kullanımı gibi çalışır,çoklu data gönderimini test etmek için oluşturduk
            Enumerable.Range(1, 15).ToList().ForEach(data =>
            {
                //Log tipini random olarak alıyorum
                LogNames log = (LogNames)new Random().Next(1, 4);

                //İletilecek ifade Exp:Log-type Critical
                string message = $"Log-type {log}";

                //İletim byte şeklinde olduğu için byte a çevirdik
                var messageBody = Encoding.UTF8.GetBytes(message);

                var routeKey = $"route-{log}";

                //ilk parametre exchange ismi
                //yukarıda kuyruklarımı ve route key lerimi oluşturdum.
                //Direct Exchange kuyruklara data iletirken route keyden faydalanır ve o kuyruğa özel mesaj gönderir
                //Son parametrede mesajımı veriyorum
                channel.BasicPublish("logs-direct", routeKey, null, messageBody);

                //mesajlar exchange aktarılırken yavaş aktarılsın ve kuyruktan dinlerkende aktarımı görebilelim
                Thread.Sleep(500);

                Console.WriteLine("Log Gönderilmiştir: " + message);
            });

            Console.ReadLine();
        }
    }
}
