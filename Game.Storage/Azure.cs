using Microsoft.Extensions.Configuration;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Azure.Storage.Files.Shares;
using Azure.Data.Tables;

namespace Game.Storage.Azure
{
    class Storage
    {
        public BlobServiceClient? blobServiceClient { get; }
        public QueueServiceClient? queueServiceClient { get; }
        public ShareServiceClient? shareServiceClient { get; }
        public TableServiceClient? tableServiceClient { get; }

        public Storage Singleton { get; } = new Storage();

        private Storage()
        {
            IConfiguration config = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

            string? connectionString = config["AzureWebJobsStorage"];

            if(connectionString == null)
            {
                throw new System.Exception("AzureWebJobsStorage is not set");
            }

            blobServiceClient = new BlobServiceClient(connectionString);
            queueServiceClient = new QueueServiceClient(connectionString);
            shareServiceClient = new ShareServiceClient(connectionString);
            tableServiceClient = new TableServiceClient(connectionString);
        }
    }
}
