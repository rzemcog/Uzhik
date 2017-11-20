using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Uzhik.Models
{
    public class Shop : IMongoDocument
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Address { get; set; }
    }
}
