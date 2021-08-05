using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Web.Watermark.Models;

namespace Web.Watermark.Controllers
{
    public class HomeController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }

        
     
    }
}
