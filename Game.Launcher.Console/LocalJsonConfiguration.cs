using Game.Common.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Game.TextUI
{
    public class LocalJsonConfiguration : IAppConfiguration
    {
        private readonly ILogger<LocalJsonConfiguration> logger;
        private readonly IConfigurationSection configuration;

        public LocalJsonConfiguration(ILogger<LocalJsonConfiguration> logger)
        {
            this.logger = logger;

            var environmentName = "local";

            logger.LogDebug("Environment.ProcessPath: {0}", Environment.ProcessPath);

            var builder = new ConfigurationBuilder()
                // get paranet fodler of Environment.ProcessPath
                .SetBasePath(Directory.GetParent(Environment.ProcessPath!)!.FullName)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"{environmentName}.appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configurationRoot = builder.Build();
            configuration = configurationRoot.GetSection("Application");
        }

        public string? Get(ConfigurationParameter key)
        {
            logger.LogDebug("Getting configuration for {0}", key);
            return configuration[key.ToString()];
        }
    }
}
