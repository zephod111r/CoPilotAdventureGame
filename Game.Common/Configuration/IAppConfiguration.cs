namespace Game.Common.Configuration
{
    public interface IAppConfiguration
    {
        string? Get(ConfigurationParameter key);
    }
}
