using EntityDataModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.ResultViewModel
{
    public class PostResultViewModel
    {
        public Guid PostId { get; set; }
        public string Title { get; set; }
        public Guid? CreateBy { get; set; }
        public Guid? CommentId { get; set; }
        public string CreateByName { get; set; }
        public DateTime? CreateTime { get; set; }
        public Guid? LastEditBy { get; set; }
        public string LastEditByName { get; set; }
        public DateTime? LastEditTime { get; set; }
        public  string CategoryName { get; set; }
        public  string PostInfomation { get; set; }
        public int TotalComments { get; set; }
        public List<Tag> PostTag { get; set; }
        public List<Comments> PostComment { get; set; }

    }
}
