using System.Threading.Tasks;
using Game.Functions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Game.Function
{
    class Program
    {
        static async Task Main(string[] args)
        {

            var host = new HostBuilder()
            .ConfigureFunctionsWorkerDefaults()
            .Build();

            await host.RunAsync();
        }
    }
}
