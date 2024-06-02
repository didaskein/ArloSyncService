using System.ComponentModel;

namespace ArloSyncService.Common.Enumerations
{
    public enum ConfigurationEnum
    {
        [Description("Running in .net Core on an Azure WebApp")]
        DotNetCore,

        [Description("Running in .net Core on an Azure Function")]
        Function
    }
}
