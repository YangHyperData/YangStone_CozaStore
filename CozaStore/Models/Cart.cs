using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CozaStore.Models;
namespace CozaStore.Models
{
    public class Cart
    {
        CozaStoreEntities db = new CozaStoreEntities();
        public int Productid { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public decimal ProductPrice { get; set; }
        public int ProductNumber { get; set; }
        public int Sizeid { get; set; }
        public int Colorid { get; set; }
        public decimal TotalProductPrice
        {
            get { return ProductNumber * ProductPrice; }
        }
        public Cart(int Productid, int sizeid, int colorid, int numproduct)
        {
            this.Productid = Productid;
            Product s = db.Products.SingleOrDefault(n => n.Productid == this.Productid);
            this.ProductName = s.Name;
            this.ProductImage = s.Illsutration;
            this.ProductPrice = decimal.Parse(s.Price.ToString());
            this.ProductNumber = 1;
            this.Colorid = colorid;
            this.Sizeid = sizeid;
            this.ProductNumber = numproduct;
        }

    }
}