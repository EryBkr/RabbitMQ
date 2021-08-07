using DbToExcel.FileCreateWorkerService.Models;
using DbToExcel.FileCreateWorkerService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbToExcel.FileCreateWorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        //Buradaki Services ile DI yapýlabilir
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    //Settings dosyalarý için Configuration'a ihtiyacýmýz vardý
                    IConfiguration Configuration = hostContext.Configuration;

                    //DB context imi ve yolunu ekledim.Northwind veritabanýndan scaffholding yaptým
                    services.AddDbContext<NorthwindContext>(opt =>
                    {
                        opt.UseSqlServer(Configuration.GetConnectionString("SqlServer"));
                    });

                    //Servisimi DI ile oluþturuyorum
                    services.AddSingleton<RabbitMQClientService>();

                    //Connection Factory i baðlantý adresini vererek DI ile instance oluþturuyorum
                    //Worker service de bulunan appsettings.json a baðlantý adresimi girdim
                    services.AddSingleton(sp => new ConnectionFactory() { Uri = new Uri(Configuration.GetConnectionString("RabbitMQ")), DispatchConsumersAsync = true });

                    services.AddHostedService<Worker>();
                });
    }
}
