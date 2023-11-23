using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CozaStore.Models
{
    public class Usercomment
    {
        public User user { get; set; }
        public Comment comment { set; get; }
    }
}