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
            //Topic exchange detaylı route işlemi için kullanılır.Kuyruklar subscriber tarafında oluşturulacaktır
            //ilk parametre exchange ismidir
            //ikinci parametre exchange kayıt edilsin mi diye soruyor
            //Exchange type belirlendi
            channel.ExchangeDeclare("logs-topic", durable: true, type: ExchangeType.Topic);

            //For kullanımı gibi çalışır,çoklu data gönderimini test etmek için oluşturduk
            Enumerable.Range(1, 16).ToList().ForEach(data =>
            {
                //Topic Exchange regex gibi route key aramamızı sağlar
                //Error.Critical.Info gibi bir route key e sahip olduğumuzu düşünelim *.Critical.* şeklinde ortasında Critical ibaresi barındıran route keylerle bağlanabiliriz
                LogNames log1 = (LogNames)new Random().Next(1, 5);
                LogNames log2 = (LogNames)new Random().Next(1, 5);
                LogNames log3 = (LogNames)new Random().Next(1, 5);
                var routeKey = $"{log1}.{log2}.{log3}";

                //İletilecek ifade Exp:Log-type Critical-Error-Info
                string message = $"Log-type {log1}-{log2}-{log3}";

                //İletim byte şeklinde olduğu için byte a çevirdik
                var messageBody = Encoding.UTF8.GetBytes(message);

                //ilk parametre exchange ismi
                //yukarıda kuyruklarımı ve route key lerimi oluşturdum.
                //Topic Exchange kuyruklara data iletirken route keyden faydalanır ve o kuyruğa özel mesaj gönderir.Route Key araması yukarıda belirttiğim gibi gerçekleşir
                //Son parametrede mesajımı veriyorum
                channel.BasicPublish("logs-topic", routeKey, null, messageBody);

                //mesajlar exchange aktarılırken yavaş aktarılsın ve kuyruktan dinlerkende aktarımı görebilelim
                Thread.Sleep(500);

                Console.WriteLine("Log Gönderilmiştir: " + message);
            });

            Console.ReadLine();
        }
    }
}
