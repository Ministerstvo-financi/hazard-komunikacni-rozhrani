using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ValidationPilotServices.ConfigService;
using ValidationPilotServices.DataTypes;
using ValidationPilotServices.Infrastructure;
using ValidationPilotServices.Infrastructure.Attributes;
using ValidationPilotServices.Infrastructure.Enums;
using ValidationPilotServices.Infrastructure.Extensions;
using ValidationPilotServices.SchemaReader;
using ValidationPilotServices.SchemaReader.Profiles;

namespace ValidationPilotServices.DataReader
{
    /// <summary>
    /// The Package type class specifies fields, properties, methods and function
    /// allows to validate files received.
    /// The whole validation process starts from receiving package name string.
    /// This string validated specifies basic properties of the files received in
    /// this package.There are the Operator ID, the Model, the Reporting Period,
    /// The Game Type (only for remote access files), The Game Location (only for
    /// daily  reports files) and Data Package Version must be defined there.
    /// </summary>
    public class PackageReaderService : ValidationCore
    {
        #region Fields

        private readonly int _idOperatorGroup = 1;
        private readonly int _packageModelGroup = 2;
        private readonly int _reportingPeriodGroup = 3;
        private readonly int _gameTypeGroup = 4;
        private readonly int _remoteVersionGroup = 5;
        private readonly int _casinoGroup = 5;
        private readonly int _dailyVersionGroup = 6;

        private readonly int _remoteGroupsCount = 6;
        private readonly int _dailyGroupsCount = 7;

        //Number of groups expectation after reg expression applied.
        private readonly string gameTypeAssignation = "HraDruh";
      
        //full files path string
        public readonly string package_source_full_path;
        //package name string to get package's entry parameters
        public readonly string package_number;

        public  List<string> FilesCollection { get; private set; } = new List<string>();

        private readonly EnumPackageTypeReport _packageType;

        private DateTime _reportingPeriod = default(DateTime); 

        public CodeBookCollection CodeBook { get; private set; }

        public Fields FieldsCollection { get; private set; }

        #endregion

        private string RegExpression
        {
            get
            {
                switch (this._packageType)
                {
                    case EnumPackageTypeReport.REMOTE:
                        return ConfigSettings.remote_regex;
                    case EnumPackageTypeReport.DAILY:
                        return ConfigSettings.daily_regex;
                    default:
                        LoggerService.LoggerService.GetGlobalLog().Warn("RegExpression.get(): The regular Expression for this Package is not defined");
                        throw new ArgumentException("The regular Expression for this Package is not defined");
                }
            }
        }

        private string ReportingPeriodDateFormat
        {
            get
            {
                switch (this._packageType)
                {
                    case EnumPackageTypeReport.REMOTE:
                        return ConfigSettings.remote_reporting_period_parameter_format;
                    case EnumPackageTypeReport.DAILY:
                        return ConfigSettings.daily_reporting_period_parameter_format;
                    default:
                        return string.Empty;
                }
            }
        }

        private int ExpectedGroupsCount
        {
            get
            {
                switch (this._packageType)
                {
                    case EnumPackageTypeReport.REMOTE:
                        return this._remoteGroupsCount;
                    case EnumPackageTypeReport.DAILY:
                        return this._dailyGroupsCount;
                    default:
                        LoggerService.LoggerService.GetGlobalLog().Warn("The Groups count parameter for this Package is not defined");
                        throw new ArgumentException("The Groups count parameter for this Package is not defined");
                }
            }
        }

        [ContextCondition(MethodToProcess = "EQUALSOPERATORID", ConditionErrorMessageTemplate = " The value {0} must be equals {1}")]
        [ContextCondition(MethodToProcess = "STARTSWITHOPERATORID", ConditionErrorMessageTemplate = " The {0} value must starts with {1}")]
        public string OperatorId { get; private set; }

        [ContextCondition(MethodToProcess = "WITHINPACKAGETIMESPAN")]
        public string ReportPeriod => this._reportingPeriod.ToString(this.ReportingPeriodDateFormat);

        public string Model { get; private set; }

        [ContextCondition(MethodToProcess = "EQUALSGAMEID")]
        public string GameType { get; private set; }

        [ContextCondition(MethodToProcess = "EQUALSLOCATIONID")]
        public string Casino { get; private set; }

        public string DataPackageVersion { get; private set; }

        /// <summary>
        /// This is the EntryPoint of the validation process.
        /// 1. split packageSourcePath and get package name
        /// </summary>
        /// <param name="packageSourcePath">Full path of the files to validate.</param>
        public PackageReaderService(string packageSourcePath)
        {
            this.package_source_full_path = !string.IsNullOrEmpty(packageSourcePath)
                ? packageSourcePath.Trim()
                : throw new ApplicationException($"The packageSourcePath parameter is empty.");
            this.package_number = this.GetPackageNameFromSourceString();

            Regex remote = new Regex(ConfigSettings.remote_regex, RegexOptions.Compiled);
            Regex daily = new Regex(ConfigSettings.daily_regex, RegexOptions.Compiled);
            if ( remote.Match(package_number).Success ){
                this._packageType = EnumPackageTypeReport.REMOTE;
            } else if ( daily.Match(package_number).Success ){
                this._packageType = EnumPackageTypeReport.REMOTE;
            } else {
                 LoggerService.LoggerService.GetGlobalLog().Warn($"The Package name {this.package_number} has invalid name.");
                 throw new ArgumentException($"The Package name {this.package_number} has invalid name.");
            }

            this.Model = GetPackageModelFromSourceString();

            //code book and fields structure initialization
            if (!this.GetCodeBook() | !this.GetFieldsStructureCollection())
            {
                throw new ApplicationException(this.ErrorMessage);
            }

            this.PackageSettingBuild();
        }

        #region PRIVATE FUNCTIONS AND METHODS

        /// <summary>
        /// This function return Package Name from the full folder path.
        /// </summary>
        /// <returns>The Package Name string.</returns>
        private string GetPackageNameFromSourceString()
        {
            DirectoryInfo di = new DirectoryInfo(this.package_source_full_path);
            if (!di.Exists)
            {
                throw new DirectoryNotFoundException($"The Folder {this.package_source_full_path} is not found.");
            }

            return di.Name;
        }

        /// <summary>
        /// This function returns Model Type (M or V) selected from package name sting.
        /// </summary>
        /// <returns>The model name of the package.</returns>
        private string GetPackageModelFromSourceString()
        {
            Regex regex = new Regex(ConfigSettings.model_regex, RegexOptions.Compiled);
            Match match = regex.Match(this.package_number, 0, this.package_number.Length);

            if (match.Groups.Count < 2)
            {
                throw new ValidationException($"Package {this.package_number} has invalid model name.");
            }
            return match.Groups[this._packageModelGroup].Value;
        }

        /// <summary>
        /// This procedure builds package parameters set for further process.
        /// 1. Has package name valid format?
        /// 2. Set Operator Id;
        /// 3. Set Reporting period. Reporting period must has valid date time format.
        /// 4. Set type assignation (Gaming location for M, Game Type for V);
        /// 5. Package Data Version.
        /// </summary>
        private void PackageSettingBuild()
        {
            Regex regex = new Regex(this.RegExpression, RegexOptions.Compiled);
            Match match = regex.Match(this.package_number, 0, this.package_number.Length);

            if (match.Groups.Count != this.ExpectedGroupsCount)
            {
                LoggerService.LoggerService.GetGlobalLog().Warn($"The Package name {this.package_number} has invalid name.");
                throw new ArgumentException($"The Package name {this.package_number} has invalid name.");
            }

            this.OperatorId = match.Groups[this._idOperatorGroup].Value;
            this._reportingPeriod = this.GetReportPeriod(match.Groups[_reportingPeriodGroup].Value);

            //validate game type value
            this.GameType = this.ValidatePackageAssignation(match.Groups[this._gameTypeGroup].Value.ToString());
            if (!this.CodeBook.IsValidValue(this.gameTypeAssignation, this.GameType, out string message))
            {
                this.AddErrorMessage(
                    $"The GameType Parameter of the Package is not defined. Value: {match.Groups[4].Value.ToString()}");
                LoggerService.LoggerService.GetGlobalLog().Warn(this.ErrorMessage);    
                throw new ArgumentException(this.ErrorMessage);
            }

            this.Casino = this._packageType == EnumPackageTypeReport.DAILY
                ? match.Groups[this._casinoGroup].Value
                : string.Empty;

            this.DataPackageVersion = this._packageType == EnumPackageTypeReport.DAILY
                ? match.Groups[this._dailyVersionGroup].Value
                : match.Groups[this._remoteVersionGroup].Value;
        }

        private string ValidatePackageAssignation(string assignationValue)
        {
            try
            {
                List<SpecialCodeBookProfile> collection;

                if (this._packageType == EnumPackageTypeReport.REMOTE)
                {
                    GameTypeCodeBookReaderService service = new GameTypeCodeBookReaderService();
                    collection = service.Get();
                }
                else
                {
                    //TODO : this is for daily report.
                    return assignationValue;
                }

                SpecialCodeBookProfile item =
                    collection.FirstOrDefault(p => p.Key.ToUpper().Equals(assignationValue.ToUpper()));

                if (item == null)
                {
                    LoggerService.LoggerService.GetGlobalLog().Warn($"Game type {assignationValue} defined in Package Number is invalid.");
                    throw new ArgumentException($"Game type {assignationValue} defined in Package Number is invalid.");
                }

                return item.Key;
            }
            catch(Exception ex)
            {
                this.AddErrorMessage(ex.Message);
                return string.Empty;
            }
        }

        /// <summary>
        /// This function returns Reporting Date of the package in case the value selected from package name has valid format.
        /// Otherwise the function invokes  ArgumentException. 
        /// </summary>
        /// <param name="value">The Reporting Date of the package string representation to validate.</param>
        /// <returns>The Reporting Date of the package.</returns>
        private DateTime GetReportPeriod(string value)
        {
            if (!DateTime.TryParseExact(value, this.ReportingPeriodDateFormat, null, DateTimeStyles.None, out DateTime date))
            {
                LoggerService.LoggerService.GetGlobalLog().Warn($"The Package Reporting Period has invalid value - {value}");
                throw new ArgumentException($"The Package Reporting Period has invalid value - {value}");
            }

            if (this._packageType == EnumPackageTypeReport.REMOTE)
            {
                if (!new int[] { 0, 8, 16 }.Contains(date.Hour))
                {
                    LoggerService.LoggerService.GetGlobalLog().Warn($"The Package Reporting Period hours has invalid value - {date.Hour}");
                    throw new ArgumentException($"The Package Reporting Period hours has invalid value - {date.Hour}");
                }
            }

            return date;
        }

        /// <summary>
        /// This function returns TRUE if all values from Code Book have beenDataReader/PackageReaderService.cs.
        /// Otherwise the function returns FALSE.
        /// </summary>
        /// <returns>True - if the code book values have been read.</returns>DataReader/PackageReaderService.cs
        private bool GetCodeBook()
        {
            try
            {
                CodeBookReaderService service = new CodeBookReaderService();
                this.CodeBook = service.Get();

                if (!this.CodeBook.Items.Any())
                {
                    throw new ArgumentNullException($"There ara no any code book items found at all.");
                }
            }
            catch (Exception ex)
            {
               LoggerService.LoggerService.GetGlobalLog().Error("Exception in GetCOdeBook",ex);
               this.AddErrorMessage($"Code book initialization process: {ex.Message}");
            }
            return this.IsValid;
        }

        /// <summary>
        /// This function returns TRUE if all fields structure collection has been read successfully.
        /// </summary>
        /// <returns></returns>
        private bool GetFieldsStructureCollection()
        {
            try
            {
                FieldsStructureReaderService service = new FieldsStructureReaderService(this.Model);
                this.FieldsCollection = service.Get();

                if (!this.FieldsCollection.Items.Any())
                {
                    throw new ArgumentNullException($"There ara no any fields found at all.");
                }
            }
            catch (Exception ex)
            {
                this.AddErrorMessage($"Fields structure initialization process: {ex.Message}");
            }

            return this.IsValid;
        }

        #endregion

        public bool Ini()
        {
            this.FilesCollection = this.FieldsCollection.Items.Select(p => p.FileSource).Distinct().ToList();
            return this.IsValid;
        }
    }
}
