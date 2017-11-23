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
       // [BsonElement("avalaible")]

        [BsonElement("available")]
        public bool Available { get; set; }
        [BsonElement("history")]
        public List<History> History = new List<History>();

        [BsonElement("subscribers")]
        public List<string> Subscribers = new List<string>();

    }

    public class History
    {
        [BsonElement("price")]
        public int Price { get; set; }
        [BsonElement("date")]
        public string Date { get; set; }
    }
}
