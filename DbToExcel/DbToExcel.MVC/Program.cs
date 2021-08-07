using DbToExcel.MVC.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbToExcel.MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            //Initilaize iþlemi için uyguladýk
            using (var scope = host.Services.CreateScope())
            {
                //GetReq Servis varlýðýndan emin isek
                var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                //Hali hazýrda migration varsa uygula
                appDbContext.Database.Migrate();

                //Kayýt yok ise
                if (!appDbContext.Users.Any())
                {
                    userManager.CreateAsync(new IdentityUser { UserName = "admin", Email = "admin@mail.com" }, "Admin123*").Wait();

                    userManager.CreateAsync(new IdentityUser { UserName = "user", Email = "user@mail.com" }, "User123*").Wait();
                }
            }
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
