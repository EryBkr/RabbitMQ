using ClosedXML.Excel;
using DbToExcel.FileCreateWorkerService.Models;
using DbToExcel.FileCreateWorkerService.Services;
using DbToExcel.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DbToExcel.FileCreateWorkerService
{
    //Worker service geliþmiþ bir konsol uygulamasý gibidir.DI Log gibi eklentilerle gelmektedir.
    //Verilen iþleri yapmak ve iletmekle görevilidr herhangi bir UI kavramý yoktur
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly RabbitMQClientService _service;
        private IModel _channel;

        //Worker Servis Singleton,DbContext Scope olduðu için DI ile direkt alamýyorum
        private readonly IServiceProvider _serviceProvider;

        public Worker(ILogger<Worker> logger, RabbitMQClientService service, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _service = service;
            _serviceProvider = serviceProvider;
        }

        //Worker baþladýðý zaman
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            //Baðlantýyý kuruyorum
            _channel = _service.Connect();
            _channel.BasicQos(0, 1, false);
            _logger.LogInformation("Baðlantý baþlatýldý");
            return base.StartAsync(cancellationToken);
        }

        //Direkt olarak çalýþacak metodum
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var subscriber = new AsyncEventingBasicConsumer(_channel);

            //Dinlenecek kuyruðu belirledik
            _channel.BasicConsume(RabbitMQClientService.QueueName, false, subscriber);

            subscriber.Received += Subscriber_Received;

            _logger.LogInformation("Ýþlem tamamlandý");

            return Task.CompletedTask;
        }


        private async Task Subscriber_Received(object sender, BasicDeliverEventArgs @event)
        {
            //Test amaçlý geçikme verdim
            await Task.Delay(5000);

            //Kuyruða gönderdiðim datayý worker serviste alýyorum
            var excelMessage = JsonSerializer.Deserialize<CreateExcelMessage>(Encoding.UTF8.GetString(@event.Body.ToArray()));

            using var memoryStream = new MemoryStream();

            //Excel oluþturma iþlemleri
            var workGroup = new XLWorkbook();
            var dataSet = new DataSet();

            //Datalarýmý Data Set e ekledik
            dataSet.Tables.Add(GetTable("Products"));
            workGroup.Worksheets.Add(dataSet);

            //Excel dosyasýný ram e kaydettik
            workGroup.SaveAs(memoryStream);

            //Api tarafýnda ki IFormFile a göndermeye hazýr hale getiriyorum
            MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
            multipartFormDataContent.Add(new ByteArrayContent(memoryStream.ToArray()), "file", Guid.NewGuid().ToString() + ".xlsx");

            //Endpoint imi belirledim.
            var baseUrl = "https://localhost:44307/api/Files";


            using (var httpClient = new HttpClient())
            {
                //Endpoint e gerekli datalarý gönderiyorum
                var response = await httpClient.PostAsync($"{baseUrl}?fileId={excelMessage.FileId}", multipartFormDataContent);

                if (response.IsSuccessStatusCode)
                {
                    //Gönderme iþlemi baþarýlý mesajýmý silebilirim
                    _channel.BasicAck(@event.DeliveryTag, false);
                }
            }
        }



        //Datatable tipinde tabloda ki datalarý alýyoruz
        private DataTable GetTable(string tableName)
        {
            List<Product> products;

            //DI ile oluþmadýðý bu þekilde context e eriþtik
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<NorthwindContext>();

                products = context.Products.ToList();
            }

            DataTable table = new DataTable { TableName = tableName };
            table.Columns.Add("ProductId", typeof(int));
            table.Columns.Add("ProductName", typeof(string));
            table.Columns.Add("Discontinued", typeof(bool));

            products.ForEach(product =>
            {
                table.Rows.Add(product.ProductId, product.ProductName, product.Discontinued);
            });

            return table;
        }

    }
}
