using System;
using System.IO;
using System.Threading;
using ValidationPilotServices.ConfigService;
using ValidationPilotServices.DataReader;
using ValidationPilotServices.LoggerService;
using System.Reflection;

namespace PackageValidation
{
    public class Program
    {
        private static PackageReaderService _packageTypeInstance = null;

        public static void Main(string[] args)
        {
            try
            {
                var culture = new System.Globalization.CultureInfo("en-us");
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
                
                LoggerIni(string.Empty);
                
                LoggerService.GetGlobalLog().Info($"Version: {Assembly.GetExecutingAssembly().GetName().Version.ToString()}");
                LoggerService.GetGlobalLog().Info($"Package validation started for package {(args.Length>0?args[0]:"UNKNOWN!")}");

                bool isPathReceived = (args != null && args.Length > 0);

                if (!isPathReceived)
                {
                    WriteFatalErrorInfo("No package dir argument.");
                    Environment.Exit(1);
                }

                var packagePath = args[0];

                //remove trailing (back)slash
                if (packagePath.EndsWith("/") || packagePath.EndsWith(@"\") ){
                    packagePath=packagePath.Substring(0,packagePath.Length-1);
                }

                var packagePathInfo = new DirectoryInfo(packagePath);
                if (!packagePathInfo.Exists)
                {
                    WriteFatalErrorInfo($"Package dir does not exist {packagePathInfo.FullName}");
                    Environment.Exit(2);
                }

                string fileToValidate = null;
                if (args.Length>1){
                    fileToValidate = args[1];
                }

                LoggerIni(packagePathInfo.FullName);
                WriteDebugInfo("Start Package Validation Process.");
                WriteDebugInfo($"Package Source folder Received: {packagePathInfo.FullName}");
                WriteDebugInfo($"Package Initialization Process.");

                //start validation process - package type initialization
                if (PreStartProcessValidation() && ValidationProcessInitialization(packagePathInfo.FullName))
                {
                    WriteDebugInfo($"Start Validation Process.");
                    ValidationProcess(fileToValidate);
                }
            }
            catch (Exception ex)
            {
                    LoggerIni(string.Empty);
                    WriteFatalErrorInfo("Uncaught exception at main level", ex);
                    Environment.Exit(3);
            }
            finally {
                WriteDebugInfo("Package Validation Process Completed.");
            }
        }

        private static void LoggerIni(string receivedPath)
        {
            LoggerService.LoggerServiceIni(receivedPath);
        }

        /// <summary>
        /// This function validate compulsory files necessary to operate.
        /// If, at least, one file doesn't exist, it's not possible to continue
        /// the validation process.
        /// </summary>
        /// <returns>true - all files presented; false - at least one file doesn't presented.</returns>
        private static bool PreStartProcessValidation()
        {
            bool valid = true;

            try
            {
                MetaDataCheckService service = new MetaDataCheckService();
                valid = service.Ini();

                if (!valid)
                {
                    throw new FileNotFoundException(service.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                WriteFatalErrorInfo("Exception while PreStartProcessValidation", ex);
            }

            return valid;
        }

        private static bool ValidationProcessInitialization(string sourceFolderPath)
        {
            try
            {
                _packageTypeInstance = new PackageReaderService(sourceFolderPath);
                if (!_packageTypeInstance.Ini())
                {
                    LoggerService.GetGlobalLog().Warn($"Error in ValidationProcessInitialization: {_packageTypeInstance.ErrorMessage}");
                    throw new ArgumentException(_packageTypeInstance.ErrorMessage);
                }

                return _packageTypeInstance.IsValid;
            }
            catch (Exception ex)
            {
                WriteFatalErrorInfo("Exception in ValidationProcessInitialization",ex);
                return false;
            }
        }

        private static bool ValidationProcess(string fileToValidate)
        {
            try
            {
                DataReaderService validationService = new DataReaderService(_packageTypeInstance);
                validationService.StartValidationProcess(fileToValidate);
                return true;
            }
            catch (Exception ex)
            {
                WriteFatalErrorInfo("Exception during validation process", ex);
                return false;
            }
        }

        private static void WriteDebugInfo(string msg)
        {
            LoggerService.GetGlobalLog().Debug(msg);
            Console.WriteLine(msg);
        }

        private static void WriteFatalErrorInfo(string msg, Exception ex=null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg);
            Console.WriteLine("It's not possible to continue the validation process.");
            LoggerService.GetGlobalLog().Fatal(msg,ex);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
