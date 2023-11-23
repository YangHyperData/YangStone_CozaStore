using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CozaStore.Models
{
    public class UserOrder
    {
        public User user { get; set; }
        public Order order { get; set; }
    }
}