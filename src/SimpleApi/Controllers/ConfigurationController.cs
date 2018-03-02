using System.Threading.Tasks;
using System.Web.Http;
using ProjectAPI.Models;

namespace ProjectAPI.Controllers
{
    public class ConfigurationController : ApiController
    {
        private readonly IMessageGenerator _messageGenerator;

        public ConfigurationController(IMessageGenerator messageGenerator)
        {
            _messageGenerator = messageGenerator;
        }

        public async Task<Message> GetAsync()
        {
            return await _messageGenerator.GenerateAsync();
        }
    }
}
