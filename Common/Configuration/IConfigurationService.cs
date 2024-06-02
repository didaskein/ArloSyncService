namespace ArloSyncService.Common.Configuration
{
    public interface IConfigurationService<T> where T : class
    {
        T ConfigurationOptions { get; }
    }
}
