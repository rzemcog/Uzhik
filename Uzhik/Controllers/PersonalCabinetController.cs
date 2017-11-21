using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uzhik.Models;
using Uzhik.ViewModels;

namespace Uzhik.Controllers
{
    public class PersonalCabinetController : Controller
    {
        MongoContext<User> _users;
        MongoContext<Product> _products;
        List<NotificationSettingsModel> monitoredProducts;

        public PersonalCabinetController(MongoContext<User> users, MongoContext<Product> products)
        {
            _users = users;
            _products = products;
            monitoredProducts = new List<NotificationSettingsModel>();

        }
        private async Task<User> GetUser()
        {
            return (await _users.GetCollection()).FirstOrDefault(u => u.Email == User.Identity.Name);
        }
        private async Task<List<NotificationSettingsModel>> GetMonitoredProducts()
        {
            var user = await GetUser();
            if (user.MonitoredProducts.Count != 0)
            {
                foreach (var monitoredProduct in user.MonitoredProducts)
                {

                    monitoredProducts.Add(new NotificationSettingsModel()
                    {
                        Product = (await _products.GetCollection()).FirstOrDefault(p => p.Id == monitoredProduct.ProductId),
                        NotificationSettings = monitoredProduct.NotificationSettings
                    });
                }
                    
            }
            return monitoredProducts;
        }


        [Authorize]
        public async Task<IActionResult> PersonalCabinet()
        {
            var user = await GetUser();
            user.MonitoredProducts.RemoveAll(p => p.ProductId == null);
            await _users.Update(user);
            ViewBag.MonitoredProducts = await GetMonitoredProducts();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetProduct(string name)
        {
            if(name!=null)
            {
                var monitoredProducts = await GetMonitoredProducts();
                ViewBag.MonitoredProducts = monitoredProducts.Where(p => p.Product.Name.Contains(name));
            }
            return View("PersonalCabinet");
        }


        [HttpPost]
        public async Task<IActionResult> ChangeSettings(string id, NotificationSettings notificationSettings, string name)
        {
            var user = await GetUser();
            var monitoredProduct = user.MonitoredProducts.FirstOrDefault(m => m.ProductId == id);
            if (monitoredProduct == null) user.MonitoredProducts.Add(new MonitoredProduct() { ProductId = id, NotificationSettings = notificationSettings });
            else monitoredProduct.NotificationSettings = notificationSettings;
            await _users.Update(user);
            return RedirectToAction("PersonalCabinet");
        }

        public async Task<IActionResult> StopMonitoringProduct(string Sender, string Id, string Name)
        {
            var user = await GetUser();
            user.MonitoredProducts.RemoveAll(p => p.ProductId == Id);
            await _users.Update(user);
            if(Sender == "Main") return RedirectToAction("ShowProduct", "Main" , new { name = Name });
            return RedirectToAction("PersonalCabinet");
        }
    }
}
