using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EduSyncAPI.Services
{
    public class EventHubService
    {
        private readonly string _connectionString;
        private readonly string _hubName;

        public EventHubService(IConfiguration config)
        {
            _connectionString = config["EventHubConfig:ConnectionString"];
            _hubName = config["EventHubConfig:HubName"];
        }

        public async Task SendEventAsync<T>(T eventData)
        {
            await using var producerClient = new EventHubProducerClient(_connectionString, _hubName);
            using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();

            string json = JsonSerializer.Serialize(eventData);
            eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(json)));

            await producerClient.SendAsync(eventBatch);
        }
    }
}
