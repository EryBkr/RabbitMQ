using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Watermark.BackgroundServices;
using Web.Watermark.Models;
using Web.Watermark.Services;

namespace Web.Watermark
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Baðlantý bilgisini verdim DI içerisinde tanýmladým
            //Singleton olarak oluþturma nedenimiz sadece 1 adet instance oluþmasý içindir
            //Asenktron metot kullandýðýmdan dolayý bunu ikinci parametrede bildiriyorum
            services.AddSingleton(sp => new ConnectionFactory() { Uri = new Uri(Configuration.GetConnectionString("RabbitMQ")), DispatchConsumersAsync = true });

            //Servisimi de ekledi
            services.AddSingleton<RabbitMQClientService>();


            //Publisher DI ile oluþturulacak
            services.AddSingleton<RabbitMqPublisher>();

          
            services.AddDbContext<AppDbContext>(opt =>
            {
                //Sql Server kullanmadýk In Memory Kullandýk
                opt.UseInMemoryDatabase("productDb");
            });

            //Background servisimi ekledim
            services.AddHostedService<ImageWatermarkProcessBackgroundService>();
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
