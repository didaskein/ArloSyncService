using ArloSyncService.Common.Enumerations;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace ArloSyncService.Common.Configuration
{
    public class ConfigurationHelper : IConfigurationHelper
    {
        private readonly IConfiguration _configurationCore;
        private readonly ConfigurationEnum _configurationType;

        public ConfigurationHelper(IConfiguration configurationRoot, ConfigurationEnum configurationType = ConfigurationEnum.DotNetCore)
        {
            _configurationType = configurationType;
            _configurationCore = configurationRoot;
        }

        public string? GetSetting(string key)
        {
            if (_configurationType == ConfigurationEnum.DotNetCore && _configurationCore != null)
            {
                return _configurationCore[key];
            }
            else if (_configurationType == ConfigurationEnum.Function)
            {
                return System.Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Process);
            }
            else
            {
                throw new NotImplementedException("Configuration type not implemented");

            }

        }

        public string? GetConnectionString(string key)
        {
            if (_configurationType == ConfigurationEnum.DotNetCore && _configurationCore != null)
            {
                return _configurationCore[key];
            }
            else if (_configurationType == ConfigurationEnum.Function)
            {
                return System.Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Process);
            }
            else
            {
                throw new NotImplementedException("Configuration type not implemented");

            }
        }

        /// <summary>
        /// Gets the bool settings.
        /// </summary>
        /// <param name="nameValue">The name value.</param>
        /// <returns></returns>
        public bool GetBoolSetting(string nameValue)
        {
            bool.TryParse(GetSetting(nameValue), out bool res);
            return res;
        }

        /// <summary>
        /// Gets the bool settings.
        /// </summary>
        /// <param name="nameValue">The name value.</param>
        /// <param name="defaultValue">if set to <c>true</c> [default value].</param>
        /// <returns></returns>
        public bool GetBoolSetting(string nameValue, bool defaultValue)
        {
            bool res = defaultValue;

            if (GetSetting(nameValue) != null)
            {
                bool.TryParse(GetSetting(nameValue), out res);
            }

            return res;
        }

        /// <summary>
        /// Gets the string settings.
        /// </summary>
        /// <param name="nameValue">The name value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public string? GetStringSetting(string nameValue, string defaultValue)
        {
            string res = defaultValue;
            if (GetSetting(nameValue) != null) res = GetSetting(nameValue);

            return res;
        }

        public List<T> GetEnumSetting<T>(string nameValue, string defaultValue, char separator = ';') where T : struct, IComparable, IConvertible, IFormattable
        {
            var result = new List<T>();

            if (typeof(T).IsEnum)
            {
                var str = GetStringSetting(nameValue, defaultValue);

                if (!string.IsNullOrWhiteSpace(str))
                {
                    var splittedString = str.Split(separator);

                    foreach (var splittedElt in splittedString)
                    {
                        T element = (T)System.Enum.Parse(typeof(T), splittedElt);
                        result.Add(element);
                    }
                }
            }

            return result;
        }

        public List<string> GetListStringSetting(string nameValue, string defaultValue, char separator = ';')
        {
            var result = new List<string>();

            var str = GetStringSetting(nameValue, defaultValue);

            if (!string.IsNullOrWhiteSpace(str))
            {
                var splittedString = str.Split(separator);

                foreach (var splittedElt in splittedString)
                {
                    result.Add(splittedElt);
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the int settings.
        /// </summary>
        /// <param name="nameValue">The name value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public int GetIntSetting(string nameValue, int defaultValue)
        {
            int res = defaultValue;
            if (GetSetting(nameValue) != null)
            {
                int ires = res;
                if (int.TryParse(GetSetting(nameValue), out ires))
                {
                    res = ires;
                }
            }
            return res;
        }

        public double GetDoubleSetting(string nameValue, double defaultValue)
        {
            double res = defaultValue;
            if (GetSetting(nameValue) != null)
            {
                double ires = res;
                if (double.TryParse(GetSetting(nameValue), NumberStyles.Number, CultureInfo.CreateSpecificCulture("en-US"), out ires))
                {
                    res = ires;
                }
            }
            return res;
        }

        public Uri? GetUriSetting(string nameValue)
        {
            string? uri = GetStringSetting(nameValue, string.Empty);
            if (uri == null) return null;
            bool created = Uri.TryCreate(uri, UriKind.Absolute, out var res);
            return res;
        }

    }
}
