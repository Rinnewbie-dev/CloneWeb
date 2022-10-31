using EntityDataModel.Data;
using EntityDataModel.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using ViewModel;
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
        [Authorize]
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
        [Authorize]
        public IActionResult CreatePost(PostViewModel Model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Not a valid model");
            var claimns = User?.Identities.First().Claims.ToList();
            Model.PostId = Guid.NewGuid();
            Model.CreateTime = DateTime.Now;
            Model.CreateBy = Guid.Parse(claimns.Where(x => x.Type == "UserId").FirstOrDefault().Value.ToString());

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

            post.TotalComments = _context.PostComment.Where(x=>x.PostId == post.PostId).ToList().Count;
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
        [Authorize]
        public IActionResult AddComment(Guid PostId, string Comment)
        {
            try
            {
                var claimns = User?.Identities.First().Claims.ToList();

                var comments = new Comments();
                comments.CommentId = Guid.NewGuid();
                comments.CreateTime = DateTime.Now;
                comments.CommentMessage = Comment;
                comments.CreateBy = Guid.Parse(claimns.Where(x=>x.Type == "UserId").FirstOrDefault().Value.ToString());
                
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
