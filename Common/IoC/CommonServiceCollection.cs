using ArloSyncService;
using ArloSyncService.Common.Configuration;
using ArloSyncService.Common.Configuration.Model;
using ArloSyncService.Common.Enumerations;
using ArloSyncService.Common.Helper;
using ArloSyncService.Logic.Arlo;
using ArloSyncService.Logic.ArloStation;
using ArloSyncService.Logic.Listener;
using ArloSyncService.Logic.Mail;
using BF.Services.Logic;


namespace BF.Services.IoC
{
    public static class CommonServiceCollection
    {
        public static void ConfigureServices(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddHostedService<WindowsBackgroundService>();

            services.AddMemoryCache();
            services.AddHttpClient();
            services.AddSingleton<IConfigurationHelper>(s => new ConfigurationHelper(configuration, ConfigurationEnum.DotNetCore));

            //ArloConfiguration
            services.AddSingleton<ArloConfiguration>(ctx =>
            {
                return new ConfigurationService<ArloConfiguration>(configuration, new ArloConfiguration()).ConfigurationOptions;
            });


            // Logic
            services.AddSingleton<ArloService>();
            services.AddSingleton<SecurityHelper>();

            services.AddSingleton<ArloClient>();
            services.AddSingleton<ArloHttpHelper>();
            services.AddSingleton<MailClient>();

            services.AddSingleton<SSEListener>();
            services.AddSingleton<MQTTListener>();
            services.AddSingleton<DeviceActivities>();

            services.AddSingleton<ArloStationClient>();

        }
    }
}
