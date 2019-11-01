using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Configuration;
using PIDI.Models;

namespace PIDI.App_Start
{
    public class MongoDBContext
    {
        public IMongoDatabase database;

        public MongoDBContext()
        {
            var mongoClient = new MongoClient(ConfigurationManager.AppSettings["MongoDBHost"]);
            database = mongoClient.GetDatabase(ConfigurationManager.AppSettings["MongoDBName"]);
        }

    }
}