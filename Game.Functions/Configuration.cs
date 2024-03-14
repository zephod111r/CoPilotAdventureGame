using Game.Common.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;


namespace Game.Functions
{
    internal class Configuration : IAppConfiguration
    {
        private readonly ILogger<Configuration> logger;
        private readonly IConfigurationSection configuration;

        public Configuration(ILogger<Configuration> logger)
        {
            this.logger = logger;
        }

        public string? Get(ConfigurationParameter key)
        {
            logger.LogDebug("Getting configuration for {0}", key);

            string strkey = key.ToString();
            string? value =  System.Environment.GetEnvironmentVariable(strkey, System.EnvironmentVariableTarget.Process);

            return value;
        }
    }
}
