using EntityDataModel.Data;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CloneWeb.Views.ViewComponents
{
    public class TotalVisitViewComponent : Microsoft.AspNetCore.Mvc.ViewComponent
    {
        private readonly EntityDataContext _context;
        public TotalVisitViewComponent(EntityDataContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var total = await GetTotalVist();
            return View(total);
        }
        public async Task<int> GetTotalVist()
        {
            return 122146164;
        }

    }
}
