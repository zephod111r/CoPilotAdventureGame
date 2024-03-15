using Game.Common.Storage;

namespace Game.TextUI
{
    internal class NoStorage : IStorage
    {
        public Task<Stream> GetFile(string key)
        {
            throw new NotImplementedException();
        }

        public Task<Uri?> GetFileUri(string key)
        {
            throw new NotImplementedException();
        }

        public Task<T?> Load<T>(string key)
        {
            throw new NotImplementedException();
        }

        public Task<Stream?> LoadStatic(string key)
        {
            throw new NotImplementedException();
        }

        public Task Save<T>(string key, T value)
        {
            throw new NotImplementedException();
        }

        public Task<Uri?> Upload(string key, byte[] value)
        {
            throw new NotImplementedException();
        }
    }
}
