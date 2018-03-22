using System.Threading;
using System.Threading.Tasks;
using ProjectAPI.Models;

namespace ProjectAPI
{
    public interface IStorage
    {
        Task<ConfigurationDocument> RetrieveConfigurationAsync(string environment, int divisionId, CancellationToken cancellationToken = default(CancellationToken));
        Task CreateConfigurationAsync(string environment, int divisionId, object configuration, CancellationToken cancellationToken = default(CancellationToken));
        Task<OpResult> StoreConfigurationExceptionAsync(string environment, int divisionId, object configuration, CancellationToken cancellationToken = default(CancellationToken));
        Task<OpResult> StoreConfigurationExceptionAsync(string environment, int divisionId, string applicationName, object applicationConfiguration, CancellationToken cancellationToken = default(CancellationToken));
        Task<ConfigurationDocument> DeleteConfigurationAsync(string environment, int divisionId);
        Task<OpResult> DeleteConfigurationExceptionAsync(string environment, int divisionId, string applicationName);
    }
}
