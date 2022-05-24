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
using System.IO;

namespace Labixa.Controllers
{
    public class ShopContactController : Controller
    {
        //ShopContact
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

        public ShopContactController(ShopController shopController, IOrderItemService orderItemService, IProductCategoryService productCategoryService, IBlogService blogService, IProductService productService, IBlogCategoryService blogCategoryService, IWebsiteAttributeService websiteAttributeService, IMomoService momoService, IOrderService orderService)
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
        // GET: /ShopContact/
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Contact()
        {
            ShopFormModel shopFormModel = new ShopFormModel();
            shopFormModel.blogsHelper = _blogService.GetStaticPage().OrderBy(p => p.DateCreated);
            shopFormModel.websiteAttributes = _shopController.checkWebsiteAtribute(_websiteAttributeService.GetWebsiteAttributesByType("Contact").ToList());
            ViewBag.shopFormModel = shopFormModel;
            return View();
        }
        [HttpPost]
        public ActionResult SendContact(string name, string phone, string email, string messenger)
        {
            MailFormModel mailFormModel = new MailFormModel();
            mailFormModel.Messenger = messenger;
            mailFormModel.Phone = phone;
            mailFormModel.Name = name;
            mailFormModel.Address = "";
            string mess = "";
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress("nhokthach007@gmail.com");
                message.To.Add(new MailAddress(email));
                message.Subject = "Detox - Thông báo đóng góp ý kiến";
                message.IsBodyHtml = true; //to make message body as html  
                message.Body = ConvertViewToString("_PartialViewMail", mailFormModel);
       
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com"; //for gmail host  
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("nhokthach007@gmail.com", "0938707235");
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
                mess = "Gửi thông tin thành công";
                Session["ShoppingCart"] = new List<Product>();
            }
            catch (Exception e)
            {
                mess = "Gửi thông tin thất bại do không thể gửi mail";
            }

            return Json(JsonRequestBehavior.AllowGet);
        }

        //private static string RenderPartialViewToString(Controller controller, string viewName, Object model)
        //{
        //    using (StringWriter sw = new StringWriter())
        //    {
        //        ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
        //        controller.ViewData.Model = model;

        //        ViewContext viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
        //        viewResult.View.Render(viewContext, sw);

        //        return sw.ToString();
        //    }
        //}

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