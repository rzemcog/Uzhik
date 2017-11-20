using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uzhik.Models;

namespace Uzhik.Controllers
{
    public class MainController:Controller
    {
        MongoContext<Product> _products;
        MongoContext<User> _users;
        List<NotificationSettings> notificationSettings;
        List<Product> products;

        public MainController(MongoContext<Product> products, MongoContext<User> users)
        {
            _products = products;
            _users = users;
            notificationSettings = new List<NotificationSettings>();
            this.products = new List<Product>();
        }



        [Authorize]
        public IActionResult Index()
        {
          
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ShowProduct(string name)
        {
            ViewBag.Products = (await _products.GetFilteredCollection(new BsonDocument())).Where(p => p.Name.Contains(name));
            return View("Index");
        }



        [HttpPost]
        public async Task<IActionResult> MonitorProduct(string id, NotificationSettings notificationSettings, string name)
        {
            var user = (await _users.GetCollection()).FirstOrDefault(u => u.Email == User.Identity.Name);
            var monitoredProduct = user.MonitoredProducts.FirstOrDefault(m=>m.ProductId == id);
            if (monitoredProduct == null) user.MonitoredProducts.Add(new MonitoredProduct() { ProductId = id, NotificationSettings = notificationSettings });
            else monitoredProduct.NotificationSettings = notificationSettings;
            await _users.Update(user);
            return RedirectToAction("ShowProduct",new {name = name});
        }




    }
}
