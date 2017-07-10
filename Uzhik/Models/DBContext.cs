using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Configuration;

namespace Uzhik.Models
{
    public class DBContext
    {
        MongoClient client;
        IMongoDatabase database;

        public DBContext()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MongoDB"].ConnectionString;
            var con = new MongoUrlBuilder(connectionString);

            client = new MongoClient(connectionString);
            database = client.GetDatabase(con.DatabaseName);
        }

        public DBContext(string connectionName)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;
            var con = new MongoUrlBuilder(connectionString);

            client = new MongoClient(connectionString);
            database = client.GetDatabase(con.DatabaseName);
        }

        public IMongoCollection<BsonDocument> GetCollection(string name)
        {
            return database.GetCollection<BsonDocument>(name);
        }




    }
}