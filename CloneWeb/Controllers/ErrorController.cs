using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using System.Net;
using ViewModel;

namespace CloneWeb.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index(int? statusCode)
        {

            if (statusCode == (int)HttpStatusCode.NotFound || statusCode == (int)HttpStatusCode.InternalServerError)
                return View();

            if (statusCode == (int)HttpStatusCode.Unauthorized || statusCode == (int)HttpStatusCode.Forbidden)
                return Redirect("/Authentication/Login");
            
            return View();
        }
    }
}
