using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using ProjectAPI.Extensions;

namespace ProjectAPI
{
    using Models;

    public struct OpResult
    {
        public long Matched;
        public long Updated;
    }

    public class ConfigurationStorage: IStorage
    {
        private readonly IMongoDatabase _db;

        public ConfigurationStorage(string mongoConnectionString)
        {
            var client = new MongoClient(mongoConnectionString);
            _db = client.GetDatabase("Configurations");
        }

        public async Task CreateConfigurationAsync(string environment, int divisionId, object configuration, CancellationToken cancellationToken = new CancellationToken())
        {
            var collection = Collection<ConfigurationDocument>(environment);
            var toBeStored = new ConfigurationDocument
            {
                DivisionId = divisionId,
                Configuration = new ConfigurationSubDocument
                {
                    Value = BsonDocument.Parse(JsonConvert.SerializeObject(configuration)),
                    LastUpdate = DateTime.Now
                },
                PerApplicationExceptions = new Dictionary<string, ConfigurationSubDocument>()
            };

            await collection.InsertOneAsync(toBeStored, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<OpResult> StoreConfigurationExceptionAsync(string environment, int divisionId, object configuration, CancellationToken cancellationToken = default(CancellationToken))
        {
            var collection = Collection<ConfigurationDocument>(environment);
            var result = await collection.UpdateOneAsync(Builders<ConfigurationDocument>.Filter.Eq(_ => _.DivisionId, divisionId),
                Builders<ConfigurationDocument>.Update
                    .Set(_ => _.Configuration.Value, BsonDocument.Parse(JsonConvert.SerializeObject(configuration)))
                    .Set(_ => _.Configuration.LastUpdate, DateTime.Now),
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return result.ToOpResult();
        }

        public async Task<OpResult> StoreConfigurationExceptionAsync(string environment, int divisionId, string applicationName, object applicationConfiguration, CancellationToken cancellationToken = default(CancellationToken))
        {
            var collection = Collection<ConfigurationDocument>(environment);
            var result = await collection.UpdateOneAsync(Builders<ConfigurationDocument>.Filter.Eq(_ => _.DivisionId, divisionId),
                Builders<ConfigurationDocument>.Update
                    .Set(_ => _.PerApplicationExceptions[applicationName].Value, BsonDocument.Parse(JsonConvert.SerializeObject(applicationConfiguration)))
                    .Set(_ => _.PerApplicationExceptions[applicationName].LastUpdate, DateTime.Now),
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return result.ToOpResult();
        }

        public async Task<ConfigurationDocument> DeleteConfigurationAsync(string environment, int divisionId)
        {
            var collection = Collection<ConfigurationDocument>(environment);
            var deleted = await collection.FindOneAndDeleteAsync(Builders<ConfigurationDocument>.Filter.Eq(_ => _.DivisionId, divisionId))
                .ConfigureAwait(false);

            return deleted;
        }

        public async Task<OpResult> DeleteConfigurationExceptionAsync(string environment, int divisionId, string applicationName)
        {
            var collection = Collection<ConfigurationDocument>(environment);
            var result = await collection.UpdateOneAsync(Builders<ConfigurationDocument>.Filter.Eq(_ => _.DivisionId, divisionId),
                Builders<ConfigurationDocument>.Update.Unset(_ => _.PerApplicationExceptions[applicationName]))
                .ConfigureAwait(false);

            return result.ToOpResult();
        }

        public async Task<ConfigurationDocument> RetrieveConfigurationAsync(string environment, int divisionId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var collection = Collection<ConfigurationDocument>(environment);
            var result = await collection.Find(Builders<ConfigurationDocument>.Filter.Eq(_ => _.DivisionId, divisionId))
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            return result;
        }

        private IMongoCollection<T> Collection<T>(string environment)
        {
            return _db.GetCollection<T>($"{typeof (T).Name}:{environment}");
        }
    }
}
