using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Web.Watermark.Services;

namespace Web.Watermark.BackgroundServices
{
    //Arka planda otomatik çalışacak servisimi oluşturdum
    //Tabii ki bu arkadaşı background servise ekliyorum
    public class ImageWatermarkProcessBackgroundService : BackgroundService
    {
        //Servisimi DI ile oluşturuyorum
        private readonly RabbitMQClientService _service;
        private IModel _channel;

        public ImageWatermarkProcessBackgroundService(RabbitMQClientService service)
        {
            _service = service;
        }

        //Uygulama ayağa kalktığında
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            //Kanalı oluşturdum
            _channel = _service.Connect();
            //1 er 1 er datayı alıyorum
            _channel.BasicQos(0, 1, false);


            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var subscriber = new AsyncEventingBasicConsumer(_channel);
            _channel.BasicConsume(RabbitMQClientService.QueueName, false, subscriber);

            //Eventi dinliyorum yukarıda ayarları tanımlamıştım
            //Eventi ekledim
            subscriber.Received += Subscriber_Received;

            return Task.CompletedTask;
        }

        //Gelen datayı alma ve işleme adımları
        private Task Subscriber_Received(object sender, BasicDeliverEventArgs @event)
        {
            //Test amaçlı bekleme süresini 30sn olarak ayarladım.Normalde bir web sayfasının 2 snde açılması gerekiyor.Kuyruk sistemi kullandığımız için sayfamız hiçbir takılma yaşamadan devam edecektir.
             Task.Delay(30000).Wait();

            try
            {
                //Kayıtlı imaj ismini aldım
                var imageCreatedEvent = JsonSerializer.Deserialize<ProductImageCreatedEvent>(Encoding.UTF8.GetString(@event.Body.ToArray()));

                //Resmin yoluna ulaştık
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", imageCreatedEvent.ImageName);

                //Aldığımız Resmin sağ altköşesine yazı yazıyoruz
                using var img = Image.FromFile(path);
                using var graphic = Graphics.FromImage(img);
                var font = new Font(FontFamily.GenericMonospace, 32, FontStyle.Bold, GraphicsUnit.Pixel);
                var textSize = graphic.MeasureString("www.eraybakir.com", font);
                var color = Color.FromArgb(128, 255, 255, 255);
                var brush = new SolidBrush(color);
                var position = new Point(img.Width - ((int)textSize.Width + 30), img.Height - ((int)textSize.Height + 30));
                //Çizme işlemi tamamlandı
                graphic.DrawString("www.eraybakir.com", font, brush, position);
                //oluşturduğum resmi kaydediyorum
                img.Save("wwwroot/images/watermarks/" + imageCreatedEvent.ImageName);

                //Bellekten düşüyorum
                img.Dispose();
                graphic.Dispose();

                //İşlem başarılı , kuyruktan silebiliriz artık
                _channel.BasicAck(@event.DeliveryTag,false);
            }
            catch (Exception)
            {
                throw;
            }

            return Task.CompletedTask;

        }
    }
}
