using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CozaStore.Models;
using CozaStore.Areas.Dashboard.Models;
namespace CozaStore.Areas.Dashboard.Controllers
{
    public class AdminController : Controller
    {
        CozaStoreEntities db = new CozaStoreEntities();
        // GET: Dashboard/Admin
        public ActionResult Index()
        {
            var sum = db.Orders.Sum(n => n.TotalPrice);
            var cntorder = db.Orders.ToList();
            var cntuser = db.Users.ToList();
            ViewBag.sum = sum;
            ViewBag.cntorder = cntorder.Count();
            ViewBag.cntuser = cntuser.Count();
            return View();
        }
       [ChildActionOnly]
       public ActionResult ViewTopProduct()
        {
            var lstproduct = (from c in db.Products
                             select c).ToList().Take(6);

            List<Rankproduct> list = new List<Rankproduct>();
            foreach (var item in lstproduct)
            {
                int count = 0;
                var lstdetail = from c in db.DetailsOrders
                                where c.Productid == item.Productid
                                select c;
                if(lstdetail != null)
                count = lstdetail.Count();
                Rankproduct tmp = new Rankproduct { product = item, numproduct = count };
                list.Add(tmp);
            }
             
            return View(list.OrderByDescending(n=>n.numproduct).ToList());
        }
        [ChildActionOnly]
        public ActionResult chart()
        {
            List<decimal?> totalprice = new List<decimal?>();
            for(int i =1; i <= 12; i++)
            {
                var item = db.Orders.Where(n => n.OrderDay.Value.Month == i).Sum(c => c.TotalPrice);
                if (item != null)
                    totalprice.Add(item);
                else
                    totalprice.Add(0);
            }
                
            return View(totalprice);
        }
        [ChildActionOnly]
        public ActionResult ViewRankUser()
        {
            var rankUser = db.Users.ToList();
            List<RankUser> lstrank = new List<RankUser>();
            decimal? sum = 0;
            foreach(var item in rankUser)
            {
                decimal? sumitem = db.Orders.Where(n => n.Userid == item.Userid).Sum(n => n.TotalPrice);
                lstrank.Add(new RankUser
                {
                    user = item,
                    total = sumitem
                });
                sum += sumitem;
            }
            return View(lstrank.OrderByDescending(n => n.total).ToList());
        }
        [ChildActionOnly]
        public ActionResult Order()
        {
            var lstorder = (from c in db.Orders
                           join p in db.Users
                           on c.Userid equals p.Userid
                           
                           select new UserOrder
                           {
                               user = p,
                               order = c
                           }).OrderByDescending(n => n.order.OrderDay.Value.Year)
                             .ThenByDescending(n => n.order.OrderDay.Value.Month); ;
            return View(lstorder.ToList());
        }

        public ActionResult DetailsOrder(int id, int userid)
        {
            var lstdetail = from c in db.DetailsOrders
                            join p in db.Products
                            on c.Productid equals p.Productid
                            where c.Orderid == id
                            select new getproduct{ product = p,detailsOrder = c };
            ViewBag.user = db.Users.Where(n => n.Userid == userid).SingleOrDefault();
            ViewBag.oder = db.Orders.SingleOrDefault(n => n.Orderid == id);
            return View(lstdetail.ToList());
        }

        public ActionResult Accept(int id)
        {
            var order = db.Orders.SingleOrDefault(n => n.Orderid == id);
            order.Status = 1;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Cancel(int id)
        {
            var order = db.Orders.SingleOrDefault(n => n.Orderid == id);
            order.Status = 0;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult DeleteOrder(int id)
        {
            var order = db.Orders.SingleOrDefault(n => n.Orderid == id);
            var detailsorder = db.DetailsOrders.Where(n => n.Orderid == order.Orderid);
            db.DetailsOrders.RemoveRange(detailsorder);
            db.Orders.Remove(order);
            db.SaveChanges();
            return RedirectToAction("Index");

        }

        public ActionResult PrinfOrder(int id)
        {
            var order = db.Orders.SingleOrDefault(n => n.Orderid == id);
            var user = db.Users.SingleOrDefault(n => n.Userid == order.Userid);
            var lstdetails = db.DetailsOrders.Where(n => n.Orderid == id)
                            .Join(
                             db.Products,
                             p => p.Productid,
                             c => c.Productid,
                             (c,p) => new getproduct
                             {
                                 product = p,
                                 detailsOrder = c
                             }
                    );
            ViewBag.product = lstdetails.ToList();
            ViewBag.user = user;
            return View(order);
        }
    }
}