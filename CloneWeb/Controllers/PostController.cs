using EntityDataModel.Data;
using EntityDataModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            Model.Url = ToUrlSlug(Model.Title);
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
        [Route("{Year}/{Month}/{Date}/{Url}/{PostId}")]
        public async Task<IActionResult> ViewPost(Guid? PostId)
        {
            var post = await (from db in _context.Post.Include(x => x.PostTag)
                              join cate in _context.Category on db.CategoryId equals cate.CategoryId
                              //where db.PostId == PostId
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
                                  PostTag = db.PostTag.ToList(),
                                  PostInfomation = db.PostInfomation,
                                  Url = db.Url,
                                  Tags = (from db in db.PostTag.ToList()
                                         join tag in _context.Tag on db.TagId equals tag.TagId
                                         select tag).ToList(),
                              }
                            ).FirstOrDefaultAsync();
            if (post == null) return NotFound();
                    
            post.TotalComments = _context.PostComment.Where(x=>x.PostId == post.PostId).ToList().Count;
            ViewBag.PostId = post.PostId;
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
                                  PostTag = db.PostTag.ToList(),
                                  PostInfomation = db.PostInfomation,
                                  TotalComments = db.PostComment.Count(),
                                  Tags = (from db in db.PostTag.ToList()
                                          join tag in _context.Tag on db.TagId equals tag.TagId
                                          select tag).ToList(),
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
        public static string ToUrlSlug(string value)
        {

            //First to lower case
            value = value.ToLowerInvariant();

            //Remove all accents
            var bytes = Encoding.GetEncoding("Cyrillic").GetBytes(value);
            value = Encoding.ASCII.GetString(bytes);

            //Replace spaces
            value = Regex.Replace(value, @"\s", "-", RegexOptions.Compiled);

            //Remove invalid chars
            value = Regex.Replace(value, @"[^a-z0-9\s-_]", "", RegexOptions.Compiled);

            //Trim dashes from end
            value = value.Trim('-', '_');

            //Replace double occurences of - or _
            value = Regex.Replace(value, @"([-_]){2,}", "$1", RegexOptions.Compiled);

            return value;
        }
    }
}
