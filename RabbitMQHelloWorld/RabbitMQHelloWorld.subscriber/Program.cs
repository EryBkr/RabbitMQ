using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMQHelloWorld.subscriber
{
    //Subscriber tarafı data alan taraftır
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


            //subscriber lara kaçar adet data göndereceğimizi belirliyoruz.
            //ilk parametrenin 0 olması herhangi bir boyutta ki mesajı alabiliriz demek
            //ikinci parametre kaçar adet mesaj gönderileceğini belirliyor
            //üçüncü parametre  true ise mesajlar 1 er 1er dağılatacak false ise toplamı 1 olarak şekilde dağıtılacak örneğin 5 değerini seçseydik hepsine teker teker vererek toplamı 5 yapmaya çalışacak sonra yine tur a devam edecekti.
            channel.BasicQos(0, 1, false);

            var subscriber = new EventingBasicConsumer(channel);

            //Random bir kuyruk ismi oluşturuyoruz
            var queueName = channel.QueueDeclare().QueueName;

            //Topic Exchange kullanma nedenimiz aslında burada başlıyor.Route Key i sanki regex gibi bir ifadeyle kullanabiliyormuşcasına bu şekilde yazabiliyoruz
            var routeKey = "*.Error.*";

            //Kuyruk deklere etmiyorum sadece bind ediyorum.Bağlı olduğu exchange kadar yaşam ömrü olacaktır
            channel.QueueBind(queueName, "logs-topic", routeKey);

            //ilk parametreye kuyruk ismi tanımladık
            //ikinci parametre iletim sonrası mesaj silinsin mi diye soruyor
            //ikinci parametreye false verdiysek eğer ileride rabbitmq ya mesajın silinmesi için haber verebiliriz
            //kuyruk exchange yi dinlemeye başladıktan sonra publisher mesaj gönderdikçe alır tabi bu senaryo kuyruk kayıtlı değilse gerçekleşecektir kuyruk kaydedilecek şekilde oluşturulduysa subscriber kaldığı yerden ya da en başından dataları dinleyecektir.Senaryoya göre değişebilecek bir yapıdır.Tabi en başta kuyrukların oluşmuş olması gerekiyor
            channel.BasicConsume(queueName, false, subscriber);

            Console.WriteLine("Loglar dinleniyor");

            //Eventi yakalıyoruz
            subscriber.Received += (object sender, BasicDeliverEventArgs e) =>
            {
                //Mesaj byte array olarak gegldiği için çevrim işlemi yapıyoruz
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                Console.WriteLine("Gelen Mesaj : " + message);

                //mesajın iletildikten sonra silinmemesini seçtiysek bu kısımda artık mesajımızı silebiliriz
                //e.delivertag mesaj bilgisini alıyor,ikinci parametre ise işlenmiş ama rabbitmq ya gitmemiş mesajlar da dahil edilsin mi diye soruyor
                channel.BasicAck(e.DeliveryTag, false);
            };


            Console.ReadLine();
        }
    }
}
