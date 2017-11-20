using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Uzhik.Models
{
    public class Product:IMongoDocument
    {
        public Product()
        {

        }

        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("name")]
        public string Name { get; set; }
        [BsonElement("image")]
        public string Image { get; set; }
        [BsonElement("link")]
        public string Link { get; set; }
        [BsonElement("avalaible")]
        public bool Available { get; set; }
        [BsonElement("history")]
        public History History { get; set; }

        [BsonIgnore]
        public string Price { get; set; }
    }

    public class History
    {
        [BsonElement("price")]
        public string Price { get; set; }
        [BsonElement("time")]
        public string Time { get; set; }
    }
}
