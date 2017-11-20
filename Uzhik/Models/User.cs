using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Uzhik.Models
{
    public class User : IMongoDocument
    { 
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [Display(Name = "Имя")]
        public string Name { get; set; }
        [Display(Name = "Пароль")]
        public string Password { get; set; }
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Products")]
        public List<MonitoredProduct> MonitoredProducts = new List<MonitoredProduct>();

    }

    public class MonitoredProduct
    {
        public NotificationSettings NotificationSettings { get; set; }

        public string ProductId { get; set; }

    }

    public enum Sign
    {
        LessOrEqual,
        BiggerOrEqual
    }

    public class NotificationSettings
    {
        public List<Shop> Shops = new List<Shop>();

        public bool Availability { get; set; }

        public bool PriceChanging { get; set; }

        public string NecessaryPrice { get; set; }

        public Sign Sign { get; set; }
    }
}
