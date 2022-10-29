using EntityDataModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Create
{
    public class PostViewModel : Post
    {
        public List<Guid?> TagId { get; set; }
    }
}
