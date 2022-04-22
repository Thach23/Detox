﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Outsourcing.Service;
using Labixa.ViewModels;
using Outsourcing.Data.Models;
using System.Configuration;
using Newtonsoft.Json;

namespace Labixa.Controllers
{
    public class ShopController : Controller
    {
        readonly IProductCategoryService _productCategoryService;
        readonly IBlogCategoryService _blogCategoryService;
        readonly IBlogService _blogService;
        readonly IProductService _productService;
        readonly IWebsiteAttributeService _websiteAttributeService;
        readonly IOrderService _orderService;
        readonly IMomoService _momoService;
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

        public ShopController(IProductCategoryService productCategoryService, IBlogService blogService, IProductService productService, IBlogCategoryService blogCategoryService, IWebsiteAttributeService websiteAttributeService, IMomoService momoService, IOrderService orderService)
        {
            _productCategoryService = productCategoryService;
            _blogService = blogService;
            _productService = productService;
            _blogCategoryService = blogCategoryService;
            _websiteAttributeService = websiteAttributeService;
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
        }
        //
        // GET: /Shop/
        public ActionResult Index()
         {
            ShopFormModel shopFormModel = new ShopFormModel();
            var productCategories = _productCategoryService.GetProductCategories();
            shopFormModel.productCategories = productCategories;
            List<Product> listProducts = new List<Product>();
            shopFormModel.websiteAttributes = checkWebsiteAtribute(_websiteAttributeService.GetWebsiteAttributesByType("Home").ToList());

            foreach (var item in productCategories)
            {
                var product = _productService.GetProductsByCategoryId(item.Id).OrderByDescending(p => p.DateCreated).Take(8);
                foreach(var b in product)
                {
                    listProducts.Add(b);
                }
            }

            shopFormModel.hotProducts = _productService.GetProducts().OrderByDescending(p => p.DateCreated).Take(6);
            shopFormModel.blogsHelper = _blogService.GetStaticPage().OrderBy(p => p.DateCreated);
            ViewBag.ShopFormModel = shopFormModel;
            return View(listProducts);
        }

        public ActionResult Product(string slug)
        {
            ShopFormModel shopFormModel = new ShopFormModel();
            var productCategory = _productCategoryService.GetProductCategoryBySlug(slug);
            var product = _productService.GetProductsByCategoryId(productCategory.Id);
            shopFormModel.blogsHelper = _blogService.GetStaticPage().OrderBy(p => p.DateCreated);
            shopFormModel.websiteAttributes = checkWebsiteAtribute(_websiteAttributeService.GetWebsiteAttributesByType("Product").ToList());
            ViewBag.productCategory = productCategory;
            ViewBag.ShopFormModel = shopFormModel;
            return View(product);
        }

        public ActionResult News()
        {    
            var blogs = _blogService.GetBlogs(); 

            ShopFormModel shopFormModel = new ShopFormModel();
            shopFormModel.blogsRelated = _blogService.Get3BlogNewsNewest();
            shopFormModel.blogsHelper = _blogService.GetStaticPage().OrderBy(p => p.DateCreated);
            shopFormModel.websiteAttributes = checkWebsiteAtribute(_websiteAttributeService.GetWebsiteAttributesByType("News").ToList());
            ViewBag.shopFormModel = shopFormModel;
            return View(blogs);
        }

        public ActionResult NewsDetail(string slug)
        {
            var blog = _blogService.GetBlogBySlug(slug);
            ShopFormModel shopFormModel = new ShopFormModel();
            shopFormModel.blogsRelated = _blogService.Get3BlogNewsNewest();
            shopFormModel.blogsHelper = _blogService.GetStaticPage().OrderBy(p => p.DateCreated);
            shopFormModel.websiteAttributes = _websiteAttributeService.GetWebsiteAttributesByType("News").ToList();
            foreach(var item in shopFormModel.websiteAttributes)
            {
                if (item.Description == "title")
                {
                    item.Value = blog.Title;
                }
                if (item.Description == "description")
                {
                    item.Value = blog.Description;
                }
                if (item.Description == "keyword")
                {
                    item.Value = blog.Title;
                }
                if (item.Description == "image")
                {
                    item.Value = blog.BlogImage;
                }
            }
            shopFormModel.websiteAttributes = checkWebsiteAtribute(shopFormModel.websiteAttributes);
            ViewBag.shopFormModel = shopFormModel;
            return View(blog);
        }

        public ActionResult Help(string slug)
        {
            ShopFormModel shopFormModel = new ShopFormModel();
            shopFormModel.blogsHelper = _blogService.GetStaticPage().OrderBy(p => p.DateCreated);
            shopFormModel.websiteAttributes = checkWebsiteAtribute(_websiteAttributeService.GetWebsiteAttributesByType("Help").ToList());
            var blog = _blogService.GetBlogBySlug(slug);
            ViewBag.shopFormModel = shopFormModel;
            return View(blog);
        }

        public ActionResult Contact()
        {
            ShopFormModel shopFormModel = new ShopFormModel();
            shopFormModel.blogsHelper = _blogService.GetStaticPage().OrderBy(p => p.DateCreated);
            shopFormModel.websiteAttributes = checkWebsiteAtribute(_websiteAttributeService.GetWebsiteAttributesByType("Contact").ToList());
            ViewBag.shopFormModel = shopFormModel;
            return View();
        }

        public ActionResult AboutUs()
        {
            ShopFormModel shopFormModel = new ShopFormModel();
            shopFormModel.blogsHelper = _blogService.GetStaticPage().OrderBy(p => p.DateCreated);
            shopFormModel.websiteAttributes = checkWebsiteAtribute(_websiteAttributeService.GetWebsiteAttributesByType("About").ToList());
            ViewBag.shopFormModel = shopFormModel;
            return View();
        }

        public ActionResult Search(string key)
        {
           var productSearch = _productService.GetAllProducts().Where(p => p.Name.Contains(key));

            ShopFormModel shopFormModel = new ShopFormModel();
            shopFormModel.blogsHelper = _blogService.GetStaticPage().OrderBy(p => p.DateCreated);
            shopFormModel.websiteAttributes = checkWebsiteAtribute(_websiteAttributeService.GetWebsiteAttributesByType("Product").ToList());
            ViewBag.shopFormModel = shopFormModel;
            return View(productSearch);
        }

        public ActionResult SearchBlog(string key)
        {
            var blogSearch = _blogService.GetBlogs().Where(p => p.Title.Contains(key) || p.Description.Contains(key));

            ShopFormModel shopFormModel = new ShopFormModel();
            shopFormModel.blogsRelated = _blogService.Get3BlogNewsNewest();
            shopFormModel.blogsHelper = _blogService.GetStaticPage().OrderBy(p => p.DateCreated);
            shopFormModel.websiteAttributes = checkWebsiteAtribute(_websiteAttributeService.GetWebsiteAttributesByType("News").ToList());
            ViewBag.shopFormModel = shopFormModel;
            return View(blogSearch);
        }

        public ActionResult Checkout()
        {
            List<Product> listCart = (List<Product>)Session["ShoppingCart"];

            ShopFormModel shopFormModel = new ShopFormModel();
            shopFormModel.websiteAttributes = checkWebsiteAtribute(_websiteAttributeService.GetWebsiteAttributesByType("Home").ToList());
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
            if(listCart == null)
            {
                listCart = new List<Product>();
            }
            var checkProduct = listCart.Where(p => p.Id == id).FirstOrDefault();
            if(checkProduct == null) {
                var product = _productService.GetProductById(id);
                product.Stock = 1;
                listCart.Add(product);
            }
            else
            {
                foreach(var item in listCart)
                {
                    if(item.Id == id)
                    {
                        item.Stock++;
                    }    
                }
            }

            this.Session["ShoppingCart"] = listCart;

            string message = "Thêm giỏ hàng thành công";
            return Json(new { Message = message, JsonRequestBehavior.AllowGet });
        }

        [HttpPost]
        public ActionResult DeleteItemCart(int id)
        {
            List<Product> listCart = (List<Product>)Session["ShoppingCart"];
            var product = _productService.GetProductById(id);
            listCart.Remove(product);

            this.Session["ShoppingCart"] = listCart;

            string message = "Xóa sản phẩm thành công";
            return Json(new { Message = message, JsonRequestBehavior.AllowGet });
        }

        [HttpPost]
        public ActionResult UpdateItemCart(int id, string val)
        {
            List<Product> listCart = (List<Product>)Session["ShoppingCart"];
            foreach(var item in listCart)
            {
                if (item.Id == id)
                {
                    if(val == "-")
                    {
                        if (item.Stock >= 2)
                        {
                            item.Stock--;
                        }
                    }
                    else
                    {
                        item.Stock++;
                    }
                }
            }    

            this.Session["ShoppingCart"] = listCart;

            string message = "Cập sản phẩm thành công";
            return Json(new { Message = message, JsonRequestBehavior.AllowGet });
        }

        [HttpPost]
        public ActionResult LoadCart()
        {
            List<Product> getCart = (List<Product>)Session["ShoppingCart"];
          
            return PartialView(getCart);
        }

        public ActionResult LoadCheckout()
        {
            List<Product> getCart = (List<Product>)Session["ShoppingCart"];

            return PartialView(getCart);
        }

        public List<WebsiteAttribute> checkWebsiteAtribute(List<WebsiteAttribute> webSiteAtribute)
        {
            foreach(var item in webSiteAtribute)
            {
                if(item.Description == "title")
                {
                    if(item.Value == null || item.Value == " ")
                    {
                        item.Value = "Gems-Tek";
                    }
                }
                if (item.Description == "description")
                {
                    if (item.Value == null || item.Value == " ")
                    {
                        item.Value = "Gems-Tek";
                    }
                }
                if (item.Description == "keyword")
                {
                    if (item.Value == null || item.Value == " ")
                    {
                        item.Value = "Gems-Tek";
                    }
                }
            }
            return webSiteAtribute;
        }

        public ActionResult ThanhToanMomo(string phone, string address, string total)
        {
            var momorequest = new ModelMomoRequest();
            var ord = new Order();
            List<Product> getCart = (List<Product>)Session["ShoppingCart"];


            if(getCart != null)
            {
                ord.OrderTotal = int.Parse(total);
                ord.CustomerAddress = address;
                ord.CustomerPhone = phone;
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


            //var _document = _productService.GetProductBySlug();

            //if (_document != null)
            //{
            //    ord.OrderTotal = _document.Price;
            //    ord.ProfileId = _document.Id;//id tai lieu
            //    ord.DateCreated = DateTime.Now;
            //    ord.Deleted = false;
            //    ord.ProductName = _document.Name;
            //    ord.Product_Slug = slug;
            //    ord.Status = "Waiting";
            //    ord.Note = _document.DescEng;
            //    ord.RequestId = momorequest.requestId = Guid.NewGuid().ToString();//ma don hang
            //    ord.Path_Download = _document.Tags;
            //    ord = _orderService.CreateOrder(ord);

            //}

            momorequest.orderId = ord.Id.ToString();
            //momorequest.amount = _document.Price.ToString();
            momorequest.partnerName = _partnerName;
            //momorequest.orderInfo = _document.Name;
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
                    return Redirect("/tai-lieu/" + order.Product_Slug + "?tokenid=" + order.signature);
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

                return Redirect("/tai-lieu/" + order.Product_Slug);
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