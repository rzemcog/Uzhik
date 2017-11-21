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
        public async Task<IActionResult> ShowProduct(string name)
        {
            var products = (await _products.GetFilteredCollection(new BsonDocument())).Where(p => p.Name.Contains(name));
            var user = await GetUser();
            foreach (Product product in products)
            {
                NotificationSettings notificationSettings = null;
                var monitoredProduct = user.MonitoredProducts.FirstOrDefault(p => p.ProductId == product.Id);
                if (monitoredProduct != null) notificationSettings = monitoredProduct.NotificationSettings; 
                notificationModel.Add(new NotificationSettingsModel() { Product = product, NotificationSettings = notificationSettings });
            }
            ViewBag.Products = notificationModel; 
            ViewBag.RequestName = name;
            return View("Index");
        }



        [HttpPost]
        public async Task<IActionResult> MonitorProduct(string id, NotificationSettings notificationSettings, string name)
        {
            var user = await GetUser();
            var monitoredProduct = user.MonitoredProducts.FirstOrDefault(m=>m.ProductId == id);
            if (monitoredProduct == null) user.MonitoredProducts.Add(new MonitoredProduct() { ProductId = id, NotificationSettings = notificationSettings });
            else monitoredProduct.NotificationSettings = notificationSettings;
            await _users.Update(user);
            return RedirectToAction("ShowProduct",new {name = name});
        }




    }
}
