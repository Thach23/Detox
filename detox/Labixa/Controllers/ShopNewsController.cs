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

namespace Labixa.Controllers
{
    public class ShopNewsController : Controller
    {
        //ShopNew
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

        public ShopNewsController(ShopController shopController, IOrderItemService orderItemService, IProductCategoryService productCategoryService, IBlogService blogService, IProductService productService, IBlogCategoryService blogCategoryService, IWebsiteAttributeService websiteAttributeService, IMomoService momoService, IOrderService orderService)
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
        // GET: /ShopNews/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult News(int? page)
        {
            int pageSize = 6;
            int pageIndex = 1;
            var blogs = _blogService.GetBlogs();
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            IPagedList<Blog> prdPageList = null;
            prdPageList = blogs.ToPagedList(pageIndex, pageSize);

            ShopFormModel shopFormModel = new ShopFormModel();
            shopFormModel.key = "";
            shopFormModel.blogsRelated = _blogService.Get3BlogNewsNewest();
            shopFormModel.blogsHelper = _blogService.GetStaticPage().OrderBy(p => p.DateCreated);
            shopFormModel.websiteAttributes = _shopController.checkWebsiteAtribute(_websiteAttributeService.GetWebsiteAttributesByType("News").ToList());
            ViewBag.shopFormModel = shopFormModel;
            return View(prdPageList);
        }

        public ActionResult NewsDetail(string slug)
        {
            var blog = _blogService.GetBlogBySlug(slug);
            ShopFormModel shopFormModel = new ShopFormModel();
            shopFormModel.key = "";
            shopFormModel.blogsRelated = _blogService.Get3BlogNewsNewest();
            shopFormModel.blogsHelper = _blogService.GetStaticPage().OrderBy(p => p.DateCreated);
            shopFormModel.websiteAttributes = _websiteAttributeService.GetWebsiteAttributesByType("News").ToList();
            foreach (var item in shopFormModel.websiteAttributes)
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
            shopFormModel.websiteAttributes = _shopController.checkWebsiteAtribute(shopFormModel.websiteAttributes);
            ViewBag.shopFormModel = shopFormModel;
            return View(blog);
        }

        public ActionResult SearchBlog(string keySearchBlog, int? page)
        {
            int pageSize = 6;
            int pageIndex = 1;
            
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            IPagedList<Blog> prdPageList = null;
            if (keySearchBlog == "")
            {
                var blogSearch = _blogService.GetBlogs();
                prdPageList = blogSearch.ToPagedList(pageIndex, pageSize);
            }
            else
            {
                var blogSearch = _blogService.GetBlogs().Where(p => p.Title.Contains(keySearchBlog) || p.Description.Contains(keySearchBlog));
                prdPageList = blogSearch.ToPagedList(pageIndex, pageSize);
            }

            ShopFormModel shopFormModel = new ShopFormModel();
            shopFormModel.keySearchBlog = keySearchBlog;
            shopFormModel.blogsRelated = _blogService.Get3BlogNewsNewest();
            shopFormModel.blogsHelper = _blogService.GetStaticPage().OrderBy(p => p.DateCreated);
            shopFormModel.websiteAttributes = _shopController.checkWebsiteAtribute(_websiteAttributeService.GetWebsiteAttributesByType("News").ToList());
            ViewBag.shopFormModel = shopFormModel;
            return View(prdPageList);
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