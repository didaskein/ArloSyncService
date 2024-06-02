using System;
using System.Collections.Generic;

namespace ArloSyncService.Common.Configuration
{
    public interface IConfigurationHelper
    {
        string? GetSetting(string key);
        string? GetConnectionString(string key);

        /// <summary>
        /// Gets the bool settings.
        /// </summary>
        /// <param name="nameValue">The name value.</param>
        /// <returns></returns>
        bool GetBoolSetting(string nameValue);

        /// <summary>
        /// Gets the bool settings.
        /// </summary>
        /// <param name="nameValue">The name value.</param>
        /// <param name="defaultValue">if set to <c>true</c> [default value].</param>
        /// <returns></returns>
        bool GetBoolSetting(string nameValue, bool defaultValue);

        /// <summary>
        /// Gets the string settings.
        /// </summary>
        /// <param name="nameValue">The name value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        string? GetStringSetting(string nameValue, string defaultValue);

        List<T> GetEnumSetting<T>(string nameValue, string defaultValue, char separator = ';') where T : struct, IComparable, IConvertible, IFormattable;
        List<string> GetListStringSetting(string nameValue, string defaultValue, char separator = ';');

        /// <summary>
        /// Gets the int settings.
        /// </summary>
        /// <param name="nameValue">The name value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        int GetIntSetting(string nameValue, int defaultValue);

        double GetDoubleSetting(string nameValue, double defaultValue);
        Uri? GetUriSetting(string nameValue);
    }
}
