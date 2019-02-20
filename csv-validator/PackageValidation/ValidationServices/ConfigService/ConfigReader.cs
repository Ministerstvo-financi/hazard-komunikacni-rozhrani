using System;
using Microsoft.Extensions.Configuration;

namespace ValidationPilotServices.ConfigService
{
    public static class ConfigReader
    {
        public static readonly string RootSectionName = "SourceLocation";
        public static readonly string SourceFolderSectionName = "Folder";
        public static readonly string GlobalLogDir = "GlobalLogDir";

        public static readonly IConfigurationRoot Config;

        static ConfigReader()
        {
            Config = new ConfigurationBuilder()
                .AddJsonFile("structureSourceSettings.json")
                .Build();
        }

        #region PRIVATE FUNCTIONS AND METHODS

        private static IConfigurationSection GetSourceSection(string name)
        {
            IConfigurationSection section = Config.GetSection(name);
            if (section == null)
            {
                throw new ArgumentNullException($"The configuration section required is not found");
            }

            return section;
        }
        
        private static string GetValue(string name, string keyId)
        {
            IConfigurationSection section = GetSourceSection(name).GetSection(keyId);
            if (!section.Exists())
            {
                throw new ArgumentNullException($"The configuration section for {keyId} required is not found");
            }

            string value = section.Value;

            if (string.IsNullOrEmpty(value))
            {
                LoggerService.LoggerService.GetGlobalLog().Warn($"Error in GetValue: The settings for {keyId} has wrong value.");
                throw new ArgumentException($"The settings for {keyId} has wrong value.");
            }

            return value;
        }

        #endregion

        #region PUBLIC FUNCTIONS AND METHODS

        public static string GetSectionStringValue(string sectionName)
        {
            return GetSourceSection(sectionName).Value;
        }

        public static int GetSectionIntValue(string sectionName)
        {
            string sourceValue = GetSectionStringValue(sectionName);

            if (!int.TryParse(sourceValue, out int value))
            {
                LoggerService.LoggerService.GetGlobalLog().Warn($"Error in GetSectionIntValue: The settings for {sectionName} has wrong value.");
                throw new ArgumentException($"The settings for {sectionName} has wrong value.");
            }

            return value;
        }
        
        public static string GetSourceFolderName()
        {
            return GetValue(RootSectionName, SourceFolderSectionName);
        }

        public static string GetGlobalLogDirectoryName()
        {
            return GetSourceSection(GlobalLogDir).Value;
        }

        public static bool IsSectionExist(string sectionName)
        {
            return Config.GetSection(sectionName).Exists();
        }

        #endregion
    }
}
