using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uzhik.Models;
using Uzhik.Services;

namespace Uzhik.Controllers
{

    public class ChangeTrackingArgs:EventArgs
    {
        public string UserEmail { get; set; }
        public NotificationSettings NotificationSettings { get; set; }

        public string ProductName { get; set; }
        public ProductChanges ProductChanges { get; set; }
    }

    public class ProductChanges
    {
        public bool Available { get; set; }
        public int Price { get; set; }
       
    }

    public class ChangeController:Controller
    {

        MongoContext<Product> _products;
        MongoContext<User> _users;
        INotificationSender _notificationSender;
        public ChangeController(MongoContext<Product> products, MongoContext<User> users,
            INotificationSender notificationSender)
        {
            _products = products;
            _users = users;
            _notificationSender = notificationSender;
            ChangeTracked += ChangeTrackedHandler;
        }

        public event Action<ChangeTrackingArgs> ChangeTracked;

        [HttpPost]
        public async Task<IActionResult> NotificateAboutChanges(string id, string date)
        {
            
            var product = await _products.GetDocument(id);
            if (product == null) return Content("Данного товара не существует");
            if (product.Subscribers.Count==0) return Content("Никто не отслеживает данный товар");
            foreach (var _id in product.Subscribers)
            {
                var subscriber = await _users.GetDocument(_id);
                var notificationSettings = subscriber.MonitoredProducts.FirstOrDefault(m => m.ProductId == id).NotificationSettings;
                var productChanges = new ProductChanges()
                {
                    Price = product.History.FirstOrDefault(p => p.Date == date).Price,
                    Available = product.Available
                };
                var changes = new ChangeTrackingArgs()
                {
                    UserEmail = subscriber.Email,
                    NotificationSettings = notificationSettings,
                    ProductChanges = productChanges,
                    ProductName = product.Name
                };
                ChangeTracked(changes);
            }
            return Content("Запрос обработан");
        }


        private void ChangeTrackedHandler(ChangeTrackingArgs args)
        {
            string notificationMessage;
            if (Convert.ToInt32(args.NotificationSettings.NecessaryPrice) >= args.ProductChanges.Price && args.NotificationSettings.Sign == Sign.LessOrEqual ||
                Convert.ToInt32(args.NotificationSettings.NecessaryPrice) <= args.ProductChanges.Price && args.NotificationSettings.Sign == Sign.BiggerOrEqual)
            {
                 notificationMessage = EmailNotificationSender.GenerateNotificationMessage(args.NotificationSettings.Availability,
                         args.NotificationSettings.PriceChanging, args.ProductChanges.Available, args.ProductChanges.Price, args.ProductName);
            }
            else
                notificationMessage = EmailNotificationSender.GenerateNotificationMessage(args.NotificationSettings.Availability,
                         args.NotificationSettings.PriceChanging, args.ProductChanges.Available, null, args.ProductName);

            _notificationSender.Send(notificationMessage, args.UserEmail);
        }
    }
}
