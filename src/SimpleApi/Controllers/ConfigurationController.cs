using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using ProjectAPI.Models;

namespace ProjectAPI.Controllers
{
    public class ConfigurationController : ApiController
    {
        private readonly IStorage _storage;

        public ConfigurationController(IStorage storage)
        {
            _storage = storage;
        }

        [Route("{environment}/configuration/{divisionId:int}")]
        public async Task<HttpResponseMessage> GetAsync(string environment, int divisionId, string applicationName = "")
        {
            var configuration = await _storage.RetrieveConfigurationAsync(environment, divisionId);
            return configuration == null
                ? Request.CreateResponse(HttpStatusCode.NotFound)
                : Request.CreateResponse(HttpStatusCode.OK,
                    new
                    {
                        Configuration = new
                        {
                            Content = BsonSerializer.Deserialize<object>(
                                configuration.PerApplicationExceptions != null && configuration.PerApplicationExceptions.ContainsKey(applicationName)
                                    ? configuration.PerApplicationExceptions[applicationName].Value
                                    : configuration.Configuration.Value)
                        }
                    });
        }

        [Route("{environment}/configuration/{divisionId:int}")]
        public async Task<HttpResponseMessage> DeleteAsync(string environment, int divisionId, string applicationName = "")
        {
            if (string.IsNullOrWhiteSpace(applicationName))
            {
                var deletedConfiguration = await _storage.DeleteConfigurationAsync(environment, divisionId)
                    .ConfigureAwait(false);

                return deletedConfiguration != null
                    ? Request.CreateResponse(HttpStatusCode.OK, new PutConfiguration
                    {
                        DivisionId = deletedConfiguration.DivisionId,
                        Value = BsonSerializer.Deserialize<object>(deletedConfiguration.Configuration.Value)
                    })
                    : Request.CreateResponse(HttpStatusCode.NotFound);
            }
            else
            {
                var opResult = await _storage.DeleteConfigurationExceptionAsync(environment, divisionId, applicationName);
                return Request.CreateResponse(opResult.Updated > 0 ? HttpStatusCode.Accepted : HttpStatusCode.NotModified);
            }
        }

        [Route("{environment}/configuration")]
        public async Task<HttpResponseMessage> PutAsync(string environment, [FromBody] PutConfiguration configuration)
        {
            try
            {
                await _storage.CreateConfigurationAsync(environment, configuration.DivisionId, configuration.Value);
                return Request.CreateResponse(HttpStatusCode.Created);
            }
            catch (MongoWriteException e)
            {
                switch (e.WriteError.Category)
                {
                    case ServerErrorCategory.DuplicateKey:
                        return Request.CreateErrorResponse(HttpStatusCode.Forbidden, $"Resource already exists");
                    case ServerErrorCategory.ExecutionTimeout:
                        return Request.CreateErrorResponse(HttpStatusCode.GatewayTimeout, e);
                }
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        [Route("{environment}/configuration/{divisionId:int}")]
        public async Task<HttpResponseMessage> PostAsync(string environment, int divisionId, [FromBody] PostConfiguration configuration,
            string applicationName = "")
        {
            OpResult opResult;
            if (!string.IsNullOrWhiteSpace(applicationName))
            {
                opResult = await _storage.StoreConfigurationExceptionAsync(environment, divisionId, applicationName, configuration.Value)
                    .ConfigureAwait(false);
            }
            else
            {
                opResult = await _storage.StoreConfigurationExceptionAsync(environment, divisionId, configuration.Value)
                    .ConfigureAwait(false);
            }
            return opResult.Matched == 0
                ? Request.CreateResponse(HttpStatusCode.NotFound)
                : Request.CreateResponse(opResult.Updated > 0 ? HttpStatusCode.Accepted : HttpStatusCode.NotModified);
        }
    }
}
