using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ValidationPilotServices.ConfigService;
using ValidationPilotServices.Infrastructure.Attributes;

namespace ValidationPilotServices.Infrastructure.Extensions
{
    public static class ConfigReaderExtensions
    {
        public static void ConfigFieldsIni<T>(this T source)
        {
            PropertyInfo[] pis = source.GetType().GetProperties()
                .Where(p => p.GetCustomAttribute<ConfigurationReaderAttribute>() != null).ToArray();

            if (pis?.Length == 0)
            {
                return;
            }

            foreach (PropertyInfo pi in pis)
            {
                ConfigurationReaderAttribute attribute = pi.GetCustomAttribute<ConfigurationReaderAttribute>();
                if (attribute == null || string.IsNullOrEmpty(attribute.ConfigurationParameter))
                {
                    continue;
                }

                if (pi.PropertyType == typeof(string))
                {
                    pi.SetValue(source, ConfigReader.GetSectionStringValue(attribute.ConfigurationParameter));
                }
                else if (pi.PropertyType == typeof(int))
                {
                    pi.SetValue(source, ConfigReader.GetSectionIntValue(attribute.ConfigurationParameter));
                }
            }
        }

        public static string GetMapPropertyFromConfig(this Type source, string propertyName)
        {
            PropertyInfo pi = source.GetProperty(propertyName);

            if (pi == null)
            {
                throw new ArgumentOutOfRangeException($"The Property {propertyName} is not found for target object.");
            }

            ConfigurationReaderAttribute attribute = pi.GetCustomAttribute<ConfigurationReaderAttribute>();
            if (attribute == null || string.IsNullOrEmpty(attribute.ConfigurationParameter)
                || !ConfigReader.IsSectionExist(attribute.ConfigurationParameter))
            {
                return propertyName;
            }

            return ConfigReader.GetSectionStringValue(attribute.ConfigurationParameter);
        }
    }
}
