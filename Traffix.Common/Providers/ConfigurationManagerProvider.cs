using System;
using System.Configuration;
using Traffix.Common.IocAttributes;

namespace Traffix.Common.Providers
{
    public interface IConfigurationManagerProvider
    {
        bool IsDebugMode();
        string GetConfigValue(string key);
        bool GetConfigBoolValue(string key);

        string GetConnectionString(string name = null);
    }
    
    [Singleton]
    public class ConfigurationManagerProvider : IConfigurationManagerProvider
    {
        private readonly string _defaultConnectionStringName;

        public ConfigurationManagerProvider(string defaultConnectionStringName)
        {
            
            _defaultConnectionStringName = defaultConnectionStringName;
        }

        public bool IsDebugMode()
        {
            return GetConfigBoolValue("DebugMode");
        }

        public string GetConfigValue(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public bool GetConfigBoolValue(string key)
        {
            var value = ConfigurationManager.AppSettings[key];
            if (value == null)
                return false;
            return Convert.ToBoolean(value);
        }

        public string GetConnectionString(string name = null)
        {
            return string.IsNullOrEmpty(name)
                ? ConfigurationManager.ConnectionStrings[_defaultConnectionStringName].ConnectionString
                : ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }
    }
}