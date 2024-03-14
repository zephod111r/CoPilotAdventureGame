using System.Threading.Tasks;

namespace Game.Common.Manager
{
    public interface IGameManager
    {
        async Task<string> Start() { return ""; }
    }
}
