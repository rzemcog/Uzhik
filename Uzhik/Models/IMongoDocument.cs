using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Uzhik.Models
{
    public interface IMongoDocument
    {
        [BsonRepresentation(BsonType.ObjectId)]
        string Id { get; set; }
    }
}
