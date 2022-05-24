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
using Labixa.Controllers;

namespace Labixa.Controllers
{
    public class ShopHelpController : Controller
    {
        //ShopHelp
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

        public ShopHelpController(ShopController shopController, IOrderItemService orderItemService, IProductCategoryService productCategoryService, IBlogService blogService, IProductService productService, IBlogCategoryService blogCategoryService, IWebsiteAttributeService websiteAttributeService, IMomoService momoService, IOrderService orderService)
        {
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
        // GET: /ShopHelp/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Help(string slug)
        {
            ShopFormModel shopFormModel = new ShopFormModel();
            shopFormModel.blogsHelper = _blogService.GetStaticPage().OrderBy(p => p.DateCreated);
            shopFormModel.websiteAttributes = _shopController.checkWebsiteAtribute(_websiteAttributeService.GetWebsiteAttributesByType("Help").ToList());
            var blog = _blogService.GetBlogBySlug(slug);
            ViewBag.shopFormModel = shopFormModel;
            return View(blog);
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
    }
}