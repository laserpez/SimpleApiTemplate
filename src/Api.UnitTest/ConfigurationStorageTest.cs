using System;
using System.Threading.Tasks;
using ConfigApi;
using MongoDB.Bson;
using NUnit.Framework;

namespace Api.UnitTest
{
    [TestFixture]
    public class ConfigurationStorageTest
    {
        private ConfigurationStorage _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new ConfigurationStorage("mongodb://test:test@devdbsmon028blq.yoox.net:27017/admin?readPreference=primary");
        }

        [TestCase("unittest")]
        public async Task Should_store_and_retrieve_config(string environment)
        {
            const int divisionId = 3;

            await _sut.DeleteConfigurationAsync(environment, divisionId);

            await _sut.CreateConfigurationAsync(environment, divisionId, new {ConnectionString = "blahblah"});

            var retrievedTree = await _sut.RetrieveConfigurationAsync(environment, divisionId);

            Assert.AreEqual("blahblah", retrievedTree.Configuration.Value.GetElement("ConnectionString").Value.AsString);

            var _ = await _sut.StoreConfigurationExceptionAsync(environment, divisionId, new {ConnectionString="bybybyby"});
            
            Assert.That(_.Matched, Is.EqualTo(1));

            retrievedTree = await _sut.RetrieveConfigurationAsync(environment, divisionId);

            Assert.AreEqual("bybybyby", retrievedTree.Configuration.Value.GetElement("ConnectionString").Value.AsString);
        }

        [Test]
        public void Should_map()
        {
            var obj_ = new {Name = "Ciccio", Value = 12};
            var doc = obj_.ToBsonDocument();

            Assert.AreEqual("Ciccio", doc["Name"].AsString);
            Assert.AreEqual(12, doc["Value"].AsInt32);
        }
    }
}
