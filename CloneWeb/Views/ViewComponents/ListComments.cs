using EntityDataModel.Data;
using EntityDataModel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ViewModel.ResultViewModel;

namespace CloneWeb.Views.ViewComponents
{
    public class ListCommentsViewComponent : Microsoft.AspNetCore.Mvc.ViewComponent
    {
        private readonly EntityDataContext _context;
        public ListCommentsViewComponent(EntityDataContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(Guid? PostId)
        {
            var lstRecentPost = await GetListComment(PostId);
            return View(lstRecentPost);
        }
        private async Task<List<CommentResultViewModel>> GetListComment(Guid? PostId)
        {
            //top 10 post mới nhất
            var data = await _context.Post.Where(x => x.PostId == PostId).Include(x => x.PostComment).FirstOrDefaultAsync();

            var result = (from db in data.PostComment
                          join c in _context.Comments on db.CommentId equals c.CommentId
                          join u in _context.User on c.CreateBy equals u.UserId
                          select new CommentResultViewModel
                          {
                              UserName = u.UserName,
                              CreateTime = c.CreateTime,
                              CommentMessage = c.CommentMessage,
                              AvatarUrl = u.ImageUrl,
                          }).OrderByDescending(x=>x.CreateTime).ToList();
            return result;
        }
    }
}
