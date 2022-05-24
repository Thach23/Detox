using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Outsourcing.Service;
using Labixa.ViewModels;
using Outsourcing.Data.Models;
using System.Configuration;
using Newtonsoft.Json;
using System.Net;
using System.Net.Mail;
using PagedList;
using System.Net.Mime;
using Labixa.Controllers;
using System.IO;

namespace Labixa.Controllers
{


    public class ShopProductController : Controller
    {
        readonly IProductCategoryService _productCategoryService;
        readonly IBlogCategoryService _blogCategoryService;
        readonly IBlogService _blogService;
        readonly IProductService _productService;
        readonly IWebsiteAttributeService _websiteAttributeService;
        readonly IOrderService _orderService;
        readonly IMomoService _momoService;
        readonly IOrderItemService _orderItemService;
        readonly ShopController _shopController;
        private string _accessKey;
        private string _endpoint;
        private string _partnerCode;
        private string _serectkey;
        private string _redirectUrl;
        private string _ipnUrl;
        private string _requestType;
        private string _partnerName;
        private string _storeId;
        private string _lang;
        private string _refixOrder;

        public ShopProductController(ShopController shopController, IOrderItemService orderItemService, IProductCategoryService productCategoryService, IBlogService blogService, IProductService productService, IBlogCategoryService blogCategoryService, IWebsiteAttributeService websiteAttributeService, IMomoService momoService, IOrderService orderService)
        {
            //ShopProduct
            _productCategoryService = productCategoryService;
            _blogService = blogService;
            _productService = productService;
            _blogCategoryService = blogCategoryService;
            _websiteAttributeService = websiteAttributeService;
            _orderService = orderService;
            _shopController = shopController;
            this._orderItemService = orderItemService;
            this._momoService = momoService;
            this._orderService = orderService;
            this._accessKey = ConfigurationManager.AppSettings["accessKey"];
            this._endpoint = ConfigurationManager.AppSettings["endpoint"];
            this._partnerCode = ConfigurationManager.AppSettings["partnerCode"];
            this._serectkey = ConfigurationManager.AppSettings["serectkey"];
            this._redirectUrl = ConfigurationManager.AppSettings["redirectUrl"];
            this._ipnUrl = ConfigurationManager.AppSettings["ipnUrl"];
            this._requestType = ConfigurationManager.AppSettings["requestType"];
            this._partnerName = ConfigurationManager.AppSettings["partnerName"];
            this._storeId = ConfigurationManager.AppSettings["storeId"];
            this._lang = ConfigurationManager.AppSettings["lang"];
            this._refixOrder = ConfigurationManager.AppSettings["refixOrder"];
        }

        //
        // GET: /ShopProduct/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Product(string slug, int? page)
        {
            int pageSize = 4;
            int pageIndex = 1;

            ShopFormModel shopFormModel = new ShopFormModel();
            var productCategory = _productCategoryService.GetProductCategoryBySlug(slug);
            var product = _productService.GetProductsByCategoryId(productCategory.Id);
            shopFormModel.blogsHelper = _blogService.GetStaticPage().OrderBy(p => p.DateCreated);
            shopFormModel.websiteAttributes = _shopController.checkWebsiteAtribute(_websiteAttributeService.GetWebsiteAttributesByType("Product").ToList());

            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            IPagedList<Product> prdPageList = null;
            prdPageList = product.ToPagedList(pageIndex, pageSize);

            ViewBag.productCategory = productCategory;
            ViewBag.ShopFormModel = shopFormModel;
            return View(prdPageList);
        }

        public ActionResult Search(string key, int? page)
        {
            int pageSize = 4;
            int pageIndex = 1;
            var productSearch = _productService.GetAllProducts().Where(p => p.Name.ToLower() == key.ToLower());
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            IPagedList<Product> prdPageList = null;
            prdPageList = productSearch.ToPagedList(pageIndex, pageSize);

            ShopFormModel shopFormModel = new ShopFormModel();
            shopFormModel.key = key;
            shopFormModel.blogsHelper = _blogService.GetStaticPage().OrderBy(p => p.DateCreated);
            shopFormModel.websiteAttributes = _shopController.checkWebsiteAtribute(_websiteAttributeService.GetWebsiteAttributesByType("Product").ToList());
            ViewBag.shopFormModel = shopFormModel;
            return View(prdPageList);
        }

        public ActionResult Checkout()
        {
            List<Product> listCart = (List<Product>)Session["ShoppingCart"];

            ShopFormModel shopFormModel = new ShopFormModel();
            shopFormModel.blogsHelper = _blogService.GetStaticPage().OrderBy(p => p.DateCreated);
            shopFormModel.websiteAttributes = _shopController.checkWebsiteAtribute(_websiteAttributeService.GetWebsiteAttributesByType("Home").ToList());
            foreach (var item in shopFormModel.websiteAttributes)
            {
                if (item.Description == "title")
                {
                    item.Value = "Thanh toán";
                }
                if (item.Description == "description")
                {
                    item.Value = "Thanh toán đơn hàng";
                }
                if (item.Description == "keyword")
                {
                    item.Value = "Thanh toán";
                }
                if (item.Description == "image")
                {
                    item.Value = " ";
                }
            }
            ViewBag.shopFormModel = shopFormModel;
            return View(listCart);
        }

        [HttpPost]
        public ActionResult Buy(int id)
        {
            List<Product> listCart = (List<Product>)Session["ShoppingCart"];
            if (listCart == null)
            {
                listCart = new List<Product>();
            }
            var checkProduct = listCart.Where(p => p.Id == id).FirstOrDefault();
            if (checkProduct == null)
            {
                var product = _productService.GetProductById(id);
                product.Stock = 1;
                listCart.Add(product);
            }
            else
            {
                foreach (var item in listCart)
                {
                    if (item.Id == id)
                    {
                        item.Stock++;
                    }
                }
            }

            this.Session["ShoppingCart"] = listCart;

            string message = "Thêm giỏ hàng thành công";
            return Json(new { Message = message, JsonRequestBehavior.AllowGet });
        }

        //public List<WebsiteAttribute> checkWebsiteAtribute(List<WebsiteAttribute> webSiteAtribute)
        //{
        //    foreach (var item in webSiteAtribute)
        //    {
        //        if (item.Description == "title")
        //        {
        //            if (item.Value == null || item.Value == " ")
        //            {
        //                item.Value = "Gems-Tek";
        //            }
        //        }
        //        if (item.Description == "description")
        //        {
        //            if (item.Value == null || item.Value == " ")
        //            {
        //                item.Value = "Gems-Tek";
        //            }
        //        }
        //        if (item.Description == "keyword")
        //        {
        //            if (item.Value == null || item.Value == " ")
        //            {
        //                item.Value = "Gems-Tek";
        //            }
        //        }
        //    }
        //    return webSiteAtribute;
        //}


        public ActionResult ThanhToanMomo(string phone, string address, string total, string email)
        {
            var momorequest = new ModelMomoRequest();
            var ord = new Order();
            List<Product> getCart = (List<Product>)Session["ShoppingCart"];


            if (getCart != null)
            {
                ord.OrderTotal = int.Parse(total);
                ord.CustomerAddress = address;
                ord.CustomerPhone = phone;
                ord.CustomerEmail = email;
                //ord.ProfileId = _document.Id;//id tai lieu
                ord.DateCreated = DateTime.Now;
                ord.Deleted = false;
                //ord.ProductName = _document.Name;
                //ord.Product_Slug = slug;
                ord.Status = "Waiting";
                //ord.Note = _document.DescEng;
                ord.RequestId = momorequest.requestId = Guid.NewGuid().ToString();//ma don hang
                //ord.Path_Download = _document.Tags;
                ord = _orderService.CreateOrder(ord);

            }

            foreach (var item in getCart)
            {
                var orderItem = new OrderItem();
                orderItem.OrderId = ord.Id;
                orderItem.ProductId = item.Id;
                orderItem.Quantity = item.Stock.GetValueOrDefault();
                orderItem.Price = item.Price;
                _orderItemService.CreateOrderItem(orderItem);
            }


            momorequest.orderId = _refixOrder + ord.Id.ToString();
            momorequest.amount = total;
            momorequest.partnerName = _partnerName;
            momorequest.orderInfo = phone;
            momorequest.ipnUrl = _ipnUrl;
            momorequest.lang = _lang;
            momorequest.storeId = _storeId;
            momorequest.partnerCode = _partnerCode;
            momorequest.redirectUrl = _redirectUrl;
            momorequest.requestType = _requestType;
            momorequest.extraData = "";
            momorequest.signature = HMACMOMO(momorequest.amount, momorequest.extraData, momorequest.orderId, momorequest.orderInfo, momorequest.requestId);
            momorequest.storeId = _storeId;
            var url = _momoService.PurchaseMomo(momorequest, _endpoint, _accessKey, _serectkey);
            ViewBag.url = url;

            return Redirect(url);
        }
        public ActionResult ThanhToanTrucTuyen(string phone, string address, string total, string email)
        {
            ShopFormModel shopFormModel = new ShopFormModel();
            shopFormModel.blogsHelper = _blogService.GetStaticPage().OrderBy(p => p.DateCreated);
            shopFormModel.websiteAttributes = _shopController.checkWebsiteAtribute(_websiteAttributeService.GetWebsiteAttributesByType("Home").ToList());
            foreach (var item in shopFormModel.websiteAttributes)
            {
                if (item.Description == "title")
                {
                    item.Value = "Thanh toán";
                }
                if (item.Description == "description")
                {
                    item.Value = "Thanh toán đơn hàng";
                }
                if (item.Description == "keyword")
                {
                    item.Value = "Thanh toán";
                }
                if (item.Description == "image")
                {
                    item.Value = " ";
                }
            }

            string mess = "";
            MailFormModel mailFormModel = new MailFormModel();
            mailFormModel.Messenger = "";
            mailFormModel.Phone = phone;
            mailFormModel.Name = "";
            mailFormModel.Address = address;
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress("nhokthach007@gmail.com");
                message.To.Add(new MailAddress(email));
                message.Subject = "Detox - Thông báo xác nhận đặt hàng";
                message.IsBodyHtml = true; //to make message body as html  
                message.Body = ConvertViewToString("_PartialViewMail", mailFormModel);

                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com"; //for gmail host  
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("nhokthach007@gmail.com", "0938707235");
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
                mess = "Đặt hàng thành công";
                Session["ShoppingCart"] = new List<Product>();
            }
            catch (Exception e)
            {
                mess = "Đặt hàng thất bại do không thể gửi mail";
            }
            shopFormModel.Messenger = mess;
            ViewBag.shopFormModel = shopFormModel;
            return View();
        }

        private string ConvertViewToString(string viewName, object model)
        {
            ViewData["Data"] = model;
            using (StringWriter writer = new StringWriter())
            {
                ViewEngineResult vResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext vContext = new ViewContext(this.ControllerContext, vResult.View, ViewData, new TempDataDictionary(), writer);
                vResult.View.Render(vContext, writer);
                return writer.ToString();
            }
        }

        public ActionResult RedirectMomo(string partnerCode, string orderId, string requestId, string amount, string orderInfo, string orderType, string transId, string resultCode, string message,
                                            string payType, string responseTime, string extraData, string signature)
        {
            var _signature = HMACMOMO_RESPONSE(amount, extraData, orderId, orderInfo, requestId, message, orderType, payType, responseTime, resultCode, transId);
            if (signature.Equals(_signature) && message.ToLower().Contains("successful"))
            {
                var order = _orderService.GetOrders().Where(p => p.RequestId.Equals(requestId) && p.Status.ToLower().Equals("waiting") && !p.Deleted).FirstOrDefault();
                if (order != null)
                {
                    //ok
                    order.message = message;

                    order.partnerCode = partnerCode;
                    order.payType = payType;
                    order.RequestId = requestId;
                    order.responseTime = responseTime;
                    order.resultCode = resultCode;
                    order.signature = signature;
                    order.Status = "Done";
                    order.transId = transId;
                    _orderService.EditOrder(order);
                    Session["ShoppingCart"] = new List<Product>();
                    return RedirectToRoute("TrangChu");
                }
            }
            return View();
        }
        public ActionResult Download(string tokenId)
        {
            try
            {
                var order = _orderService.GetOrders().Where(p => p.signature.Equals(tokenId) && !p.Deleted).FirstOrDefault();
                if (order != null && order.Count > 0)
                {
                    order.Count -= 1;
                    _orderService.EditOrder(order);
                    string path = Server.MapPath(order.Path_Download);
                    byte[] fileBytes = System.IO.File.ReadAllBytes(path);
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, order.Note);
                }

                //return Redirect("/tai-lieu/" + order.Product_Slug);
                return RedirectToRoute("TrangChu");
            }
            catch (Exception)
            {
            }
            return RedirectToAction("Index");
        }

        public ActionResult PSB(string partnerCode, string orderId, string requestId, string amount, string orderInfo, string orderType, string transId, string resultCode, string message,
                                            string payType, string responseTime, string extraData, string signature)
        {
            return View();
        }

        public string HMACMOMO(string amount, string extraData, string orderId, string orderInfo, string requestId)
        {
            //Before sign HMAC SHA256 signature
            string rawHash = "accessKey=" + _accessKey +
                "&amount=" + amount +
                "&extraData=" + extraData +
                "&ipnUrl=" + _ipnUrl +
                "&orderId=" + orderId +
                "&orderInfo=" + orderInfo +
                "&partnerCode=" + _partnerCode +
                "&redirectUrl=" + _redirectUrl +
                "&requestId=" + requestId +
                "&requestType=" + _requestType
                ;

            MoMoSecurity crypto = new MoMoSecurity();
            //sign signature SHA256
            string _signature = crypto.signSHA256(rawHash, _serectkey);
            return _signature;
        }
        public string HMACMOMO_RESPONSE(string amount, string extraData, string orderId, string orderInfo, string requestId, string message, string orderType, string payType, string responseTime, string resultCode, string transId)
        {
            //Before sign HMAC SHA256 signature
            string rawHash = "accessKey=" + _accessKey +
                "&amount=" + amount +
                "&extraData=" + extraData +
                 "&message=" + message +
                "&orderId=" + orderId +
                "&orderInfo=" + orderInfo +
                "&orderType=" + orderType +
                "&partnerCode=" + _partnerCode +
                "&payType=" + payType +
                "&requestId=" + requestId +
                "&responseTime=" + responseTime +
                "&resultCode=" + resultCode +
                "&transId=" + transId
                ;

            MoMoSecurity crypto = new MoMoSecurity();
            //sign signature SHA256
            string _signature = crypto.signSHA256(rawHash, _serectkey);
            return _signature;
        }

    }


}