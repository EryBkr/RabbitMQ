using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Linq;
using System.Text;
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
            //Fanout exchamge mesajları ona bağlı olan bütün kuyruklara gönderir
            //ilk parametre exchange ismidir
            //ikinci parametre exchange kayıt edilsin mi diye soruyor
            //Exchange type belirlendi
            channel.ExchangeDeclare("logs-fanout", durable: true, type: ExchangeType.Fanout);

            //For kullanımı gibi çalışır,çoklu data gönderimini test etmek için oluşturduk
            Enumerable.Range(1, 50).ToList().ForEach(data=> 
            {
                //İletilecek ifade
                string message = $"Log {data}";

                //İletim byte şeklinde olduğu için byte a çevirdik
                var messageBody = Encoding.UTF8.GetBytes(message);

                
                //ilk parametre exchange ismi
                //kuyruk ismi tanımlamadım benim exchange 'ime bağlanacak subscriber lar kendi kuyruklarını oluşturup exchange'yi dinlesinler.
                //herhangi bir subscriber yokken mesajlarımı gönderirsem mesajlar haliyle bir kuyruğa iletilememiş olurlar.Fakat oluşturulan kuyruklar kayıt olacak şekilde oluşturulurlarsa subscriber dataları daha sonra da alabilirler tabi en başta kuyrukların oluşmuş olnası gerekiyor
                //Son parametrede mesajımı veriyorum
                channel.BasicPublish("logs-fanout", string.Empty, null, messageBody);

                //mesajlar exchange aktarılırken yavaş aktarılsın ve kuyruktan dinlerkende aktarımı görebilelim
                Thread.Sleep(1000);

                Console.WriteLine("Gönderilen Mesaj: "+message);
            });
         
            Console.ReadLine();
        }
    }
}
