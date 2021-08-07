using DbToExcel.MVC.Models;
using DbToExcel.MVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbToExcel.MVC.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppDbContext _context;
        private readonly RabbitMQPublisher _publisher;

        public ProductController(UserManager<IdentityUser> userManager, AppDbContext context, RabbitMQPublisher publisher)
        {
            _userManager = userManager;
            _context = context;
            _publisher = publisher;
        }

        public IActionResult Index()
        {
            return View();
        }

        //Kişinin Excel Dosyasının adını oluşturuyor
        public async Task<IActionResult> CreateProductExcel()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var fileName = $"product-excel-{Guid.NewGuid().ToString().Substring(1, 10)}";
            UserFile userFile = new()
            {
                UserId = user.Id,
                FileName = fileName,
                FileStatus = FileStatus.Creating
            };

            TempData["StartCreatingExcel"] = true;

            await _context.UserFiles.AddAsync(userFile);
            await _context.SaveChangesAsync();

            //Savechanges çalıştığı için Id yi alabiliyoruz.Sonuçta referans tip
            //Publisher ile kuyruğa gönderiyoruz
            _publisher.Publish(new Shared.CreateExcelMessage { FileId = userFile.Id});

            return RedirectToAction(nameof(Files));
        }

        //Dosyaların adlarını ve indirilebilirlik durumlarını görebiliyoruz
        public async Task<IActionResult> Files()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);


            return View(await _context.UserFiles.Where(i => i.UserId == user.Id).ToListAsync());
        }
    }
}
