using Microsoft.Extensions.Configuration;
using System;
using System.ComponentModel;

namespace ArloSyncService.Common.Configuration
{
    public class ConfigurationService<T> : IConfigurationService<T> where T : class
    {
        private readonly IConfiguration _configuration;

        public ConfigurationService(IConfiguration configuration, T configurationOptions, string configKey = null)
        {
            _configuration = configuration;

            if (String.IsNullOrEmpty(configKey))
            {
                DisplayNameAttribute displayNameAttr = null;
                Type classType = typeof(T);
                var attrs = classType.GetCustomAttributes(typeof(DisplayNameAttribute), false);
                if (attrs != null && attrs.Length > 0)
                {
                    displayNameAttr = attrs[0] as DisplayNameAttribute;
                }
                if (displayNameAttr == null)
                {
                    throw new ArgumentNullException($"ConfigKey not defined for class '{classType.Name}'. Specified the configKey parameter or add a DisplayNameAttribute on your class  '{classType.Name}'.");
                }
                configKey = displayNameAttr.DisplayName;
            }


            ConfigurationOptions = configurationOptions;
            _configuration.Bind(configKey, ConfigurationOptions);
        }

        public T ConfigurationOptions { get; private set; }
    }
}
