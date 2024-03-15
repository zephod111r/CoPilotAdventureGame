using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Common.Storage
{
    public interface IStorage
    {
        Task<Uri?> Upload(string key, byte[] value);
        Task<Uri?> GetFileUri(string key);
        Task Save<T>(string key, T value);
        Task<T?> Load<T>(string key);
        Task<Stream?> LoadStatic(string key);

    }
}
