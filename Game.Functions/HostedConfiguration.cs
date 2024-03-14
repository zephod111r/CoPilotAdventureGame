using Game.Common.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Game.Functions
{
    public class HostedConfiguration : IAppConfiguration
    {
        private readonly ILogger<HostedConfiguration> logger;
        private readonly IConfigurationSection configuration;

        public HostedConfiguration(IConfiguration configuration, ILogger<HostedConfiguration> logger)
        {
            this.logger = logger;
            this.configuration = configuration.GetSection("Application");
        }

        public string Get(ConfigurationParameter key)
        {
            logger.LogDebug("Getting configuration for {0}", key);
            return configuration[key.ToString()];
        }
    }
}
