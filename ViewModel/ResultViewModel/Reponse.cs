using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.ResultViewModel
{
    public class Reponse
    {
        public int code { get; set; }
        public bool isSuccess { get; set; }
        public string message { get; set; }
        public object data { get; set; }
        public int recordsFiltered { get; set; }
    }
}
