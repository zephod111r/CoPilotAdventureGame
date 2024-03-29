﻿using Microsoft.Extensions.Configuration;
using Azure.Storage.Blobs;
using Game.Common.Storage;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Game.Storage.Azure
{

    public class AzureBlobStorage : IStorage
    {
        private IConfiguration configuration;

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
            this.configuration = configuration;

            string? connectionString = configuration["AzureWebJobsStorage"];

            if (connectionString == null)
            {
                throw new Exception("AzureWebJobsStorage is not set");
            }

            blobServiceClient = new BlobServiceClient(connectionString);
            staticContainerClient = blobServiceClient.GetBlobContainerClient("copilotadvgame-static");
            dynamicContainerClient = blobServiceClient.GetBlobContainerClient("copilotadvgame-dynamic");
        }
        
        public async Task<Uri?> Upload(string key, byte[] value)
        {
            await dynamicContainerClient.CreateIfNotExistsAsync();
            await dynamicContainerClient.UploadBlobAsync(key, new MemoryStream(value));
            return await GetFileUri(key);
        }

        public async Task<Uri?> GetFileUri(string key)
        {
            await dynamicContainerClient.CreateIfNotExistsAsync();
            BlobClient blob = dynamicContainerClient.GetBlobClient(key);
            if (await blob.ExistsAsync())
            {
                return blob.Uri;
            }

            return null;
        }

        public async Task<Stream> GetFile(string key)
        {
            await dynamicContainerClient.CreateIfNotExistsAsync();
            BlobClient blob = dynamicContainerClient.GetBlobClient(key);
            if (await blob.ExistsAsync())
            {
                MemoryStream stream = new MemoryStream();
                await blob.DownloadToAsync(stream);
                return stream;
            }

            return new MemoryStream();
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
            if (configuration["StaticFiles"] == "local")
            {
                DirectoryInfo info = new DirectoryInfo(".");
                foreach (var item in info.EnumerateDirectories())
                {
                    if (item.Name == "wwwroot")
                    {
                        string path = $"{item.FullName}\\{key}";
                        return new FileStream(path, FileMode.Open, FileAccess.Read);
                    }
                };
                
                return  new MemoryStream();
            }
            await staticContainerClient.CreateIfNotExistsAsync();
            BlobClient blob = staticContainerClient.GetBlobClient(key);
            return await blob.OpenReadAsync();
        }
    }
}
