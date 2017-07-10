using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Threading.Tasks;
using Uzhik.Exceptions;

namespace Uzhik.Models
{
    
    public enum Access
    {
        Enabled,
        Disabled
    }

    public class User
    {
        
        public User()
        {

        }

        public int UserId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string VKPage { get; set; }
        public string MobileNumber { get; set; }
        public string Password { get; set; }

        public UserTracingSettings UserTracingSettings { get; set; }

        public List<Item> TracingItems { get; set; }

        public static async Task<Access> CheckDataIn(User user)
        {

            DBContext db = new DBContext();
            var usersCollection = db.GetCollection("users");
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("Password", user.Password) & builder.Eq("Email", user.Email);

            var suitableUsers = await usersCollection.Find(filter).ToListAsync();

            if (suitableUsers.Count > 1) throw new MoreThanOneSuitableUserException();

            return suitableUsers.Count == 0 ? Access.Disabled : Access.Enabled;

        }
    }
}