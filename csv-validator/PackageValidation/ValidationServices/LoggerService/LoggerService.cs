using log4net;
using log4net.Config;
using log4net.Repository;
using System.IO;
using System.Reflection;
using ValidationPilotServices.ConfigService;
using System;

namespace ValidationPilotServices.LoggerService
{
    public static class LoggerService
    {
        private static readonly ILoggerRepository Repository = LogManager.GetRepository(Assembly.GetEntryAssembly());

        private static ILog _globalLog;
        private static ILog _validationProcessingLog;
        private static ILog _validationErrors;

        public static void LoggerServiceIni(string packageDir)
        {
            log4net.GlobalContext.Properties["PackageDir"] = packageDir;
            log4net.GlobalContext.Properties["GlobalLogDir"] = ConfigReader.GetGlobalLogDirectoryName();

            var appConfigFi = new FileInfo(Path.Join(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "App.config"));
            XmlConfigurator.Configure(Repository, appConfigFi);

            _validationProcessingLog = LogManager.GetLogger(Repository.Name, "validation-processing");
            _validationErrors = LogManager.GetLogger(Repository.Name, "validation-errors");
            _globalLog = LogManager.GetLogger(Repository.Name, "global-log");
        }

        /// <summary>
        /// This function returns Logger to write global application information.
        /// </summary>
        /// <returns>The Package application Logger.</returns>
        public static ILog GetValidationProcessingLog()
        {
            return _validationProcessingLog;
        }

        /// <summary>
        /// This function returns Logger to write file validation information in the package folder.
        /// </summary>
        /// <returns>The file Logger.</returns>
        public static ILog GetValidationErrorsLog()
        {
            return _validationErrors;
        }
        public static ILog GetGlobalLog()
        {
            return _globalLog;
        }
    }
}
