using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace Uzhik.Models
{
    public class User : IMongoDocument
    { 
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("Name")]
        public string Name { get; set; }
        [BsonElement("Password")]
        public string Password { get; set; }
        [BsonElement("Email")]
        public string Email { get; set; }
        [BsonElement("RandomWord")]
        public string RandomWord { get; set; }

        [BsonElement("MonitoredProducts")]
        public List<MonitoredProduct> MonitoredProducts = new List<MonitoredProduct>();

    }


    public class MonitoredProduct
    {
        [BsonElement("NotificationSettings")]
        public NotificationSettings NotificationSettings { get; set; }

        [BsonElement("ProductId")]
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

        [BsonElement("Availability")]
        public bool Availability { get; set; }
        [BsonElement("PriceChanging")]
        public bool PriceChanging { get; set; }
        [BsonElement("NecessaryPrice")]
        public string NecessaryPrice { get; set; }
        [BsonElement("Sign")]
        public Sign Sign { get; set; }
    }
}
