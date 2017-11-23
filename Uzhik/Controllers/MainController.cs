using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uzhik.Models;
using Uzhik.ViewModels;

namespace Uzhik.Controllers
{
    public class MainController:Controller
    {
        MongoContext<Product> _products;
        MongoContext<User> _users;
        List<NotificationSettingsModel> notificationModel;


        public MainController(MongoContext<Product> products, MongoContext<User> users)
        {
            _products = products;
            _users = users;
            notificationModel = new List<NotificationSettingsModel>();           
        }


        private async Task<User> GetUser()
        {
            return (await _users.GetCollection()).FirstOrDefault(u => u.Email == User.Identity.Name);
        }

   

        [Authorize]
        public IActionResult Index()
        {
          
            return View();
        }   


        [HttpGet]
        public async Task<IActionResult> ShowProduct(string name, string page="1")
        {
            var products = (await _products.GetFilteredCollection(new BsonDocument())).Where(p => (p.Name.ToLower()).Contains(name.ToLower())).ToArray();
            var user = await GetUser();
            int pageInt = Convert.ToInt32(page);
            for(int index=(pageInt - 1)*10; index < pageInt * 10 && index < products.Length; index++)
            {
                NotificationSettings notificationSettings = null;
                var monitoredProduct = user.MonitoredProducts.FirstOrDefault(p => p.ProductId == products[index].Id);
                if (monitoredProduct != null) notificationSettings = monitoredProduct.NotificationSettings;
                notificationModel.Add(new NotificationSettingsModel() { Product = products[index], NotificationSettings = notificationSettings });
            }
            ViewBag.Products = notificationModel; 
            ViewBag.RequestName = name;
            ViewBag.PagesCount = Math.Ceiling(products.Count() / 10d);
            ViewBag.CurrentPage = pageInt;
            return View("Index");
        }



        [HttpPost]
        public async Task<IActionResult> MonitorProduct(string id, NotificationSettings notificationSettings, string name, string page)
        {
            var user = await GetUser();
            var product = await _products.GetDocument(id);
            var monitoredProduct = user.MonitoredProducts.FirstOrDefault(m=>m.ProductId == id);
            if (monitoredProduct == null)
            {
                user.MonitoredProducts.Add(new MonitoredProduct() { ProductId = id, NotificationSettings = notificationSettings });
                product.Subscribers.Add(user.Id);
            }
            else monitoredProduct.NotificationSettings = notificationSettings;
            await _users.Update(user);
            await _products.Update(product);
            return RedirectToAction("ShowProduct", new {name = name, page=page});
        }




    }
}
