using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    public class MongoDbRepository<T>
    {
        public MongoClient MongoClient { get; set; }
        public IMongoDatabase MongoDatabase { get; set; }

        public IMongoCollection<T> Collection { get; set; }

        public MongoDbRepository()
        {
            MongoClient = new MongoClient("mongodb+srv://battless:apiclient@cluster0.uahcz.mongodb.net/battless");
            MongoDatabase = MongoClient.GetDatabase("battless");
            Collection = MongoDatabase.GetCollection<T>(typeof(T).Name);
        }
    }
}
