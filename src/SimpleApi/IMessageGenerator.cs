using System.Threading.Tasks;
using ProjectAPI.Models;

namespace ProjectAPI
{
    public interface IMessageGenerator
    {
        Task<Message> GenerateAsync();
    }

    internal class FakeMessageGenerator : IMessageGenerator
    {
        public Task<Message> GenerateAsync()
        {
            return Task.FromResult(new Message {Content = @"Hello, world!"});
        }
    }
}
