using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uzhik.Models;

namespace Uzhik.Controllers
{
    public class PersonalCabinetController:Controller
    {
        MongoContext<User> _users;
        MongoContext<Product> _products;
        List<Product> monitoredProducts;

        public PersonalCabinetController(MongoContext<User> users, MongoContext<Product> products)
        {
            _users = users;
            _products = products;
            monitoredProducts = new List<Product>();

        }

        [Authorize]
        public async Task<IActionResult> PersonalCabinet()
        {
            var user = (await _users.GetCollection()).FirstOrDefault(u => u.Email == User.Identity.Name);
            if (user.MonitoredProducts.Count != 0)
            {
                foreach (var monitoredProduct in user.MonitoredProducts)
                    monitoredProducts.Add((await _products.GetCollection()).FirstOrDefault(p => p.Id == monitoredProduct.ProductId));
                ViewBag.MonitoredProducts = monitoredProducts;
            }
            return View();
        }

        [HttpGet]
        public IActionResult GetProduct(string name)
        {

            return View("PersonalCabinet");
        }

       
        public async Task<IActionResult> StopMonitoringProduct(string data)
        {
            var user = (await _users.GetCollection()).FirstOrDefault(u => u.Email == User.Identity.Name);
            user.MonitoredProducts.RemoveAll(p => p.ProductId == data);
            await _users.Update(user);
            return View("PersonalCabinet");
        }
    }
}
