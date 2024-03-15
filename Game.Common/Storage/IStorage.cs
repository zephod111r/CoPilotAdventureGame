using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Common.Storage
{
    public interface IStorage
    {
        Task Save<T>(string key, T value);
        Task<T?> Load<T>(string key);
        Task<Stream?> LoadStatic(string key);

    }
}
