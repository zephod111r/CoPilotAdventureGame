using Game.Common.Configuration;
using Microsoft.Extensions.Configuration;

namespace Game.Console
{
    public class LocalJsonConfiguration : IAppConfiguration
    {
        private readonly IConfigurationSection configuration;

        public LocalJsonConfiguration()
        {
            var environmentName = "local";

            var builder = new ConfigurationBuilder()
                // get paranet fodler of Environment.ProcessPath
                .SetBasePath(Directory.GetParent(Environment.ProcessPath).FullName)
                .AddJsonFile("settings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"{environmentName}.settings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configurationRoot = builder.Build();
            configuration = configurationRoot.GetSection("Application");
        }

        public string? Get(ConfigurationParameter key)
        {
            return configuration[key.ToString()];
        }
    }
}
