using Microsoft.Extensions.Configuration;
using Azure.Storage.Blobs;
using Game.Common.Storage;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Game.Storage.Azure
{

    public class AzureBlobStorage : IStorage
    {
        public Stream ObjectToStream<T>(T obj)
        {
            var memoryStream = new MemoryStream();
            var binaryFormatter = new BinaryWriter(memoryStream, Encoding.UTF8, false);

            string file = JsonConvert.SerializeObject(obj);
            binaryFormatter.Write(file);
            return memoryStream;
        }

        public T? StreamToObject<T>(Stream inFile)
        {
            var memoryStream = new MemoryStream();
            var binaryFormatter = new BinaryReader(inFile, Encoding.UTF8, false);

            string stringobj = binaryFormatter.ReadString();
            T? obj = (stringobj == null) ? default(T) : JsonConvert.DeserializeObject<T>(stringobj!);
            return obj;
        }

        public BlobServiceClient? blobServiceClient { get; }

        private BlobContainerClient staticContainerClient { get; }
        private BlobContainerClient dynamicContainerClient { get; }

        public AzureBlobStorage(IConfiguration configuration)
        {
            string? connectionString = configuration["AzureWebJobsStorage"];

            if (connectionString == null)
            {
                throw new System.Exception("AzureWebJobsStorage is not set");
            }

            blobServiceClient = new BlobServiceClient(connectionString);
            staticContainerClient = blobServiceClient.GetBlobContainerClient("copilotadvgame-static");
            dynamicContainerClient = blobServiceClient.GetBlobContainerClient("copilotadvgame-dynamic");
        }

        public async Task Save<T>(string key, T value)
        {
            await dynamicContainerClient.CreateIfNotExistsAsync();
            await dynamicContainerClient.UploadBlobAsync(key, ObjectToStream<T>(value));
        }

        public async Task<T?> Load<T>(string key)
        {
            await dynamicContainerClient.CreateIfNotExistsAsync();
            BlobClient blob = dynamicContainerClient.GetBlobClient(key);
            if(await blob.ExistsAsync() )
            {
                MemoryStream stream = new MemoryStream();
                await blob.DownloadToAsync(stream);
                return StreamToObject<T>(stream);
            }

            return default(T);
        }

        public async Task<Stream?> LoadStatic(string key)
        {
            await staticContainerClient.CreateIfNotExistsAsync();
            BlobClient blob = staticContainerClient.GetBlobClient(key);
            return await blob.OpenReadAsync();
        }
    }
}
