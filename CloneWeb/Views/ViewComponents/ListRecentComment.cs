using EntityDataModel.Data;
using EntityDataModel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ViewModel.ResultViewModel;

namespace CloneWeb.Views.ViewComponents
{
    public class ListRecentCommentViewComponent : Microsoft.AspNetCore.Mvc.ViewComponent
    {
        private readonly EntityDataContext _context;
        public ListRecentCommentViewComponent(EntityDataContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var lstRecentPost = await GetRecentComment();
            return View(lstRecentPost);
        }
        private Task<List<CommentResultViewModel>> GetRecentComment()
        {

            var result = (from db in _context.Comments
                          join u in _context.User on db.CreateBy equals u.UserId
                          select new CommentResultViewModel
                          {
                              UserName = u.UserName,
                              CreateTime = db.CreateTime,
                              CommentMessage = db.CommentMessage,
                              AvatarUrl = u.ImageUrl,

                          }).OrderBy(x=>x.CreateTime).Take(3).ToList();
            return Task.FromResult(result);
        }
    }
}
