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
using System.Net.Mime;
using System.IO;

namespace Labixa.Controllers
{
    public class ShopController : Controller
    {
        //Shop
        readonly IProductCategoryService _productCategoryService;
        readonly IBlogCategoryService _blogCategoryService;
        readonly IBlogService _blogService;
        readonly IProductService _productService;
        readonly IWebsiteAttributeService _websiteAttributeService;
        readonly IOrderService _orderService;
        readonly IMomoService _momoService;
        readonly IOrderItemService _orderItemService;
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
        
        public ShopController(IOrderItemService orderItemService ,IProductCategoryService productCategoryService, IBlogService blogService, IProductService productService, IBlogCategoryService blogCategoryService, IWebsiteAttributeService websiteAttributeService, IMomoService momoService, IOrderService orderService)
        {
            _productCategoryService = productCategoryService;
            _blogService = blogService;
            _productService = productService;
            _blogCategoryService = blogCategoryService;
            _websiteAttributeService = websiteAttributeService;
            _orderService = orderService;
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

        public string ConvertViewToString(string viewName, object model)
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
    }
}