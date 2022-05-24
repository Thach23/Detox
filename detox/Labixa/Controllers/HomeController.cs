using Labixa.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Outsourcing.Service;
using Outsourcing.Data.Models;
using Outsourcing.Core.Common;
using Outsourcing.Core.Extensions;
using WebGrease.Css.Extensions;
using Outsourcing.Core.Framework.Controllers;
using PagedList;
using Labixa.ViewModels;
using System.Configuration;
using System.IO;

namespace Labixa.Controllers
{
    public class HomeController : Controller
    {
        //ShopHome
        readonly IProductService _productService;
        readonly IBlogService _blogService;
        readonly IWebsiteAttributeService _websiteAttributeService;
        readonly IProductCategoryService _productCategoryService;
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


        public HomeController(IProductService productService, IBlogService blogService, IWebsiteAttributeService websiteAttributeService,
            IProductCategoryService productCategoryService, IMomoService momoService, IOrderService orderService)
        {
            this._productCategoryService = productCategoryService;
            this._websiteAttributeService = websiteAttributeService;
            this._blogService = blogService; ;
            this._productService = productService;
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
        // GET: /Home/
        public ActionResult Index()
        {

            IndexViewModel indexViewModel = new IndexViewModel();
            indexViewModel.websiteAttributes = _websiteAttributeService.GetWebsiteAttributesByType("Home").ToList();

            var products = _productService.GetAllProducts().OrderByDescending(p => p.DateCreated).Take(8);

            ViewBag.indexViewModel = indexViewModel;
            return View(products);
        }

        public ActionResult Detail(string slug = "", string tokenid = "")
        {
            try
            {
                if (tokenid != "")
                {

                    var order = _orderService.GetOrders().Where(p => p.signature.Equals(tokenid) && !p.Deleted && p.Count > 0).FirstOrDefault();

                    if (order != null)
                    {
                        ViewBag.TokenId = tokenid;
                    }
                }
            }
            catch (Exception) { }
            IndexViewModel indexViewModel = new IndexViewModel();
            indexViewModel.websiteAttributes = _websiteAttributeService.GetWebsiteAttributesByType("Home").ToList();

            Product product = _productService.GetProductBySlug(slug);
            _productService.EditProduct(product);

            if (product != null)
            {
                foreach (var item in indexViewModel.websiteAttributes)
                {
                    if (item.Description == "title")
                    {
                        item.Value = product.MetaTitle;
                    }

                    if (item.Description == "keyword")
                    {
                        item.Value = product.MetaKeywords;
                    }

                    if (item.Description == "description")
                    {
                        item.Value = product.MetaDescription;
                    }

                    if (item.Description == "image")
                    {
                        item.Value = product.ProductImage;
                    }
                }
            }

            var productRelated = _productService.GetProductsByCategoryId(product.ProductCategoryId).Where(p => p.Id != product.Id).OrderByDescending(p => p.DateCreated).Take(8);
            indexViewModel.productRelated = productRelated;

            ViewBag.indexViewModel = indexViewModel;
            return View(product);
        }

        [HttpGet]
        public ActionResult TinTuc(int? page)
        {
            int pageSize = 6;
            int pageIndex = 1;

            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            IPagedList<Blog> blgPageList = null;

            var blogs = _blogService.GetBlogs().Where(p => p.BlogCategoryId != 2).OrderByDescending(p => p.DateCreated).ToList();
            blgPageList = blogs.ToPagedList(pageIndex, pageSize);

            IndexViewModel indexViewModel = new IndexViewModel();
            indexViewModel.websiteAttributes = _websiteAttributeService.GetWebsiteAttributesByType("News").ToList();

            indexViewModel.blogReadMore = GetFourBlogs();
            //indexViewModel.blog = GetFourBlogs().FirstOrDefault();

            ViewBag.indexViewModel = indexViewModel;
            return View(blgPageList);
        }

        public ActionResult GioiThieu()
        {
            Blog blog = _blogService.GetBlogById(16);

            IndexViewModel indexViewModel = new IndexViewModel();
            indexViewModel.websiteAttributes = _websiteAttributeService.GetWebsiteAttributesByType("About").ToList();
            if(blog != null)
            {
                foreach(var item in indexViewModel.websiteAttributes)
                {
                    if(item.Description == "keyword")
                    {
                        item.Value = blog.MetaKeywords;
                    }
                    if(item.Description == "description")
                    {
                        item.Value = blog.Description;
                    }
                    if(item.Description == "title")
                    {
                        item.Value = blog.Title;
                    }
                    if(item.Description == "image")
                    {
                        item.Value = blog.BlogImage;
                    }
                }
            }

            indexViewModel.blogReadMore = GetFourBlogs();
            //indexViewModel.blog = GetFourBlogs().FirstOrDefault();

            ViewBag.indexViewModel = indexViewModel;
            return View(blog);
        }

        public ActionResult ChiTietTinTuc(string slug)
        {
            IndexViewModel indexViewModel = new IndexViewModel();

            Blog blog = _blogService.GetBlogBySlug(slug);
            var blogsRelated = _blogService.GetBlogs().Where(p => p.Id != blog.Id).OrderByDescending(p => p.DateCreated).ToList().Take(10);
            indexViewModel.blogRelated = blogsRelated;


            indexViewModel.websiteAttributes = _websiteAttributeService.GetWebsiteAttributesByType("News").ToList();
            if (blog != null)
            {
                foreach (var item in indexViewModel.websiteAttributes)
                {
                    if (item.Description == "title")
                    {
                        item.Value = blog.MetaTitle;
                    }

                    if (item.Description == "keyword")
                    {
                        item.Value = blog.MetaKeywords;
                    }

                    if (item.Description == "description")
                    {
                        item.Value = blog.MetaDescription;
                    }

                    if (item.Description == "image")
                    {
                        item.Value = blog.ImageUrl;
                    }
                }
            }

            indexViewModel.blogReadMore = GetFourBlogs();
            //indexViewModel.blog = GetFourBlogs().FirstOrDefault();

            ViewBag.indexViewModel = indexViewModel;
            //ViewBag.blogsRelated = blogsRelated;
            return View(blog);
        }

        public ActionResult IndexCategory(string slug, int? page)
        {
            int pageSize = 6;
            int pageIndex = 1;

            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            IPagedList<Product> prdPageList = null;

            IndexViewModel indexViewModel = new IndexViewModel();
            //Lấy meta web
            indexViewModel.websiteAttributes = _websiteAttributeService.GetWebsiteAttributesByType("Document").ToList();

            //Lấy thể loại tài liệu theo slug
            ProductCategory productCategory = _productCategoryService.GetProductCategoryBySlug(slug);
            indexViewModel.productCategory = productCategory;

            //Lấy list tài liệu theo thể loại
            var products = _productService.GetProductsByCategoryId(productCategory.Id).OrderByDescending(p => p.DateCreated);
            prdPageList = products.ToPagedList(pageIndex, pageSize);

            indexViewModel.blogReadMore = GetFourBlogs();
            //indexViewModel.blog = GetFourBlogs().FirstOrDefault();

            ViewBag.indexViewModel = indexViewModel;
            return View(prdPageList);
        }

        [HttpGet]
        public ActionResult Search(string search, int? page)
        {
            int pageSize = 6;
            int pageIndex = 1;

            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            IPagedList<Blog> blgPageList = null;

            var blogs = _blogService.GetBlogs().Where(p => p.Title.Contains(search) && p.BlogCategoryId != 2 || p.Description.Contains(search) && p.BlogCategoryId != 2 || p.Content.Contains(search) && p.BlogCategoryId != 2).OrderByDescending(p => p.DateCreated).ToList();
            blgPageList = blogs.ToPagedList(pageIndex, pageSize);

            IndexViewModel indexViewModel = new IndexViewModel();
            indexViewModel.websiteAttributes = _websiteAttributeService.GetWebsiteAttributesByType("Home").ToList();
            indexViewModel.keySearch = search;
            indexViewModel.blogReadMore = GetFourBlogs();

            ViewBag.indexViewModel = indexViewModel;

            return View(blgPageList);
        }

        public IEnumerable<Blog> GetFourBlogs()
        {
            var blogs = _blogService.GetBlogs().Where(p => p.BlogCategoryId != 2).OrderByDescending(p => p.DateCreated).Take(5);
            return blogs;
        }

        public ActionResult HoTro(string slug)
        {
            var staticPage = _blogService.GetBlogBySlug(slug);
            IndexViewModel indexViewModel = new IndexViewModel();
            indexViewModel.websiteAttributes = _websiteAttributeService.GetWebsiteAttributesByType("Home").ToList();
            if(staticPage != null)
            {
                foreach(var item in indexViewModel.websiteAttributes)
                {
                    if (item.Description == "title")
                    {
                        item.Value = staticPage.MetaTitle;
                    }

                    if (item.Description == "keyword")
                    {
                        item.Value = staticPage.MetaKeywords;
                    }

                    if (item.Description == "description")
                    {
                        item.Value = staticPage.MetaDescription;
                    }

                    if (item.Description == "image")
                    {
                        item.Value = staticPage.ImageUrl;
                    }
                }
            }

            ViewBag.indexViewModel = indexViewModel;
            return View(staticPage);
        }
    }
}