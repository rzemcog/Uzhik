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
        private async Task<List<NotificationSettingsModel>> GetMonitoredProducts(int begin, int count)
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
                ViewBag.PagesCount = Math.Ceiling(user.MonitoredProducts.Count / 10d);
            }
            if (monitoredProducts.Count >= (begin + 1) + count) return monitoredProducts.GetRange(begin, count);
            else return monitoredProducts.GetRange(begin, monitoredProducts.Count - begin);
        }

        private async Task<List<NotificationSettingsModel>> GetMonitoredProducts(int begin, int count, string name)
        {
            var user = await GetUser();
            if (user.MonitoredProducts.Count != 0)
            {
                foreach (var monitoredProduct in user.MonitoredProducts)
                {
                    var product = (await _products.GetCollection()).FirstOrDefault(p => p.Id == monitoredProduct.ProductId
                         && (p.Name.ToLower()).Contains(name.ToLower()));
                    if (product != null)
                    {
                        monitoredProducts.Add(new NotificationSettingsModel()
                        {
                            Product = product,
                            NotificationSettings = monitoredProduct.NotificationSettings
                        });
                    }                  
                }
                ViewBag.PagesCount = Math.Ceiling(user.MonitoredProducts.Count / 10d);
            }         
            if (monitoredProducts.Count >= (begin + 1) + count) return monitoredProducts.GetRange(begin, count);
            else return monitoredProducts.GetRange(begin, monitoredProducts.Count - begin);
        }

        [Authorize]
        public async Task<IActionResult> PersonalCabinet(string name, string page="1")
        {
            var pageInt = Convert.ToInt32(page);
            var user = await GetUser();
            //user.MonitoredProducts.RemoveAll(p => p.ProductId == null);
            //await _users.Update(user);
            if(name!=null)
                ViewBag.MonitoredProducts = await GetMonitoredProducts((pageInt - 1) * 10, 10, name); 
            else
                ViewBag.MonitoredProducts = await GetMonitoredProducts((pageInt - 1) * 10, 10);       
            ViewBag.CurrentPage = pageInt;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangeSettings(string id, NotificationSettings notificationSettings, string name, string page)
        {
            var user = await GetUser();
            var monitoredProduct = user.MonitoredProducts.FirstOrDefault(m => m.ProductId == id);
            if (monitoredProduct == null) user.MonitoredProducts.Add(new MonitoredProduct() { ProductId = id, NotificationSettings = notificationSettings });
            else monitoredProduct.NotificationSettings = notificationSettings;
            await _users.Update(user);
            return RedirectToAction("PersonalCabinet", new { page = page });
        }

        public async Task<IActionResult> StopMonitoringProduct(string Sender, string Id, string Name, string page)
        {
            var user = await GetUser();
            var product = await _products.GetDocument(Id);
            user.MonitoredProducts.RemoveAll(p => p.ProductId == Id);
            product.Subscribers.Remove(user.Id);
            await _users.Update(user);
            await _products.Update(product);
            if(Sender == "Main") return RedirectToAction("ShowProduct", "Main" , new { name = Name, page=page });
            return RedirectToAction("PersonalCabinet", new { page = page });
        }
    }
}
