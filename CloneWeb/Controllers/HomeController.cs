using CloneWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ViewModel;
using static System.Net.WebRequestMethods;

namespace CloneWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        [Obsolete]
        private IHostingEnvironment _env;
        private IConfiguration _configuration;

        [Obsolete]
        public HomeController(IConfiguration configuration ,ILogger<HomeController> logger,IHostingEnvironment env)
        {
            _logger = logger;
            _env = env; 
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpPost]
        [Obsolete]
        public IActionResult UploadImage(List<IFormFile> files)
        {

            var filepath = "";
            foreach (var file in Request.Form.Files)
            {
                var fileName = DateTime.UtcNow.ToString("HH_mm_ss") + file.FileName ;
                string ServerMapPath = Path.Combine(_env.WebRootPath, "Upload/Post", fileName);
                using (var stream = new FileStream(ServerMapPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                filepath = _configuration["DomainUrl"] + "Upload/Post/" + fileName;
            }

            return Json(new { url = filepath });
        }
    }
}
