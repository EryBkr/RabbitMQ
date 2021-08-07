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
    //Worker service geli�mi� bir konsol uygulamas� gibidir.DI Log gibi eklentilerle gelmektedir.
    //Verilen i�leri yapmak ve iletmekle g�revilidr herhangi bir UI kavram� yoktur
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly RabbitMQClientService _service;
        private IModel _channel;

        //Worker Servis Singleton,DbContext Scope oldu�u i�in DI ile direkt alam�yorum
        private readonly IServiceProvider _serviceProvider;

        public Worker(ILogger<Worker> logger, RabbitMQClientService service, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _service = service;
            _serviceProvider = serviceProvider;
        }

        //Worker ba�lad��� zaman
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            //Ba�lant�y� kuruyorum
            _channel = _service.Connect();
            _channel.BasicQos(0, 1, false);
            _logger.LogInformation("Ba�lant� ba�lat�ld�");
            return base.StartAsync(cancellationToken);
        }

        //Direkt olarak �al��acak metodum
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var subscriber = new AsyncEventingBasicConsumer(_channel);

            //Dinlenecek kuyru�u belirledik
            _channel.BasicConsume(RabbitMQClientService.QueueName, false, subscriber);

            subscriber.Received += Subscriber_Received;

            _logger.LogInformation("��lem tamamland�");

            return Task.CompletedTask;
        }


        private async Task Subscriber_Received(object sender, BasicDeliverEventArgs @event)
        {
            //Test ama�l� ge�ikme verdim
            await Task.Delay(5000);

            //Kuyru�a g�nderdi�im datay� worker serviste al�yorum
            var excelMessage = JsonSerializer.Deserialize<CreateExcelMessage>(Encoding.UTF8.GetString(@event.Body.ToArray()));

            using var memoryStream = new MemoryStream();

            //Excel olu�turma i�lemleri
            var workGroup = new XLWorkbook();
            var dataSet = new DataSet();

            //Datalar�m� Data Set e ekledik
            dataSet.Tables.Add(GetTable("Products"));
            workGroup.Worksheets.Add(dataSet);

            //Excel dosyas�n� ram e kaydettik
            workGroup.SaveAs(memoryStream);

            //Api taraf�nda ki IFormFile a g�ndermeye haz�r hale getiriyorum
            MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
            multipartFormDataContent.Add(new ByteArrayContent(memoryStream.ToArray()), "file", Guid.NewGuid().ToString() + ".xlsx");

            //Endpoint imi belirledim.
            var baseUrl = "https://localhost:44307/api/Files";


            using (var httpClient = new HttpClient())
            {
                //Endpoint e gerekli datalar� g�nderiyorum
                var response = await httpClient.PostAsync($"{baseUrl}?fileId={excelMessage.FileId}", multipartFormDataContent);

                if (response.IsSuccessStatusCode)
                {
                    //G�nderme i�lemi ba�ar�l� mesaj�m� silebilirim
                    _channel.BasicAck(@event.DeliveryTag, false);
                }
            }
        }



        //Datatable tipinde tabloda ki datalar� al�yoruz
        private DataTable GetTable(string tableName)
        {
            List<Product> products;

            //DI ile olu�mad��� bu �ekilde context e eri�tik
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
