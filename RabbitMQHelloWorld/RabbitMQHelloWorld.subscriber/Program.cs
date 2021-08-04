using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMQHelloWorld.subscriber
{
    //Subscriber tarafı data alan taraftır
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

            //Mesaj alımı için kuyruk oluşturduk
            //Kuyruk daha önce oluşturulmuşsa hata vermez çalışmaya devam eder(parametreler aynı ise)
            //publisher ve subscriber tarafında oluşturulan kuyrukların özellikleri aynı olmalıdır
            //ilk parametre kuyruk adı,ikinci parametre kuyruk saklansın mı
            //üçünü parametre sadece bu kanal üzerinden mi kuyruğa bağlanılsın diye soruyor
            //dördüncü kuyruğa bağlı olan subscriber ların hepsi silinirse kuyruk silinsin mi,
            channel.QueueDeclare("hello-queue", true, false, false);

            //subscriber lara kaçar adet data göndereceğimizi belirliyoruz.
            //ilk parametrenin 0 olması herhangi bir boyutta ki mesajı alabiliriz demek
            //ikinci parametre kaçar adet mesaj gönderileceğini belirliyor
            //üçüncü parametre  true ise mesajlar 1 er 1er dağılatacak false ise toplamı 1 olarak şekilde dağıtılacak örneğin 5 değerini seçseydik hepsine teker teker vererek toplamı 5 yapmaya çalışacak sonra yine tur a devam edecekti.
            channel.BasicQos(0, 1, false);

            var subscriber = new EventingBasicConsumer(channel);

            //ilk parametreye kuyruk ismi tanımladık
            //ikinci parametre iletim sonrası mesaj silinsin mi diye soruyor
            //ikinci parametreye false verdiysek eğer ileride rabbitmq ya mesajın silinmesi için haber verebiliriz
            channel.BasicConsume("hello-queue", false, subscriber);

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
