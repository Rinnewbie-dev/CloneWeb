using EntityDataModel.Data;
using EntityDataModel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloneWeb.Views.ViewComponents
{
    public class ListCategoryViewComponent : Microsoft.AspNetCore.Mvc.ViewComponent
    {
        private readonly EntityDataContext _context;
        public ListCategoryViewComponent(EntityDataContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var lstRecentPost = await GetCategory();
            return View(lstRecentPost);
        }
        private async Task<List<Category>> GetCategory()
        {
            //top 10 post mới nhất
            var data = await _context.Category.ToListAsync();
            return data;
        }
    }
}
