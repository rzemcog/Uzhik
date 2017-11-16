using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Uzhik.Models
{
    public class MongoContext<T>
        where T:IMongoDocument
    {
        IMongoDatabase database; // база данных
        public string CollectionName { get; set; }
        

        public MongoContext(string connectionString, string collectionName)
        {
            CollectionName = collectionName;
            var connection = new MongoUrlBuilder(connectionString);
            MongoClient client = new MongoClient(connectionString);             // получаем клиента для взаимодействия с базой данных
            database = client.GetDatabase(connection.DatabaseName);             // получаем доступ к самой базе данных
        }

        public IMongoCollection<T> Collection
        {
            get { return database.GetCollection<T>(CollectionName); }
        }

        public async Task<IEnumerable<T>> GetCollection()
        {
            return await Collection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetFilteredCollection(FilterDefinition<T> filter)
        {
            return await Collection.Find(filter).ToListAsync();
        }


        public async Task<T> GetDocument(string id)
        {
            return await Collection.Find(new BsonDocument("_id", new ObjectId(id))).FirstOrDefaultAsync();
        }

        public async Task Create(T document)
        {
            await Collection.InsertOneAsync(document);
        }

        public async Task Update(T document)
        {
            await Collection.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(document.Id)), document);
        }

        public async Task Remove(string id)
        {
            await Collection.DeleteOneAsync(new BsonDocument("_id", new ObjectId(id)));
        }

    }
}
