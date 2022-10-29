using EntityDataModel.Data;
using EntityDataModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NuGet.Protocol.Core.Types;
using System;
using System.ComponentModel.Design;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ViewModel.Create;
using ViewModel.ResultViewModel;

namespace CloneWeb.Controllers
{
    public class PostController : Controller
    {
        private EntityDataContext _context;
        private IConfiguration _configuration;
        public PostController(EntityDataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CreatePost()
        {
            var lstCategory = _context.Category.ToList();
            ViewBag.CategoryId = new SelectList(lstCategory, "CategoryId", "Title");
            var lstTag = _context.Tag.ToList();
            ViewBag.TagId = new SelectList(lstTag, "TagId", "Title");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreatePost(PostViewModel Model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Not a valid model");
            Model.PostId = Guid.NewGuid();
            Model.CreateTime = DateTime.Now;
            Model.CreateBy = Guid.Parse("14A16D89-0ABD-4235-B155-3C6977CE0A6F");

            if (Model.PostImageUrl != null)
            {

            }
            if (Model.TagId != null)
            {
                if (Model.TagId.Count > 0)
                {
                    foreach (var item in Model.TagId)
                    {
                        _context.Add(new PostTag { PostId = Model.PostId, TagId = (Guid)item });
                    }
                }
            }
            _context.Post.Add(Model);
            _context.SaveChanges();

            return RedirectToAction("ListPost");
        }

        public async Task<IActionResult> ListPost()
        {
            var lstPost = await _context.Post.ToListAsync();
            return View();
        }
        public async Task<IActionResult> ViewPost(Guid PostId)
        {
            var post = await (from db in _context.Post.Include(x => x.PostComment).Include(x => x.PostTag)
                              join cate in _context.Category on db.CategoryId equals cate.CategoryId
                              where db.PostId == PostId
                              select new PostResultViewModel
                              {
                                  PostId = db.PostId,
                                  Title = db.Title,
                                  CreateBy = db.CreateBy,
                                  CreateByName = _context.User.Where(x => x.UserId == db.CreateBy).FirstOrDefault().UserName,
                                  CreateTime = db.CreateTime,
                                  LastEditBy = db.LastEditBy,
                                  LastEditByName = _context.User.Where(x => x.UserId == db.LastEditBy).FirstOrDefault().UserName,
                                  LastEditTime = db.LastEditTime,
                                  CategoryName = cate.Title,
                                  PostComment = _context.Comments.Where(x => x.CommentId == db.PostComment.FirstOrDefault().CommentId).ToList(),
                                  PostTag = _context.Tag.Where(x => x.TagId == db.PostTag.FirstOrDefault().TagId).ToList(),
                                  PostInfomation = db.PostInfomation
                              }
                            ).FirstOrDefaultAsync();

            if (post == null) return NotFound();

            post.TotalComments = post.PostComment.Count();
            ViewBag.PostId = PostId;
            ViewBag.PostInfomation = post.PostInfomation;

            return View(post);
        }
        public async Task<IActionResult> SearchPost(string KeyWord)
        {
            ViewBag.DomainUrl = _configuration["DomainUrl"];
            KeyWord = KeyWord.Trim();
            var post = await (from db in _context.Post.Include(x => x.PostComment).Include(x => x.PostTag)
                              join c in _context.Category on db.CategoryId equals c.CategoryId
                              where (db.Title.Contains(KeyWord))
                             || c.Title.Contains(KeyWord)
                              select new PostResultViewModel
                              {
                                  PostId = db.PostId,
                                  Title = db.Title,
                                  CreateBy = db.CreateBy,
                                  CreateByName = _context.User.Where(x => x.UserId == db.CreateBy).FirstOrDefault().UserName,
                                  CreateTime = db.CreateTime,
                                  LastEditBy = db.LastEditBy,
                                  LastEditByName = _context.User.Where(x => x.UserId == db.LastEditBy).FirstOrDefault().UserName,
                                  LastEditTime = db.LastEditTime,
                                  CategoryName = c.Title,
                                  PostComment = _context.Comments.Where(x => x.CommentId == db.PostComment.FirstOrDefault().CommentId).ToList(),
                                  PostTag = _context.Tag.Where(x => x.TagId == db.PostTag.FirstOrDefault().TagId).ToList(),
                                  PostInfomation = db.PostInfomation,
                                  TotalComments = db.PostComment.Count()
                              }).ToListAsync();

            return View(post);
        }
        public JsonResult AddComment(Guid PostId, string Comment)
        {
            try
            {
                var comments = new Comments();
                comments.CommentId = Guid.NewGuid();
                comments.CreateTime = DateTime.Now;
                comments.CommentMessage = Comment;
                comments.CreateBy = Guid.Parse("14A16D89-0ABD-4235-B155-3C6977CE0A6F");

                var PostCmt = new PostComment();
                PostCmt.PostId = PostId;
                PostCmt.CommentId = comments.CommentId;

                _context.Comments.Add(comments);
                _context.PostComment.Add(PostCmt);
                _context.SaveChanges();
                return Json(Ok(new Reponse { isSuccess = true, code = 200 }
                          ));
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IActionResult ReloadComment(Guid? PostId)
        {
            return ViewComponent("ListComments", new { PostId = PostId });
        }
        public IActionResult ReloadRecentComment(Guid? PostId)
        {
            return ViewComponent("ListRecentComment");
        }
    }
}
