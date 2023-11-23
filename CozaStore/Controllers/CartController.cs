
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CozaStore.Models;
using CozaStore.Others;
namespace CozaStore.Controllers
{
    public class CartController : Controller
    {
        CozaStoreEntities db = new CozaStoreEntities();
        public List<Cart> GetCart()
        {
            List<Cart> lstCart = Session["Cart"] as List<Cart>;
            if(lstCart == null)
            {
                lstCart = new List<Cart>();
                Session["Cart"] = lstCart;
            }
            return lstCart;
        }

        public ActionResult AddCart(int productid, string url , FormCollection f)
        {
            List<Cart> lstCart = GetCart();
            Cart product = lstCart.Find(n => n.Productid == productid);
            int sizeid = int.Parse(f["Type-Size"].ToString());
            int colorid = int.Parse(f["Type-Color"].ToString());
            int numproduct = int.Parse(f["num-product"]);
            if(product == null)
            {
                product = new Cart(productid,sizeid,colorid,numproduct);
                lstCart.Add(product);
            }
            else
            {
                product.ProductNumber += numproduct;
            }
            return Redirect(url);
        }

        private int TotalNumberProduct()
        {
            int totalNumberProduct = 0;
            List<Cart> lstCart = Session["Cart"] as List<Cart>;
            if(lstCart != null)
            {
                totalNumberProduct = lstCart.Sum(n => n.ProductNumber);
            }
            return totalNumberProduct;
        }
        // GET: Cart
        private decimal TotalPrice()
        {
            decimal TotalPrice = 0;
            List<Cart> lstCart = Session["Cart"] as List<Cart>;
            if(lstCart != null)
            {
                TotalPrice = lstCart.Sum(n => n.TotalProductPrice);
            }
            return TotalPrice;
        }
        List<Color> FullColor()
        {
            return db.Colors.ToList();
        }
         List<Size> FullSize()
        {
            return db.Sizes.ToList();
        }

        public ActionResult Cart()
        {
            List<Cart> lstCart = GetCart();
            if(lstCart.Count == 0)
            {
                return RedirectToAction("Index", "CozaHome");
            }
            ViewBag.Number = TotalNumberProduct();
            ViewBag.TotalPrice = TotalPrice();
            ViewBag.Size = FullSize();
            ViewBag.Color = FullColor();
            return View(lstCart);
        }

        public ActionResult DeleteProduct(int productid)
        {
            List<Cart> lstCart = GetCart();
            Cart product = lstCart.SingleOrDefault(n => n.Productid == productid);
            if(product != null)
            {
                lstCart.RemoveAll(n => n.Productid == productid);
                if(lstCart.Count == 0)
                {
                    return RedirectToAction("Index", "CozaHome");
                }
            }
            return RedirectToAction("Cart");
        }

        public ActionResult UpdateCart(int productid, FormCollection f)
        {
            List<Cart> lstcart = GetCart();
            if(lstcart == null)
            {
                return RedirectToAction("Cart", "Cart");
            }
            Cart product = lstcart.SingleOrDefault(n => n.Productid == productid);
            if(product == null)
            {
                return RedirectToAction("Cart", "Cart");
            }
            int num = int.Parse(f["num-product"]);
            product.ProductNumber = num;
            return RedirectToAction("Cart", "Cart");
        }
        public ActionResult ClearCart()
        {
            List<Cart> lstCart = GetCart();
            lstCart.Clear();
            return RedirectToAction("Index", "CozaHome");

        }
        [ChildActionOnly]
        public ActionResult ViewCard()
        {
            ViewBag.Number = TotalNumberProduct();
            return PartialView();
        }
        public ActionResult ShoppingCard()
        {
            if(Session["User"] == null)
            {
                return RedirectToAction("Login","Login");
            }
            List<Cart> lstcart = GetCart(); 
            ViewBag.Number = TotalNumberProduct();
            ViewBag.TotalPrice = TotalPrice();
            ViewBag.Size = FullSize();
            ViewBag.Color = FullColor();
            return View(lstcart);
        }

        public ActionResult Payment(int TotalPrice)
        {
            string url = ConfigurationManager.AppSettings["Url"];
            string returnUrl = ConfigurationManager.AppSettings["ReturnUrl"];
            string tmnCode = ConfigurationManager.AppSettings["TmnCode"];
            string hashSecret = ConfigurationManager.AppSettings["HashSecret"];

            PayLib pay = new PayLib();
            
            pay.AddRequestData("vnp_Version", "2.1.0"); //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.1.0
            pay.AddRequestData("vnp_Command", "pay"); //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
            pay.AddRequestData("vnp_TmnCode", tmnCode); //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
            pay.AddRequestData("vnp_Amount", (TotalPrice * 100).ToString()); //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 1000000
            pay.AddRequestData("vnp_BankCode", ""); //Mã Ngân hàng thanh toán (tham khảo: https://sandbox.vnpayment.vn/apis/danh-sach-ngan-hang/), có thể để trống, người dùng có thể chọn trên cổng thanh toán VNPAY
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")); //ngày thanh toán theo định dạng yyyyMMddHHmmss
            pay.AddRequestData("vnp_CurrCode", "VND"); //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
            pay.AddRequestData("vnp_IpAddr", Util.GetIpAddress()); //Địa chỉ IP của khách hàng thực hiện giao dịch
            pay.AddRequestData("vnp_Locale", "vn"); //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
            pay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang"); //Thông tin mô tả nội dung thanh toán
            pay.AddRequestData("vnp_OrderType", "other"); //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
            pay.AddRequestData("vnp_ReturnUrl", returnUrl); //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
            pay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString()); //mã hóa đơn

            string paymentUrl = pay.CreateRequestUrl(url, hashSecret);

            return Redirect(paymentUrl);
        }

        public ActionResult PaymentConfirm()
        {
            if (Request.QueryString.Count > 0)
            {
                string hashSecret = ConfigurationManager.AppSettings["HashSecret"]; //Chuỗi bí mật
                var vnpayData = Request.QueryString;
                PayLib pay = new PayLib();
                //create bill
                Order bill = new Order();
                User user = Session["User"] as User;
                bill.Userid = user.Userid;
                bill.TotalPrice = TotalPrice();
                bill.Statuspay = 1;
                bill.Status = 0;
                bill.OrderDay = DateTime.Now;
                //end create bill
                //lấy toàn bộ dữ liệu được trả về
                foreach (string s in vnpayData)
                {
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        pay.AddResponseData(s, vnpayData[s]);
                    }
                }

                long orderId = Convert.ToInt64(pay.GetResponseData("vnp_TxnRef")); //mã hóa đơn
                long vnpayTranId = Convert.ToInt64(pay.GetResponseData("vnp_TransactionNo")); //mã giao dịch tại hệ thống VNPAY
                string vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode"); //response code: 00 - thành công, khác 00 - xem thêm https://sandbox.vnpayment.vn/apis/docs/bang-ma-loi/
                string vnp_SecureHash = Request.QueryString["vnp_SecureHash"]; //hash của dữ liệu trả về
                bill.Ordercode = orderId.ToString();
                bool checkSignature = pay.ValidateSignature(vnp_SecureHash, hashSecret); //check chữ ký đúng hay không?

                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00")
                    {
                        //Thanh toán thành công
                        ViewBag.Message = "Payment success";
                        ViewBag.Bill = bill;
                        db.Orders.Add(bill);
                        db.SaveChanges();
                        List<Cart> lstcart = GetCart();
                        foreach(var item in lstcart)
                        {
                            DetailsOrder value = new DetailsOrder()
                            {
                                Productid = item.Productid,
                                Orderid = bill.Orderid,
                                Sizeid = item.Sizeid,
                                Colorid = item.Colorid,
                                Amount = item.ProductNumber,
                                TotalPrice = item.TotalProductPrice
                            };
                            db.DetailsOrders.Add(value);
                        }
                        db.SaveChanges();
                        ClearCart();
                    }
                    else
                    {
                        //Thanh toán không thành công. Mã lỗi: vnp_ResponseCode
                        ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId + " | Mã lỗi: " + vnp_ResponseCode;
                    }
                }
                else
                {
                    ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý";
                }
            }

            return View();
        }
        string getbillid()
        {
            return DateTime.Now.Year.ToString() +
                   DateTime.Now.Month.ToString() +
                   DateTime.Now.Day.ToString() +
                   DateTime.Now.Hour.ToString() +
                   DateTime.Now.Minute.ToString() +
                   DateTime.Now.Second.ToString() +
                   DateTime.Now.Year.ToString() +
                   DateTime.Now.Day.ToString();
            //co' the xai` ham` random de xao` id
        }
        public ActionResult delivery()
        {
            string billid = getbillid();
            Order bill = new Order();
            User user = Session["User"] as User;
            bill.Userid = user.Userid;
            bill.TotalPrice = TotalPrice();
            bill.Statuspay = 0;
            bill.Status = 0;
            bill.OrderDay = DateTime.Now;
            bill.Ordercode = billid;
            ViewBag.Message = "Payment success";
            ViewBag.Bill = bill;
            db.Orders.Add(bill);
            db.SaveChanges();
            List<Cart> lstcart = GetCart();
            foreach (var item in lstcart)
            {
                DetailsOrder value = new DetailsOrder()
                {
                    Productid = item.Productid,
                    Orderid = bill.Orderid,
                    Sizeid = item.Sizeid,
                    Colorid = item.Colorid,
                    Amount = item.ProductNumber,
                    TotalPrice = item.TotalProductPrice
                };
                db.DetailsOrders.Add(value);
            }
            db.SaveChanges();
            ClearCart();
            return View();
        }
    }
    
}