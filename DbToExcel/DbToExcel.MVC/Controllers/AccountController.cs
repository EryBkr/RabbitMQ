using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbToExcel.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signinManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signinManager)
        {
            _userManager = userManager;
            _signinManager = signinManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email,string password)
        {
            var hasUser = await _userManager.FindByEmailAsync(email);

            //Kullanıcı bulunamadı
            if (hasUser==null)
            {
                return View();
            }

            //Giriş kontrolü
            var signInResult = await _signinManager.PasswordSignInAsync(hasUser, password,true,false);

            //Giriş Başarısız
            if (!signInResult.Succeeded)
            {
                return View();
            }


            return RedirectToAction("Index","Home");
        }
    }
}
