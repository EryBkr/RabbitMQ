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


            //birden fazla instance olabileceği için rabbitmq tarafından bize sağlanan random kuyruk adımızı oluşturabiliriz.
            var randomQueueName = "log-database-save-queue"; /*channel.QueueDeclare().QueueName;*/


            //kuyruğun sürekli kalmasını istiyorsak exchange ye bağlanmadan önce declare etmemiz gerekiyor
            //ikinci parametre kaydedilsin mi
            //üçüncü parametre başka kanallardan kuyruğa bağlanılamaz mı diye soruyor
            //dördüncü parametre otomatik silinsin mi diye soruyor son subscriber silinse dahi kuyruk silinmeyecektir
            channel.QueueDeclare(randomQueueName, true, false, false);

            //publisher tarafında exchange oluşturmuştuk bu tarafta ise ona bir kuyruk bağlayarak gelen mesajları dinlemek istiyorum.kuyruk oluşturmak demek exchange yok olsa dahi kuyruk var olmaya devam eder ama kuyruk bind ederek exchange in işi bittiği zaman kuyruğun otomatik silinmesini sağlıyorum
            //ilk parametre kuyruk ismi
            //ikinci parametre exchange ismi
            channel.QueueBind(randomQueueName, "logs-fanout", "", null);

            //subscriber lara kaçar adet data göndereceğimizi belirliyoruz.
            //ilk parametrenin 0 olması herhangi bir boyutta ki mesajı alabiliriz demek
            //ikinci parametre kaçar adet mesaj gönderileceğini belirliyor
            //üçüncü parametre  true ise mesajlar 1 er 1er dağılatacak false ise toplamı 1 olarak şekilde dağıtılacak örneğin 5 değerini seçseydik hepsine teker teker vererek toplamı 5 yapmaya çalışacak sonra yine tur a devam edecekti.
            channel.BasicQos(0, 1, false);

            var subscriber = new EventingBasicConsumer(channel);

            //ilk parametreye kuyruk ismi tanımladık
            //ikinci parametre iletim sonrası mesaj silinsin mi diye soruyor
            //ikinci parametreye false verdiysek eğer ileride rabbitmq ya mesajın silinmesi için haber verebiliriz
            //kuyruk exchange yi dinlemeye başladıktan sonra publisher mesaj gönderdikçe alır tabi bu senaryo kuyruk kayıtlı değilse gerçekleşecektir kuyruk kaydedilecek şekilde oluşturulduysa subscriber kaldığı yerden ya da en başından dataları dinleyecektir.Senaryoya göre değişebilecek bir yapıdır.Tabi en başta kuyrukların oluşmuş olması gerekiyor
            channel.BasicConsume(randomQueueName, false, subscriber);

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
