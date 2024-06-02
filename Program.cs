using ArloSyncService;
using ArloSyncService.Common.Helper;
using BF.Services.IoC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;
using Microsoft.Extensions.Options;
using System.Reflection.Metadata;

public class Program
{
    public static void Main(string[] args)
    {
        // Doc : https://learn.microsoft.com/en-us/dotnet/core/extensions/windows-service

        // Publish the App in Visual Studio 
        //To run in Admin pwsh : 
        // sc.exe create "ArloSync" binpath= "C:\Temps\ArloDomotique\ArloSyncService\bin\Release\net8.0\win-x64\publish\win-x64\ArloSyncService.exe --contentRoot C:\Temps\ArloDomotique\ArloSyncService\bin\Release\net8.0\win-x64\publish\win-x64\"
        // sc.exe start "ArloSync"
        // sc.exe stop "ArloSync"
        // sc.exe delete "ArloSync"
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddWindowsService(options =>
        {
            options.ServiceName = "ArloSync Service";
        });

        // Add Configuration
        builder.Environment.ContentRootPath = Directory.GetCurrentDirectory();
        builder.Configuration.AddJsonFile("appsettings.json", optional: true);
        builder.Configuration.AddEnvironmentVariables(prefix: "ARLO_");
        builder.Configuration.AddCommandLine(args);

        LoggerProviderOptions.RegisterProviderOptions<EventLogSettings, EventLogLoggerProvider>(builder.Services);

        builder.Services.ConfigureServices(builder.Configuration);

        IHost host = builder.Build();
        host.Run();
    }
}