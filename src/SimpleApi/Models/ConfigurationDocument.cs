using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProjectAPI.Models
{
    public class ConfigurationDocument
    {
        [BsonId]
        public int DivisionId { get; set; }

        public ConfigurationSubDocument Configuration { get; set; }

        public Dictionary<string, ConfigurationSubDocument> PerApplicationExceptions { get; set; }
    }

    public class ConfigurationSubDocument
    {
        public BsonDocument Value { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Unspecified)]
        public DateTime LastUpdate { get; set; }
    }
}
