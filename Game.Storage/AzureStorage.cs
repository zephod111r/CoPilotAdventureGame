using Microsoft.Extensions.Configuration;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Azure.Storage.Files.Shares;
using Azure.Data.Tables;
using Game.Common.Storage;

namespace Game.Storage.Azure
{
    public class AzureStorage : IStorage
    {
        public BlobServiceClient? blobServiceClient { get; }
        public QueueServiceClient? queueServiceClient { get; }
        public ShareServiceClient? shareServiceClient { get; }
        public TableServiceClient? tableServiceClient { get; }

        private BlobContainerClient staticContainerClient;

        public AzureStorage(IConfiguration configuration)
        {
            string? connectionString = configuration["AzureWebJobsStorage"];

            if (connectionString == null)
            {
                throw new System.Exception("AzureWebJobsStorage is not set");
            }

            blobServiceClient = new BlobServiceClient(connectionString);
            queueServiceClient = new QueueServiceClient(connectionString);
            shareServiceClient = new ShareServiceClient(connectionString);
            tableServiceClient = new TableServiceClient(connectionString);

            staticContainerClient = blobServiceClient.GetBlobContainerClient("copilotadvgame-static");
        }

        public async Task Save<T>(string key, T value)
        {
            throw new NotImplementedException();
        }

        public async Task<T?> Load<T>(string key)
        {
            throw new NotImplementedException();
        }

        public async Task<Stream?> LoadStatic(string key)
        {
            var response = await staticContainerClient.GetBlobClient(key).DownloadStreamingAsync();
            return response.Value.Content;
        }
    }
}
