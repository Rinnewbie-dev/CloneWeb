using EntityDataModel.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace CloneWeb.Controllers
{
    public class AboutController : Controller
    {
        private readonly EntityDataContext _context;
        public AboutController(EntityDataContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            //ViewBag.Comments = _context.Post.Where(x=>x.PostId == Id).Count();
            return View();
        }
    }
}
