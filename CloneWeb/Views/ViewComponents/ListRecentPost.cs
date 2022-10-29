using EntityDataModel.Data;
using EntityDataModel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloneWeb.Views.ViewComponent
{
    public class ListRecentPostViewComponent : Microsoft.AspNetCore.Mvc.ViewComponent
    {
        private readonly EntityDataContext _context;
        private readonly IConfiguration _configuration;
        public ListRecentPostViewComponent(EntityDataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var lstRecentPost = await GetRecentPost();
            return View(lstRecentPost);
        }
        private async Task<List<Post>> GetRecentPost()
        {
            //top 10 post mới nhất
            ViewBag.DomainUrl = _configuration["DomainUrl"];
            var data = await _context.Post.OrderByDescending(x => x.LastEditTime).ThenByDescending(x => x.CreateTime).Take(5).ToListAsync();
            return data;
        }
    }

}
