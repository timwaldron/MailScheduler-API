using MailScheduler.Config;
using MailScheduler.Models.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MailScheduler.Repositories
{
    public class RepositoryBase<T> where T : class
    {
        private readonly IMongoCollection<T> _items;
        public virtual string CollectionName { get; set; }

        public RepositoryBase(IAppSettings settings)
        {
            var client = new MongoClient(settings.Database.ConnectionUrl);
            var database = client.GetDatabase(settings.Database.Name);

            var conventionPack = new ConventionPack { new CamelCaseElementNameConvention() };
            ConventionRegistry.Register("camelCase", conventionPack, t => true);

            _items = database.GetCollection<T>(CollectionName);
        }

        public T GetById(string id)
        {
            var filter = Builders<T>.Filter.Eq("Id", id);
            var result = _items.Find(filter).FirstOrDefault();

            return result;
        }

        public List<T> FindByQuery(FilterDefinition<T> filter)
        {
            var result = _items.Find(filter).ToList();
            return result;
        }

        public List<T> FindAll()
        {
            var result = _items.Find(_ => true).ToList();
            return result;
        }

        // Create document if it doesn't exist, overwrite it if it does exist
        public string UpsertDocument(FilterDefinition<T> filter, T entity)
        {
            var result = _items.ReplaceOne(filter, entity, new ReplaceOptions() { IsUpsert = true });
            return result.UpsertedId?.ToString();
        }
    }
}
