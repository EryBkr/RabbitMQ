using DbToExcel.MVC.Hubs;
using DbToExcel.MVC.Models;
using DbToExcel.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DbToExcel.MVC.Controllers
{
    //Worker servis Excel dosyasını oluşturduktan sonra bu endpoint e istek atarak dosyamın web projeme kayıt edilmesini sağlayacaktır
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        //Socket kütüphanemi kullanmak için DI ile oluşturuyorum
        private readonly IHubContext<MyHub> _hubcontext;

        public FilesController(AppDbContext context, IHubContext<MyHub> hubcontext, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _hubcontext = hubcontext;
            _userManager = userManager;
        }

        //Dosyanın kendisini  ve hangi dosyanın excel e çevrildiğini buraya göndereceğim (Workerservice rabbitmq aracılığıyla gönderecek)
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file, int fileId)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            //Dosya boş ise BadRequest  dönüyorum
            if (file is not { Length: > 0 }) return BadRequest();

            //Dosya bilgilerini aldık
            var userFile = await _context.UserFiles.FirstAsync(i => i.Id == fileId);

            //Dosya adı + uzantısı alındı
            var filePath = userFile.FileName + Path.GetExtension(file.FileName);

            //Dosyanın yolu oluşturuldu
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files", filePath);

            //Dosya verdiğim yola kayıtedildi
            using FileStream strem = new(path, FileMode.Create);
            await file.CopyToAsync(strem);

            //Kaydın sütunlarını güncelliyorum
            userFile.CreatedDate = DateTime.Now;
            userFile.FilePath = filePath;
            userFile.FileStatus = FileStatus.Completed;

            //Son güncellemeleri kayıt ediyorum
            await _context.SaveChangesAsync();

            //Kişiye özel tamamlandı bildirimini gönderiyorum
            await _hubcontext.Clients.User(user.UserName).SendAsync("CompletedFile");

            return Ok();
        }


    }
}
