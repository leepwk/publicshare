using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ConfigurationBuilder = Microsoft.Extensions.Configuration.ConfigurationBuilder;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace TestConsoleApp.Helpers
{
    public static class ConfigHelper
    {
        private const string DOTNET5plus = @"^.NET \d{1,2}\.\d*\.?\d*?$";

        public static string GetSetting(string name, bool throwExceptions = true)
        {
            return GetSetting<string>(name, throwExceptions);
        }

        public static T GetSetting<T>(string name, bool throwExceptions = true)
        {
            string value = string.Empty;
            var runtimeVersion = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
            if (runtimeVersion.Contains(".NET Core") || Regex.IsMatch(runtimeVersion, DOTNET5plus))
            {
                if (AppSettingsExists())
                {
                    var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json").Build();

                    var appSettings = configuration.GetSection("AppSettings");

                    value = appSettings[name];
                }
            }
            else if (runtimeVersion.Contains("NET Framework"))
            {
                value = ConfigurationManager.AppSettings[name];
            }

            try
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = Environment.GetEnvironmentVariable(name);
                }

                if (!string.IsNullOrEmpty(value))
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
            }
            catch (Exception ex)
            {
                if (throwExceptions)
                {
                    throw new ArgumentNullException($"Invalid setting [{name}], value [{value}]. Error {ex}");
                }
            }

            if (!throwExceptions)
            {
                return default(T);
            }

            // App setting value is either empty or missing
            throw new ArgumentNullException($"Invalid/missing app setting [{name}]");
        }

        public static string GetConnectionString(string name)
        {
            string connectionString = string.Empty;
            var runtimeVersion = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
            if (runtimeVersion.Contains(".NET Core") || Regex.IsMatch(runtimeVersion, DOTNET5plus))
            {
                if (AppSettingsExists())
                {
                    var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json").Build();

                    connectionString = configuration.GetSection("ConnectionStrings")["ConnectionString"];
                    ;
                }
            }
            else if (runtimeVersion.Contains("NET Framework"))
            {
                if (connectionString == null)
                    throw new Exception(string.Format("Invalid connection string found for : {0}", (object)name));
                ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings[name];
                connectionString = connectionStringSettings.ConnectionString;
            }

            return connectionString;
        }

        public static IDictionary<string, string> GetAllAppSettings()
        {
            string value = string.Empty;
            var runtimeVersion = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
            if (runtimeVersion.Contains(".NET Core") || Regex.IsMatch(runtimeVersion, DOTNET5plus))
            {
                if (AppSettingsExists())
                {
                    var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json").Build();

                    var response = new Dictionary<string, string>();
                    configuration.GetSection("AppSettings").Bind(response);
                    return response;
                }
            }
            else if (runtimeVersion.Contains("NET Framework"))
            {
                return ConfigurationManager.AppSettings.AllKeys.ToDictionary(key => key, key => ConfigurationManager.AppSettings[key]);
            }
            return new Dictionary<string, string>();
        }

        public static T GetConfigSection<T>(string name) where T : class
        {
            string value = string.Empty;
            var runtimeVersion = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
            if (runtimeVersion.Contains(".NET Core") || Regex.IsMatch(runtimeVersion, DOTNET5plus))
            {
                if (AppSettingsExists())
                {
                    var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json").Build();
                    return configuration.GetSection(name).Get<T>();
                }
            }
            if (runtimeVersion.Contains("NET Framework"))
            {
                return ConfigurationManager.GetSection(name) as T;
            }

            return default(T);
        }

        private static bool AppSettingsExists()
        {
            var appSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            return File.Exists(appSettingsPath);
        }
    }
}
