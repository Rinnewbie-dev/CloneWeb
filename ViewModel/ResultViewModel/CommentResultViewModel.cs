using EntityDataModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.ResultViewModel
{
    public class CommentResultViewModel
    {
        public string UserName { get; set; }
        public DateTime? CreateTime { get; set; }
        public string CommentMessage { get; set; }
        public string AvatarUrl { get; set; }
        public DateTime? LastEditTime { get; set; }

    }
}
