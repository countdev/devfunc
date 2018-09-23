using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace WhyNotRun.DAO
{
    public abstract class ContextAsyncDAO<T> where T : class
    {
        private IMongoDatabase _database;
        private MongoClient _client;

        protected ContextAsyncDAO()
        {
            var database = ConfigurationManager.AppSettings["database"].ToString();
            var databaseUrl = ConfigurationManager.AppSettings["database-url"].ToString();
            _client = new MongoClient(databaseUrl);
            _database = _client.GetDatabase(database);
        }

        protected FilterDefinitionBuilder<T> FilterBuilder
        {
            get
            {
                return Builders<T>.Filter;
            }
        }

        protected UpdateDefinitionBuilder<T> UpdateBuilder
        {
            get
            {
                return Builders<T>.Update;
            }
        }

        protected SortDefinitionBuilder<T> SortBuilder
        {
            get
            {
                return Builders<T>.Sort;
            }
        }

        protected ProjectionDefinitionBuilder<T> ProjectionBuilder
        {
            get
            {
                return Builders<T>.Projection;
            }
        }

        protected IMongoCollection<T> Collection
        {
            get
            {
                return _database.GetCollection<T>(typeof(T).Name.ToLower());
            }
        }
    }
}