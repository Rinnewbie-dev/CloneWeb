using EntityDataModel.Data;
using System.Web.Mvc;

namespace Core
{
    public class BaseController : Controller
    {
        public EntityDataContext _context;

        public BaseController()
        {
            _context = new EntityDataContext();
        }
    }
}
